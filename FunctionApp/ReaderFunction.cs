using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using SampleExtension;
using System.IO;

namespace FunctionApp
{
    public static class ReaderFunction
    {
        [FunctionName("ReaderFunction")]
        public static void Run(
            [HttpTrigger] SampleItem item,
            [Sample(FileName = "{Name}")] string contents, // Bind to SampleExtension  
            TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");
            log.Info("Contents:" + contents);

            //return req.CreateResponse(HttpStatusCode.OK, "Contents: " + contents);
        }

        // Bind to input as string
        // BindToInput<SampleItem> --> Converter --> string
        [NoAutomaticTrigger]
        public static void Reader(
            string name,  // from trigger
            [Sample(FileName = "{name}")] string contents,
            TraceWriter log)
        {
            log.Info(contents);
        }

        // Bind to input as rich type:
        // BindToInput<SampleItem> --> item
        [NoAutomaticTrigger]
        public static void Reader2(
            string name,  // from trigger
            [Sample(FileName = "{name}")] SampleItem item,
            TextWriter log)
        {
            log.WriteLine($"{item.Name}:{item.Contents}");
        }

#if false
#region Using 2nd extensions

        // Bind to input as rich type:
        // BindToInput<SampleItem> --> item
        [NoAutomaticTrigger]
        public static void Reader3(
            string name,  // from trigger
            [Sample(Name = "{name}")] CustomType<int> item,
            TextWriter log)
        {
            log.WriteLine($"Via custom type {item.Name}:{item.Value}");
        }
#endregion
#endif
    }
}