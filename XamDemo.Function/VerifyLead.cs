
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using XamDemo.Function.Models;

namespace XamDemo.Function
{
    public static class VerifyLead
    {
        [FunctionName("VerifyLead")]
        public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            LeadData data = JsonConvert.DeserializeObject<LeadData>(requestBody);


            if (string.IsNullOrWhiteSpace(data.FirstName))
                return (ActionResult)new OkObjectResult($"First name not provided");
            if (string.IsNullOrWhiteSpace(data.LastName))
                return (ActionResult)new OkObjectResult($"Last name not provided");
            if (string.IsNullOrWhiteSpace(data.PhoneNumber))
                return (ActionResult)new OkObjectResult($"Phone number not provided");
            if (string.IsNullOrWhiteSpace(data.Email))
                return (ActionResult)new OkObjectResult($"Email not provided");

            return (ActionResult)new OkObjectResult($"Model is valid");

        }
    }
}
