using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatabaseSync.Configurations;
using DatabaseSync.DataObjects;
using DatabaseSync.Models;
using DatabaseSync.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DatabaseSync.Controllers
{
    [Route("api/LeadSave")]
    [ApiController]
    public class LeadSaveController : ControllerBase
    {
        private ILogger _logger;
        private IConfiguration _configuration;
        private DocumentDbRepository<EnrichedLead> _repo;
        private static DocDbConfig _config;

        public LeadSaveController(IConfiguration Configuration)
        {
            _configuration = Configuration;
            _logger = new LoggerFactory().CreateLogger(ServiceEventSource.Current.GetType());
            _config = new DocDbConfig
            {
                AuthKey = ConfigurationSettings.AuthKey,
                CollectionId = ConfigurationSettings.CollectionId,
                DatabaseId = ConfigurationSettings.DatabaseId,
                Endpoint = ConfigurationSettings.Endpoint
            };
            _repo = new DocumentDbRepository<EnrichedLead>();
            _repo.Initialize(_config);
        }

        [HttpPost]
        public async Task<IActionResult> SaveLeadAsync(LeadData lead)
        {
            await SaveDataToDb(lead);
            return  new ContentResult { Content= lead.ToString() };
        }

        private async Task SaveDataToDb(LeadData lead)
        {
            //todo: Create DTOs and populate with data
            EnrichedLead crmLead = new EnrichedLead
            {
                City = lead.City,
                CurrentAgent = lead.CurrentAgent,
                Email = lead.Email,
                FirstName = lead.FirstName,
                FreeEvent = lead.FreeEvent,
                FreeProduct = lead.FreeProduct,
                LastName = lead.LastName,
                PhoneNumber = lead.PhoneNumber,
                SalesCareer = lead.SalesCareer,
                State = lead.State,
                StreetAddress = lead.StreetAddress,
                TriedProduct = lead.TriedProduct,
                Zip = lead.Zip
            };
            //todo: Send the dtos to the CRM to process(here just saved to DB)
           var doc =  await _repo.CreateItemAsync(crmLead);
        }
    }
}