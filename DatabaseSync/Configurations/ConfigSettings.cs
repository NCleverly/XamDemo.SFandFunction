using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseSync.Configurations
{
    public class ConfigurationSettings
    {
        //private static StatelessServiceContext _Context = null;
        private static string _authKey = null;
        private static string _databaseId = null;
        private static string _collectionId = null;
        private static string _endpoint = null;

        public static void Parse(StatelessServiceContext context)
        {
            AuthKey = context.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings.Sections["Data"].Parameters["AuthKey"].Value;
            CollectionId = context.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings.Sections["Data"].Parameters["CollectionId"].Value;
            DatabaseId = context.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings.Sections["Data"].Parameters["DatabaseId"].Value;
            Endpoint = context.CodePackageActivationContext.GetConfigurationPackageObject("Config").Settings.Sections["Data"].Parameters["Endpoint"].Value;
        }

        public static string AuthKey
        {
            get
            {
                return _authKey;
            }
            set
            {
                _authKey = value;
            }
        }
        public static string CollectionId
        {
            get
            {
                return _collectionId;
            }
            set
            {
                _collectionId = value;
            }
        }


        public static string DatabaseId
        {
            get
            {
                return _databaseId;
            }
            set
            {
                _databaseId = value;
            }
        }

        public static string Endpoint
        {
            get
            {
                return _endpoint;
            }
            set
            {
                _endpoint = value;
            }
        }


    }
}
