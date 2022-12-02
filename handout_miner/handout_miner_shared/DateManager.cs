using Azure;
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HandoutMiner.Shared
{

    public class DateManager : EntityManagerBase
    {
        private readonly string DateAnnotationsTableName = "DateAnnotations";
        public DateManager(string connection_string) : base(connection_string, "date", "BannedDates", "DateChanges")
        {
        }

        protected override void EnterChanges()
        {
        }
        protected override void EnterBans()
        {
            AddBan("may 01, 1924");
            AddBan("may 1, 1924");
            AddBan("january 19, 1925");
            AddBan("january 12, 1951");
            AddBan("february 11, 1925");
        }

            
        private Dictionary<DateTime, string> _dateAnnotations = new Dictionary<DateTime, string>();

        private void LoadDateAnnotations()
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
            AddAnnotation("january 16, 1925", "pcs meet with rebecca shosenberg at ny times offices.");
            AddAnnotation("january 17, 1925", "pcs meet erica carlyle at the carlyle estate");
            AddAnnotation("january 18, 1925", "funeral of jackson elias");
            AddAnnotation("january 19, 1925", "reading of the will of jackson elias");
            AddAnnotation("june 5, 1917", "m’weru (nichonka bunay?) enters the united states from kenya");
            AddAnnotation("january 11, 1918", "roger carlyle begins treatment for his dreams from dr. robert huston");
            AddAnnotation("december 03, 1918", "roger carlyle has his last therapy session with dr. robert huston");
            AddAnnotation("november 06, 1924", "dr. h clive sends telegram to penhew foundation that the well dig in giza called mycernus is going well");
            AddAnnotation("february 4, 1925", "Zahra Shafiq contracts Ferris and Sons to transport monthly shipment of spices to Misr House");

        }
        private void AddAnnotation(string date, string annotation)
        {
            if(!_dateAnnotations.ContainsKey(DateTime.Parse(date)))
                _dateAnnotations.Add(DateTime.Parse(date), annotation);
        }

        public async override Task ClearLocationStore()
        {
            
            TableClient _annotationsClient = new TableClient(Connection_String, DateAnnotationsTableName);
            try
            {
                Pageable<TableEntity> oDataQueryEntitiesTwo = _annotationsClient.Query<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {Partition}"));
                foreach (TableEntity entity in oDataQueryEntitiesTwo)
                {
                    await _annotationsClient.DeleteEntityAsync(Partition, entity.GetString("RowKey"));
                }
            }
            catch (System.Exception) { }
            await base.ClearLocationStore();
        }

        public async override Task CreateLocationStore(){
            TableClient _annotationsClient = new TableClient(Connection_String, DateAnnotationsTableName);
            await _annotationsClient.CreateIfNotExistsAsync();
            await base.CreateLocationStore();
        }

        public override async Task LoadDataIntoStore()
        {
            LoadDateAnnotations();
            TableClient _annotationsClient = new TableClient(Connection_String, DateAnnotationsTableName);
            
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
            await base.LoadDataIntoStore();
        }

        public IEnumerable<(string, string)> GetAnnotationsFromStore()
        {
            TableClient _annotationsClient = new TableClient(Connection_String, DateAnnotationsTableName);
            Pageable<TableEntity> oDataQueryEntitiesTwo = _annotationsClient.Query<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {Partition}"));
            foreach (TableEntity entity in oDataQueryEntitiesTwo)
            {
                yield return (HttpUtility.UrlDecode(entity.GetString("RowKey")), HttpUtility.UrlDecode(entity.GetString("Value")));
            }
        }

        private static string DateString(System.DateTime dt) => dt.ToString("MMMM d, yyyy");

    }
}
