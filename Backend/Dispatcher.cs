using Het.Common;
using System;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Het.Backend
{
    [ServiceContract]
    public class Dispatcher
    {
        [WebInvoke(UriTemplate = "/execute", Method = "POST", 
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public Response Execute(Request request)
        {
            try
            {
                var command = new Command()
                {
                    Request = request,
                    Response = new Response()
                };

                string correlationId = MessageHelper.Send(request.InputChannel, command);

                var result = MessageHelper.Receive(
                    request.OutputChannel, correlationId, TimeSpan.FromSeconds(request.TimeoutSeconds)) as Command;

                result.Response.Id = correlationId;

                return result.Response;
            }
            catch (Exception e)
            {
                return new Response()
                {
                    Error = e.Message
                };
            }
        }

        [WebInvoke(UriTemplate = "/helloworld", Method = "GET",
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public string HelloWorld()
        {
            return "Hello World!";
        }

    }
}
