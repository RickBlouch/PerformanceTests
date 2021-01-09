using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityKeyBuilder.Application
{
    public class IdentityKeyProperties
    {
        public string AccountNumber { get; private set; }
        public string SystemCode { get; private set; }
        public string ExternalId { get; private set; }
        public DateTime? ServiceDate { get; private set; }

        public IdentityKeyProperties(string accountNumber, string systemCode, string externalId, DateTime? serviceDate)
        {
            AccountNumber = accountNumber;
            SystemCode = systemCode;
            ExternalId = externalId;
            ServiceDate = serviceDate;
        }
    }
}
