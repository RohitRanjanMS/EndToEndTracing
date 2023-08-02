using System.Diagnostics;
using System.Net;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;

namespace EndToEndTracing
{
    public class HttpTrigger
    {
        private readonly ILogger _logger;
        private readonly ServiceBusSender _sender;

        public HttpTrigger(ILoggerFactory loggerFactory, IAzureClientFactory<ServiceBusClient> serviceBusClientFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpTrigger>();
            _sender = serviceBusClientFactory.CreateClient("SB").CreateSender("myqueue1");

            Random random = new Random();
            byte[] traceIdBytes = new byte[16];
            Activity.TraceIdGenerator = () =>
            {
                random.NextBytes(traceIdBytes);
                return ActivityTraceId.CreateFromBytes(traceIdBytes);
            };
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;
        }

        [Function("Function1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Sending 5 messages with a unique diagnostic-id to service bus");
            Activity.Current = null;
            for (int i = 0; i < 5; i++)
            {
                Activity act = new Activity("SendingMessages");
                act.Start();                
                await _sender.SendMessageAsync(new ServiceBusMessage(string.Format("id={0}", Guid.NewGuid())));
                act.Stop();
            }
            await Console.Out.WriteLineAsync("____________Done sending 5 messages to SB");
            return response;
        }
    }

    public class HttpTrigger2
    {
        private readonly ILogger _logger;
        private readonly ServiceBusSender _sender;

        public HttpTrigger2(ILoggerFactory loggerFactory, IAzureClientFactory<ServiceBusClient> serviceBusClientFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpTrigger>();
            _sender = serviceBusClientFactory.CreateClient("SB").CreateSender("myqueue1");
        }

        [Function("Function2")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Sending 5 messages with the same diagnostic-id to service bus");           
            for (int i = 0; i < 5; i++)
            {
                Activity act = new Activity("SendingMessages");
                act.Start();
                string msg = "{\"id\":1}";
                await _sender.SendMessageAsync(new ServiceBusMessage(msg));
                act.Stop();
            }            
            await Console.Out.WriteLineAsync($"&&&&&&&&&&&&&&&&&& Function 2 - Activity.Current.Id {Activity.Current?.Id}"   );
            return response;
        }
    }
}
