using HandoutMiner.Shared;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace handout_miner_skills
{
    internal static class AdjustmentNames
    {
        public static string Dates = "dates";
        public static string Locations = "locations";
        public static string People = "people";
        public static string Phrases = "phrases";
    }
    internal class AdjustmentManagers
    {
        public Dictionary<string, List<string>> Bans = new();
        public Dictionary<string, List<(string, string)>> Changes= new();
        public List<(string, string)> DateAnnotations = new();
        private string _connection_string = default!;
        private bool _initialized = false;
        public AdjustmentManagers()
        {
        }

        public void Initialize(ILogger log)
        {
            if (!_initialized)
            {
                _connection_string = Environment.GetEnvironmentVariable("handout_miner_storage_connection_string");
                if (string.IsNullOrWhiteSpace(_connection_string))
                {
                    log.LogInformation("Connection String not found!!!!!!!!!!!!!!!");
                }
                log.LogInformation($"Initializing Adjusment Managers.  Conn String Length is {_connection_string.Length}");
                DateManager dateManager = new(_connection_string);
                LocationManager locationManager = new(_connection_string);
                PeopleManager peopleManager = new(_connection_string);
                PhraseManager phraseManager = new(_connection_string);

                Process(dateManager, AdjustmentNames.Dates, log);
                Process(locationManager, AdjustmentNames.Locations, log);
                Process(peopleManager, AdjustmentNames.People, log);
                Process(phraseManager, AdjustmentNames.Phrases, log);
                _initialized = true;
            }
        }
        
        private void Process(EntityManagerBase mgr, string name, ILogger log)
        {
            log.LogInformation($"Getting bans for {name}.");
            Bans.Add(name, new List<string>());
            log.LogInformation("adding bans to manager");
            Bans[name].AddRange(mgr.GetBansFromStore());
            log.LogInformation($"Getting changes for {name}.");
            Changes.Add(name, new List<(string, string)>());
            log.LogInformation("adding changes to manager");
            Changes[name].AddRange(mgr.GetChangesFromStore());
        }
    }
}
