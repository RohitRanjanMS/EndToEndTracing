using System;
using System.Diagnostics;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace EndToEndTracing
{
    public class SBTrigger2
    {
        private readonly ILogger _logger;

        public SBTrigger2(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SBTrigger2>();
        }

        [Function("SBTrigger2")]
        public void Run([ServiceBusTrigger("myqueue2", Connection = "sbcon", IsBatched = true)] ServiceBusReceivedMessage[] myQueueItem)
        {
            _logger.LogInformation($"SBTrigger2 function processed messages: {myQueueItem.Length}^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
            string id = string.Empty;
            foreach (var item in myQueueItem) 
            {
                id += item.ApplicationProperties["Diagnostic-Id"] + " - ";
            }
            _logger.LogInformation($"SBTrigger2 function processed message: {id}");
            Console.Out.WriteLine($"&&&&&&&&&&&&&&&&&& SBTrigger2 - Activity.Current.Id {Activity.Current?.Id}, LinkActivities { Activity.Current?.Links.Count()}");
        }
    }
}
