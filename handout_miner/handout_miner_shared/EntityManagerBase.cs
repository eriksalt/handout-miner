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

    public abstract class EntityManagerBase
    {
        
        protected string Partition { get => "handoutminer"; }
        protected string BansTableName { get; set; } = "BannedPeople"; 
        protected string ChangesTableName { get; set; }  = "PeopleChanges";
        protected string EntityType { get; set; } = "People";
        protected string Connection_String { get; set; } = string.Empty;

        protected HashSet<string> Bans { get; } = new HashSet<string>();
        protected Dictionary<string, string> Changes = new Dictionary<string, string>();

        protected EntityManagerBase() { }
        protected EntityManagerBase(string connection_string, string entity_type, string bans_table_name, string changes_table_name)
        {
            Connection_String = connection_string;
            EntityType = entity_type;
            BansTableName = bans_table_name;
            ChangesTableName = changes_table_name;
            EnterBans();
            EnterChanges();
        }

        protected abstract void EnterBans();
        protected abstract void EnterChanges();

        protected void AddBan(string ban)
        {
            string tmp = ban.ToLower().Trim();
            if (!Bans.Contains(tmp)) 
                Bans.Add(tmp);
        }

        protected void AddChange(string original, string change)
        {
            string key = original.ToLower().Trim();
            string val = change.ToLower().Trim();
            if(!Changes.ContainsKey(key)) 
                Changes.Add(key, val);
        }

        public virtual async Task ClearLocationStore()
        {
            TableClient _bannedClient = new TableClient(Connection_String, BansTableName);
            TableClient _changesClient = new TableClient(Connection_String, ChangesTableName);
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
                Pageable<TableEntity> oDataQueryEntitiesTwo = _changesClient.Query<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {Partition}"));
                foreach (TableEntity entity in oDataQueryEntitiesTwo)
                {
                    await _changesClient.DeleteEntityAsync(Partition, entity.GetString("RowKey"));
                }
            }
            catch (System.Exception) { }
        }

        public virtual async Task CreateLocationStore()
        {
            TableClient _bannedClient = new TableClient(Connection_String, BansTableName);
            TableClient _changesClient = new TableClient(Connection_String, ChangesTableName);
            await _bannedClient.CreateIfNotExistsAsync();
            await _changesClient.CreateIfNotExistsAsync();
        }

        protected virtual async Task SetupBans()
        {
            TableClient _bannedClient = new TableClient(Connection_String, BansTableName);
            foreach (string loc in Bans)
            {
                SimpleStringEntity e = new SimpleStringEntity()
                {
                    RowKey = HttpUtility.UrlEncode(loc),
                    PartitionKey = Partition
                };
                Console.WriteLine("Adding ban:" + e.RowKey);
                await _bannedClient.AddEntityAsync(e);
            }
        }

        protected virtual async Task SetupChanges()
        {
        
            TableClient _changesClient = new TableClient(Connection_String, ChangesTableName);
            Dictionary<string, string> changes = Changes;
            foreach (string key in changes.Keys)
            {
                StringKeyValueEntity e = new StringKeyValueEntity()
                {
                    PartitionKey = Partition,
                    RowKey = HttpUtility.UrlEncode(key),
                    Value = HttpUtility.UrlEncode(changes[key])
                };
                Console.WriteLine("Adding change:" + e.RowKey);
                await _changesClient.AddEntityAsync(e);
            }
        }

        public virtual async Task LoadDataIntoStore()
        {
            await SetupChanges();
            await SetupBans();
        }

        public IEnumerable<string> GetBansFromStore()
        {
            TableClient _bannedClient = new TableClient(Connection_String, BansTableName);
            Pageable<TableEntity> oDataQueryEntities = _bannedClient.Query<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {Partition}"));
            foreach (TableEntity entity in oDataQueryEntities)
            {
                yield return HttpUtility.UrlDecode(entity.GetString("RowKey"));
            }
        }

        public IEnumerable<(string,string)>GetChangesFromStore()
        {
            TableClient _bannedClient = new TableClient(Connection_String, ChangesTableName);
            Pageable<TableEntity> oDataQueryEntities = _bannedClient.Query<TableEntity>(filter: TableClient.CreateQueryFilter($"PartitionKey eq {Partition}"));
            foreach (TableEntity entity in oDataQueryEntities)
            {
                yield return (HttpUtility.UrlDecode(entity.GetString("RowKey")), HttpUtility.UrlDecode(entity.GetString("Value")));
            }
        }

    }
}
