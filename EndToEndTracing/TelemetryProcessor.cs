using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.WorkerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndToEndTracing
{
    internal class MyTelemetryProcessorFactory : ITelemetryProcessorFactory
    {
        public ITelemetryProcessor Create(ITelemetryProcessor next)
        {
            return new MyTelemetryProcessor(next);
        }
    }

    internal class MyTelemetryProcessor : ITelemetryProcessor
    {
        private readonly ITelemetryProcessor _next;

        public MyTelemetryProcessor(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry telemetry)
        {
            telemetry.Context.Properties.Add("Hola", "hehe");
            // Implement your custom telemetry processing logic here
            // You can modify or filter the telemetry data as needed

            // Call the next telemetry processor in the chain
            _next.Process(telemetry);
        }
    }
}
