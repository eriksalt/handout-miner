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

        protected override void EnterBans()
        {
            AddBan("carlyle expeditims");
            AddBan("carlyle expedition");
            AddBan("carlyle massacre");
            AddBan("guillermo e");
            AddBan("guillermo e. billinghurst");
            AddBan("jackson america");
            AddBan("rebecca shosenburg");
            AddBan("the cult");
            AddBan("tottenham");
            AddBan("the stones");
        }

        protected override void EnterChanges()
        {
            AddChange("elias jackson", "jackson elias");
            AddChange("nemesio sanchez", "memesio sanchez");
            AddChange("robert our huston", "robert huston");
            AddChange("roger cardigs", "roger carlyle");
            AddChange("silas n kwane", "silas n'kwane");
            AddChange("prospero house carlton ramsey", "carlton ramsey");
            AddChange("roger robert huston", "robert huston");
        }
    }
}
