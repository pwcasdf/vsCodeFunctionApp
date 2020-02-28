using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Devices;


namespace OHC.Function
{
    public static class c2dFunctionApp
    {
        [FunctionName("c2dFunctionApp")]
        public static async void Run([ServiceBusTrigger("c2dqueue", Connection = "ohcsbq_SERVICEBUS")]string myQueueItem, ILogger log)
        {
            string connectionString = "IOTHUB_CONNECTIONSTRING";

            var jsonInput = (JObject)JsonConvert.DeserializeObject(myQueueItem);
            var deviceName=((JValue)jsonInput["DeviceName"]).ToString();
            var temperature = ((JValue)jsonInput["temperature"]).ToString();
            dynamic jsonOutput = new JObject();
            jsonOutput.Add("DeviceName", deviceName);
            jsonOutput.Add("temperature", temperature);
            string jsonOutputString = jsonOutput.ToString(Formatting.None);

            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            var serviceMessage = new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(jsonOutputString));
            await serviceClient.SendAsync(deviceName, serviceMessage);
        }
    }
}
