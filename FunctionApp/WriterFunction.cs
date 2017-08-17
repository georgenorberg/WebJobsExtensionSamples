using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SampleExtension;

namespace FunctionApp
{
    public static class WriterFunction
    {
        [FunctionName("WriterFunction")]
        public static void Run(
            [HttpTrigger] string item,
            [Sample] ICollector<SampleItem> sampleOutput, TraceWriter log)
        {
            // sampleOutput.Add($"{item.Name}:{item.Contents}");
            sampleOutput.Add(new SampleItem
            {
                 Name = "tom",
                 Contents = "hello"
            });
        }
    }
}