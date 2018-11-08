using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseSync.DataObjects
{
    public class EnrichedLead
    {
        public string FirstName;
        public string LastName;
        public string PhoneNumber;
        public string Email;
        public string StreetAddress;
        public string City;
        public string State;
        public string Zip;
        public bool TriedProduct;
        public bool CurrentAgent;
        public bool FreeProduct;
        public bool FreeEvent;
        public bool SalesCareer;
        public int LeadScore => GetLeadScore();

        private int GetLeadScore()
        {
            int score = 10;
            score += TriedProduct ? 10 : 0;

            return score;
        }
    }
}
