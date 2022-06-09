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

        protected override void EnterBans()
        {
            AddBan("abyssinian");
            AddBan("africa");
            AddBan("african-american");
            AddBan("america");
            AddBan("american");
            AddBan("ancash");
            AddBan("andean highlands");
            AddBan("andean");
            AddBan("are");
            AddBan("bolivian");
            AddBan("british");
            AddBan("cee");
            AddBan("central american");
            AddBan("colony");
            AddBan("de");
            AddBan("east africa");
            AddBan("egypt's");
            AddBan("egyptian");
            AddBan("endicott");
            AddBan("english counties");
            AddBan("english");
            AddBan("forest");
            AddBan("front");
            AddBan("ht");
            AddBan("imperial");
            AddBan("in");
            AddBan("italians");
            AddBan("kensington");
            AddBan("kenyan");
            AddBan("locksley");
            AddBan("many");
            AddBan("miss");
            AddBan("new york's");
            AddBan("nm");
            AddBan("no");
            AddBan("or");
            AddBan("orh");
            AddBan("pacific");
            AddBan("peruvian");
            AddBan("pillar");
            AddBan("poole");
            AddBan("schuyler");
            AddBan("sent");
            AddBan("somali");
            AddBan("southampton");
            AddBan("street");
            AddBan("stumbling tiger bar");
            AddBan("the americas");
            AddBan("the astoria");
            AddBan("to");
            AddBan("trinidad riso");
            AddBan("trinidad");
            AddBan("usa");
            AddBan("victoria bar");
        }

        protected override void EnterChanges()
        {
            AddChange("10 lantern street", "shanghai, china");
            AddChange("648 west 47th street", "648 west 47th street, ny");
            AddChange("aberdare forest", "aberdare national park, kenya");
            AddChange("aberdare", "aberdare national park, kenya");
            AddChange("arkham", "essex county, massachusetts");
            AddChange("cairo", "cairo, egypt");
            AddChange("cambridge", "cambridge, massachusetts");
            AddChange("chelsea hotel", "chelsea hotel, new york");
            AddChange("collingswood house", "mombasa");
            AddChange("collingswood", "mombosa");
            AddChange("distrito de lima", "lima");
            AddChange("egypt cairo", "cairo, egypt");
            AddChange("el peru", "peru");
            AddChange("hotel chelsea", "chelsea hotel, new york");
            AddChange("lantern street", "shanghai old street");
            AddChange("lima", "lima, peru");
            AddChange("manhattan, n.y", "manhtattan");
            AddChange("manhtattan", "manhattan");
            AddChange("n.y", "new york city");
            AddChange("n.y.", "new york city");
            AddChange("n.y.c.", "new york city");
            AddChange("nairobi dear janak", "nairobi");
            AddChange("nairobi dear jawak", "nairobi");
            AddChange("new york", "new york city");
            AddChange("new york, new york", "new york city");
            AddChange("nile", "nile river");
            AddChange("ny", "new york city");
            AddChange("nyc", "new york city");
            AddChange("old quarter", "old quarter, cairo");
            AddChange("puno", "puno, peru");
            AddChange("puno,peru", "puno, peru");
            AddChange("schuyler hall", "new york university");
            AddChange("shanghai", "shanghai, china");
            AddChange("southwest pacific", "pacific ocean");
            AddChange("stratford", "stratford, connecticut");
            AddChange("the united kingdom", "united kingdom");
            AddChange("tottenham", "tottenham court road"); 
        }

    }
}
