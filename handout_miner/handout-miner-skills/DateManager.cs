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
    public class DateManager
    {
        private static DateManager _instance = null;
        public static DateManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DateManager();
                return _instance;
            }
        }

        private string ConnectionString { get; set; } = String.Empty;
        private TableClient _bannedClient { get; set; } = default!;
        public static string Partition { get => "handoutminer"; }
        public static string BannedDatesTableName { get => "BannedDates"; }

        public HashSet<string> BannedDates { get; } = new HashSet<string>();
        public DateManager()
        {
            ConnectionString = Environment.GetEnvironmentVariable("handout_miner_storage_connection_string");
            //ConnectionString = System.Configuration.ConfigurationManager.AppSettings["handout_miner_storage_connection_string"];
            _bannedClient = new TableClient(ConnectionString, BannedDatesTableName);

            Pageable<TableEntity> oDataQueryEntities = _bannedClient.Query<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {Partition}"));
            foreach (TableEntity entity in oDataQueryEntities)
            {
                BannedDates.Add(HttpUtility.UrlDecode(entity.GetString("RowKey")));
            }
        }
    }
}
