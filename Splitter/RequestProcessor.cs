using Het.Backend;
using Het.Common;
using System;
using System.Linq;

namespace Splitter
{
    public partial class Processor 
    {
        private void ProcessRequest(object state)
        {
            var command = state as Command;
            var attribute = command.Context.Attribute as SplitterAttribute;
            var context = command.Context;
            command.Context = null; //tick para evitar serializacion

            #region Build Parameter
                var parametersDef = context.MethodInfo.GetParameters();

                var parametersVal = new object[parametersDef.Length];

                for (int i = 0; i < parametersDef.Length; i++)
                {
                    var customAttributes = parametersDef[i].GetCustomAttributes(true);

                    if (customAttributes.FirstOrDefault(_ =>
                            _.GetType().IsAssignableFrom(typeof(RequestAttribute))) != null)
                    {
                        parametersVal[i] = command.Request;
                    }
                    else if (customAttributes.FirstOrDefault(_ =>
                            _.GetType().IsAssignableFrom(typeof(ResponseAttribute))) != null)
                    {
                        parametersVal[i] = command.Response;
                    }
                    else if (customAttributes.FirstOrDefault(_ =>
                            _.GetType().IsAssignableFrom(typeof(CommandAttribute))) != null)
                    {
                        parametersVal[i] = command;
                    }
                    else
                    {
                        parametersVal[i] = null;
                    }
                }
            #endregion

            string[] messages = null;

            try
            {
                messages = context.MethodInfo.Invoke(context.Component, parametersVal) as string[];
            }
            catch (Exception e)
            {
                command.Response.Error =
                    string.Format("Coudn't invoke {0}: {1}", context.Component.GetType().FullName, e.Message);

                if (!string.IsNullOrWhiteSpace(attribute.Deadletter))
                {
                    MessageHelper.Send(attribute.Deadletter, context.Id, command);
                }
            }
            finally
            {
                if (messages != null)
                {
                    command.Length = messages.Length;

                    foreach (var message in messages)
                    {
                        command.Ordinal++;

                        command.Request.Message = message;
                        
                        MessageHelper.Send(attribute.OutputChannel, context.Id, command);
                    }
                }
            }
        }
    }
}
