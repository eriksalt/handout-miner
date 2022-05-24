using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EnvironmentProcessor
{
    public class SimpleStringEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

    public class StringKeyValueEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Value { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }


    public class LocationManager
    {

        public static string Partition { get => "handoutminer"; }
        public static string BannedLocationsTableName { get => "BannedLocations"; }
        public static string LocationChangesTableName { get => "LocationChanges"; }

        public static Dictionary<string, string> LocationReplacements
        {
            get
            {
                Dictionary<string, string> changes = new Dictionary<string, string>();

                changes.Add("aberdare", "aberdare national park, kenya");
                changes.Add("aberdare forest", "aberdare national park, kenya");
                changes.Add("cambridge", "cambridge, massachusetts");
                changes.Add("collingswood", "collingswood house");
                changes.Add("el peru", "peru");
                changes.Add("hotel chelsea", "chelsea hotel,new york");
                changes.Add("chelsea hotel", "chelsea hotel, new york");
                changes.Add("manhattan, n.y", "manhtattan");
                changes.Add("ny", "new york city");
                changes.Add("n.y", "new york city");
                changes.Add("n.y.", "new york city");
                changes.Add("nyc", "new york city");
                changes.Add("n.y.c.", "new york city");
                changes.Add("new york", "new york city");
                changes.Add("new york, new york", "new york city");
                changes.Add("nairobi dear janak", "nairobi");
                changes.Add("stratford", "stratford, connecticut");
                changes.Add("the united kingdom", "united kingdom");
                changes.Add("arkham", "essex county, massachusetts");
                changes.Add("648 west 47th street", "648 west 47th street, ny");
                changes.Add("nile", "nile river");

                changes.Add("southwest pacific", "pacific ocean");
                changes.Add("collingswood house", "mombasa");
                changes.Add("schuyler hall", "new york university");
                changes.Add("distrito de lima", "lima");
                changes.Add("lantern street", "shanghai old street");
                return changes;
            }
        }
        public static List<string> BannedLocations
        {
            get
            {
                List<string> locations = new List<string>();
                locations.Add("abyssinian");
                locations.Add("usa");
                locations.Add("east africa");
                locations.Add("american");
                locations.Add("africa");
                locations.Add("ancash");
                locations.Add("are");
                locations.Add("bolivian");
                locations.Add("british");
                locations.Add("cee");
                locations.Add("central american");
                locations.Add("colony");
                locations.Add("de");
                locations.Add("egypt's");
                locations.Add("egyptian");
                locations.Add("endicott");
                locations.Add("english");
                locations.Add("english counties");
                locations.Add("forest");
                locations.Add("front");
                locations.Add("ht");
                locations.Add("imperial");
                locations.Add("in");
                locations.Add("italians");
                locations.Add("kenyan");
                locations.Add("locksley");
                locations.Add("many");
                locations.Add("miss");
                locations.Add("new york's");
                locations.Add("nm");
                locations.Add("no");
                locations.Add("old quarter");
                locations.Add("or");
                locations.Add("pacific");
                locations.Add("peruvian");
                locations.Add("pillar");
                locations.Add("poole");
                locations.Add("schuyler");
                locations.Add("sent");
                locations.Add("somali");
                locations.Add("southampton");
                locations.Add("the astoria");
                locations.Add("to");
                locations.Add("victoria bar");
                locations.Add("kensington");
                return locations;
            }
        }

        public static async Task ClearLocationStore()
        {
            AzureConfig config = new AzureConfig();
            TableClient _bannedClient = new TableClient(config.storage_connection_string, BannedLocationsTableName);
            TableClient _changesClient = new TableClient(config.storage_connection_string, LocationChangesTableName);
            try {
                Pageable<TableEntity> oDataQueryEntities = _bannedClient.Query<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {Partition}"));
                foreach (TableEntity entity in oDataQueryEntities)
                {
                    await _bannedClient.DeleteEntityAsync(Partition, entity.GetString("RowKey"));
                }
            }
            catch (System.Exception) { }

            try
            {
                Pageable<TableEntity> oDataQueryEntitiesTwo = _changesClient.Query<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {Partition}"));
                foreach (TableEntity entity in oDataQueryEntitiesTwo)
                {
                    await _changesClient.DeleteEntityAsync(Partition, entity.GetString("RowKey"));
                }
            }
            catch (System.Exception){ }
        }

        public static async Task CreateLocationStore()
        {
            AzureConfig config = new AzureConfig();
            TableClient _bannedClient = new TableClient(config.storage_connection_string, BannedLocationsTableName);
            TableClient _changesClient = new TableClient(config.storage_connection_string, LocationChangesTableName);
            await _bannedClient.CreateIfNotExistsAsync();
            await _changesClient.CreateIfNotExistsAsync();
        }

        public static async Task SetupBannedLocations()
        {
            AzureConfig config = new AzureConfig();
            TableClient _bannedClient = new TableClient(config.storage_connection_string, BannedLocationsTableName);
            foreach (string loc in BannedLocations)
            {
                SimpleStringEntity e = new SimpleStringEntity()
                {
                    RowKey = HttpUtility.UrlEncode(loc),
                    PartitionKey = Partition
                };
                Console.WriteLine("Adding banned location:" + e.RowKey);
                await _bannedClient.AddEntityAsync(e);
            }
        }

        public static async Task SetupLocationChanges()
        {
            AzureConfig config = new AzureConfig();
            TableClient _changesClient = new TableClient(config.storage_connection_string, LocationChangesTableName);
            Dictionary<string, string> changes = LocationReplacements;
            foreach (string key in changes.Keys)
            {
                StringKeyValueEntity e = new StringKeyValueEntity()
                {
                    PartitionKey = Partition,
                    RowKey = HttpUtility.UrlEncode(key),
                    Value = HttpUtility.UrlEncode(changes[key])
                };
                Console.WriteLine("Adding location change:" + e.RowKey);
                await _changesClient.AddEntityAsync(e);
            }
        }


    }
}
