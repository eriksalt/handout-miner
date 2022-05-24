using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace handout_miner_skills
{
    public  class LocationManager
    {
        private static LocationManager _instance = null;
        public static LocationManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LocationManager();
                return _instance;
            }
        }

        private string ConnectionString { get; set; } = String.Empty;
        private TableClient _bannedClient { get; set; } = default!;
        private TableClient _changesClient { get; set; } = default!;
        public static string Partition { get => "handoutminer"; }
        public static string BannedLocationsTableName { get => "BannedLocations"; }
        public static string LocationChangesTableName { get => "LocationChanges"; }

        public  HashSet<string> BannedLocations { get; } = new HashSet<string>();
        public  Dictionary<string,string> LocationChanges { get; } = new Dictionary<string, string>();
        public LocationManager()
        {
            ConnectionString = Environment.GetEnvironmentVariable("handout_miner_storage_connection_string");
            //ConnectionString = System.Configuration.ConfigurationManager.AppSettings["handout_miner_storage_connection_string"];
             _bannedClient = new TableClient(ConnectionString, BannedLocationsTableName);
            _changesClient = new TableClient(ConnectionString, LocationChangesTableName);

            Pageable<TableEntity> oDataQueryEntities = _bannedClient.Query<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {Partition}"));
            foreach (TableEntity entity in oDataQueryEntities)
            {
                BannedLocations.Add(HttpUtility.UrlDecode(entity.GetString("RowKey")));
            }

            Pageable<TableEntity> oDataQueryEntitiesTwo = _changesClient.Query<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {Partition}"));
            foreach (TableEntity entity in oDataQueryEntitiesTwo)
            {
                LocationChanges.Add(HttpUtility.UrlDecode(entity.GetString("RowKey")), HttpUtility.UrlDecode(entity.GetString("Value")));
            }

        }


    }
}
