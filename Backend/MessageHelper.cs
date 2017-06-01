using Het.Common;
using System;
using System.Messaging;

namespace Het.Backend
{
    public sealed class MessageHelper
    {
        private static IMessageFormatter formatter = new XmlMessageFormatter(new[] {typeof(Command)});

        public static string Send(string channel, object @body)
        {
            return Send(channel, null, @body);
        }

        public static string Send(string channel, string correlationId, object @body)
        {
            string id = null;
            
            using (var queue = GetQueue(string.Format(@".\private$\{0}", channel)))
            {
                using (var message = new Message(body))
                {
                    if (!string.IsNullOrWhiteSpace(correlationId))
                    {
                        message.CorrelationId = correlationId;
                        message.Label = correlationId;
                    }

                    queue.Send(message, MessageQueueTransactionType.Single);

                    id = message.Id;
                }
            }

            return id;
        }

        public static object Receive(string channel, TimeSpan timeout)
        {
            string @null = null;

            return Receive(channel, null, timeout, out @null);
        }

        public static object Receive(string channel, TimeSpan timeout, out string id)
        {
            return Receive(channel, null, timeout, out id);
        }

        public static object Receive(string channel, string correlationId, TimeSpan timeout)
        {
            string @null = null;

            return Receive(channel, correlationId, timeout, out @null);
        }

        public static object Receive(string channel, string correlationId, TimeSpan timeout, out string id)
        {
            object response = null;

            using (var queue = GetQueue(string.Format(@".\private$\{0}", channel)))
            {
                Message message = null;

                try
                {
                    if (string.IsNullOrWhiteSpace(correlationId))
                    {
                        message = queue.Receive(timeout, MessageQueueTransactionType.Single);
                    }
                    else
                    {
                        message = queue.ReceiveByCorrelationId(correlationId, timeout, MessageQueueTransactionType.Single);
                    }

                    response = message.Body;

                    if ( string.IsNullOrWhiteSpace(message.Label))
                    {
                        id = message.Id;
                    }
                    else
                    {
                        id = message.Label;
                    }
                }
                catch (MessageQueueException e)
                {
                    if (e.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout)
                    {
                        id = null;

                        return null;
                    }

                    throw;
                }
                finally
                {
                    if ( message != null )
                    {
                        message.Dispose();
                    }
                }
            }

            return response;
        }


        private static MessageQueue GetQueue(string path)
        {
            MessageQueue queue = null;

            if ( MessageQueue.Exists(path) )
            {
                queue = new MessageQueue(path);
            }
            else
            {
                queue = MessageQueue.Create(path, true);
            }

            queue.Formatter = formatter;

            return queue;
        }
    }
}
