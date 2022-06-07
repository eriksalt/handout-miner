using System;
using System.Collections.Generic;
using System.Text;

namespace HandoutMiner.Shared
{
    public class LocationManager : EntityManagerBase
    {
        public LocationManager(string connection_string) : base(connection_string, "location", "BannedLocations", "LocationChanges")
        {
        }

        protected override Dictionary<string, string> Changes
        {
            get
            {
                Dictionary<string, string> changes = new Dictionary<string, string>();
                changes.Add("10 lantern street", "shanghai, china");
                changes.Add("648 west 47th street", "648 west 47th street, ny");
                changes.Add("aberdare forest", "aberdare national park, kenya");
                changes.Add("aberdare", "aberdare national park, kenya");
                changes.Add("arkham", "essex county, massachusetts");
                changes.Add("cairo", "cairo, egypt");
                changes.Add("cambridge", "cambridge, massachusetts");
                changes.Add("chelsea hotel", "chelsea hotel, new york");
                changes.Add("collingswood house", "mombasa");
                changes.Add("collingswood", "mombosa");
                changes.Add("distrito de lima", "lima");
                changes.Add("egypt cairo", "cairo, egypt");
                changes.Add("el peru", "peru");
                changes.Add("hotel chelsea", "chelsea hotel, new york");
                changes.Add("lantern street", "shanghai old street");
                changes.Add("lima", "lima, peru");
                changes.Add("manhattan, n.y", "manhtattan");
                changes.Add("manhtattan", "manhattan");
                changes.Add("n.y", "new york city");
                changes.Add("n.y.", "new york city");
                changes.Add("n.y.c.", "new york city");
                changes.Add("nairobi dear janak", "nairobi");
                changes.Add("nairobi dear jawak", "nairobi");
                changes.Add("new york", "new york city");
                changes.Add("new york, new york", "new york city");
                changes.Add("nile", "nile river");
                changes.Add("ny", "new york city");
                changes.Add("nyc", "new york city");
                changes.Add("old quarter", "old quarter, cairo");
                changes.Add("puno", "puno, peru");
                changes.Add("puno,peru", "puno, peru");
                changes.Add("schuyler hall", "new york university");
                changes.Add("shanghai", "shanghai, china");
                changes.Add("southwest pacific", "pacific ocean");
                changes.Add("stratford", "stratford, connecticut");
                changes.Add("the united kingdom", "united kingdom");
                changes.Add("tottenham", "tottenham court road");
                return changes;
            }
        }
        protected override List<string> Bans
        {
            get
            {
                List<string> bans = new List<string>();
                bans.Add("abyssinian");
                bans.Add("africa");
                bans.Add("african-american");
                bans.Add("america");
                bans.Add("american");
                bans.Add("ancash");
                bans.Add("andean highlands");
                bans.Add("andean");
                bans.Add("are");
                bans.Add("bolivian");
                bans.Add("british");
                bans.Add("cee");
                bans.Add("central american");
                bans.Add("colony");
                bans.Add("de");
                bans.Add("east africa");
                bans.Add("egypt's");
                bans.Add("egyptian");
                bans.Add("endicott");
                bans.Add("english counties");
                bans.Add("english");
                bans.Add("forest");
                bans.Add("front");
                bans.Add("ht");
                bans.Add("imperial");
                bans.Add("in");
                bans.Add("italians");
                bans.Add("kensington");
                bans.Add("kenyan");
                bans.Add("locksley");
                bans.Add("many");
                bans.Add("miss");
                bans.Add("new york's");
                bans.Add("nm");
                bans.Add("no");
                bans.Add("or");
                bans.Add("orh");
                bans.Add("pacific");
                bans.Add("peruvian");
                bans.Add("pillar");
                bans.Add("poole");
                bans.Add("schuyler");
                bans.Add("sent");
                bans.Add("somali");
                bans.Add("southampton");
                bans.Add("street");
                bans.Add("stumbling tiger bar");
                bans.Add("the americas");
                bans.Add("the astoria");
                bans.Add("to");
                bans.Add("trinidad riso");
                bans.Add("trinidad");
                bans.Add("usa");
                bans.Add("victoria bar");

                return bans;
            }
        }
    }
}
