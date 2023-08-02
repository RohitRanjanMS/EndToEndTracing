using System;
using System.Diagnostics;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EndToEndTracing
{
    public class SBTrigger1
    {
        private readonly ILogger _logger;

        public SBTrigger1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SBTrigger1>();
        }

        [Function("SBTrigger1")]
        [ServiceBusOutput("myqueue2", Connection = "sbcon")]
        public string Run([ServiceBusTrigger("myqueue1", Connection = "sbcon")] ServiceBusReceivedMessage myQueueItem)
        {
            _logger.LogInformation($"SBTrigger1 function processed message: {myQueueItem.ApplicationProperties["Diagnostic-Id"]}");
            var message = $"Output message created at {DateTime.Now}";
            Console.Out.WriteLine($"&&&&&&&&&&&&&&&&&& SBTrigger1 - Activity.Current.Id {Activity.Current?.Id}");
            return message;
        }
    }
}