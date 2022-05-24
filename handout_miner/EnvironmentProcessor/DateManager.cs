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

    public static class DateManager
    {
        public static string Partition { get => "handoutminer"; }
        public static string BannedDateTableName { get => "BannedDates"; }
        public static string DateAnnotationTableName { get => "DateAnnotations"; }

        static List<DateTime> _bannedDates = new List<DateTime>();
        static Dictionary<DateTime, string> _dateAnnotations = new Dictionary<DateTime, string>();

        private static void LoadBannedDates()
        {
            _bannedDates.Clear();
            _bannedDates.Add(DateTime.Parse("may 01, 1924"));
        }

        private static void LoadDateAnnotations()
        {
            _dateAnnotations.Clear();
            AddAnnotation("march 03, 1919", "Letter sent to Mr. Carlyle informes him that Faraz Najar has items on Egypt's past.");
            AddAnnotation("april 05, 1919", "Carlyle expedition leaves for London");
            AddAnnotation("july 04, 1919", "Carlyle Expedition leaves Cairo for a 'Safari'");
            AddAnnotation("july 31, 1919", "Carlyle expedition arrives in Mombasa Kenya");
            AddAnnotation("august 03, 1919", "Carlyle expedition leaves Nairobi for a 'Safari'");
            AddAnnotation("october 15, 1919", "Carlyle expedition reported missing in Nairobi by police");
            AddAnnotation("march 11, 1920", "Erica Carlyle arrives in Mombasa, Kenya to investigate dissappearance of her brother");
            AddAnnotation("may 24, 1920", "Massacre of Carlyle expedition reported by police");
            AddAnnotation("june 19, 1920", "Five Nandi tribesman hanged for Carlyle massacre");
            AddAnnotation("august 08, 1924", "Elias Jackson sends his publisher Jonah news that some members of Carlyle expedition survived");
            AddAnnotation("november 07, 1924", "Miriam Artwright invites Elias Jackson to investigate Harvard's library for information");
        }
        private static void AddAnnotation(string date, string annotation)
        {
            _dateAnnotations.Add(DateTime.Parse(date), annotation);
        }

        public static async Task ClearDateStorage()
        {
            AzureConfig config = new AzureConfig();
            TableClient _bannedClient = new TableClient(config.storage_connection_string, BannedDateTableName);
            TableClient _annotationsClient = new TableClient(config.storage_connection_string, DateAnnotationTableName);
            try
            {
                Pageable<TableEntity> oDataQueryEntities = _bannedClient.Query<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {Partition}"));
                foreach (TableEntity entity in oDataQueryEntities)
                {
                    await _bannedClient.DeleteEntityAsync(Partition, entity.GetString("RowKey"));
                }
            }
            catch (System.Exception) { }

            try
            {
                Pageable<TableEntity> oDataQueryEntitiesTwo = _annotationsClient.Query<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {Partition}"));
                foreach (TableEntity entity in oDataQueryEntitiesTwo)
                {
                    await _annotationsClient.DeleteEntityAsync(Partition, entity.GetString("RowKey"));
                }
            }
            catch (System.Exception) { }
        }

        public static async Task CreateDateStorage()
        {
            AzureConfig config = new AzureConfig();
            TableClient _bannedClient = new TableClient(config.storage_connection_string, BannedDateTableName);
            TableClient _annotationsClient = new TableClient(config.storage_connection_string, DateAnnotationTableName);
            await _bannedClient.CreateIfNotExistsAsync();
            await _annotationsClient.CreateIfNotExistsAsync();
        }

        public static async Task UploadDateStorage()
        {
            AzureConfig config = new AzureConfig();
            TableClient _bannedClient = new TableClient(config.storage_connection_string, BannedDateTableName);
            TableClient _annotationsClient = new TableClient(config.storage_connection_string, DateAnnotationTableName);
            LoadBannedDates();
            LoadDateAnnotations();
            foreach (System.DateTime dt in _bannedDates)
            {
                SimpleStringEntity e = new SimpleStringEntity()
                {
                    RowKey = HttpUtility.UrlEncode(DateString(dt)),
                    PartitionKey = Partition
                };
                Console.WriteLine("Adding banned date:" + e.RowKey);
                await _bannedClient.AddEntityAsync(e);
            }
            foreach (DateTime dt in _dateAnnotations.Keys)
            {
                StringKeyValueEntity e = new StringKeyValueEntity()
                {
                    PartitionKey = Partition,
                    RowKey = HttpUtility.UrlEncode(DateString(dt)),
                    Value = HttpUtility.UrlEncode(_dateAnnotations[dt])
                };
                Console.WriteLine("Adding date annotation:" + e.RowKey);
                await _annotationsClient.AddEntityAsync(e);
            }

        }
        private static string DateString(System.DateTime dt) => dt.ToString("MMMM d, yyyy");

    }
}
