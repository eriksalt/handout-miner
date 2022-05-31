using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandoutMiner.Shared;

namespace HandoutMiner
{
    
    internal static class AdjustmentsManager
    {
        private static DateManager DateManager = default!;
        private static LocationManager LocationManager = default!;
        private static PeopleManager PeopleManager = default!;
        private static PhraseManager PhraseManager = default!;
        private static AzureConfig _config = new AzureConfig();
        private static List<EntityManagerBase> Managers = new List<EntityManagerBase>();
        public static void Initialize()
        {
            DateManager = new DateManager(_config.storage_connection_string);
            LocationManager = new LocationManager(_config.storage_connection_string);
            PeopleManager = new PeopleManager(_config.storage_connection_string);
            PhraseManager = new PhraseManager(_config.storage_connection_string);
            Managers.Add(DateManager);
            Managers.Add(LocationManager);
            Managers.Add(PeopleManager);
            Managers.Add(PhraseManager);    
        }

        public static void Clean()
        {
            Managers.ForEach(async (mgr) => await mgr.ClearLocationStore());
        }

        public static void Create()
        {
            Managers.ForEach(async (mgr) => await mgr.CreateLocationStore());
        }

        public static void Upload()
        {
            Managers.ForEach(async (mgr) => await mgr.LoadDataIntoStore());
        }
    }
}
