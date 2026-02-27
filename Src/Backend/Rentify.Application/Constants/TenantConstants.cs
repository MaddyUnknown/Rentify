using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rentify.Application.Constants
{
    public static class TenantConstants
    {
        public static readonly string TenantsOutOfPageError = "Total pages '{0}' but received request for page number '{1}'";
        public static readonly string TenantNotFound = "Tenant not found for id '{0}'";
        public static readonly string TenantEmergencyDetailsNotFound = "Tenant emergency details not found for id '{0}'";
    }
}
