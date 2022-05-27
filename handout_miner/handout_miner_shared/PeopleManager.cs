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
                
                return changes;
            }
        }
        protected override List<string> Bans
        {
            get
            {
                List<string> bans = new List<string>();
               
                return bans;
            }
        }
    }
}
