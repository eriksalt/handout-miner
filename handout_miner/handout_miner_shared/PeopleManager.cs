using System;
using System.Collections.Generic;
using System.Text;

namespace HandoutMiner.Shared
{
    public class PeopleManager : EntityManagerBase
    {
        public PeopleManager(string connection_string) : base(connection_string, "people", "BannedPeople", "PeopleChanges")
        {
        }

        protected override Dictionary<string, string> Changes
        {
            get
            {
                Dictionary<string, string> changes = new Dictionary<string, string>();
                changes.Add("elias jackson", "jackson elias");
                changes.Add("nemesio sanchez", "memesio sanchez");
                changes.Add("robert our huston", "robert huston");
                changes.Add("roger cardigs", "roger carlyle");
                changes.Add("silas n kwane", "silas n'kwane");
                return changes;
            }
        }
        protected override List<string> Bans
        {
            get
            {
                List<string> bans = new List<string>();
                bans.Add("carlyle expeditims");
                bans.Add("carlyle expedition");
                bans.Add("carlyle massacre");
                bans.Add("guillermo e");
                bans.Add("guillermo e. billinghurst");
                bans.Add("jackson america");
                bans.Add("rebecca shosenburg");
                bans.Add("the cult");
                bans.Add("tottenham");
                return bans;
            }
        }
    }
}
