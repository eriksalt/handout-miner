﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HandoutMiner.Shared
{
    public class PhraseManager : EntityManagerBase
    {
        public PhraseManager(string connection_string) : base(connection_string, "phrase", "BannedPhrases", "PhraseChanges")
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
                HashSet<string> bans = new();
                bans.Add("0403 ssg");
                bans.Add("10 lantern street text");
                bans.Add("10 lantern street");
                bans.Add("24th");
                bans.Add("31st");
                bans.Add("648 west 47th street new york");
                bans.Add("8");
                bans.Add("aberdare forest");
                bans.Add("abstract  tattoo  graffiti");
                bans.Add("accompachaeological expedition");
                bans.Add("africa mombasa");
                bans.Add("africa");
                bans.Add("ally");
                bans.Add("amazon basin");
                bans.Add("american armuch");
                bans.Add("americas");
                bans.Add("arrival");
                bans.Add("art  text");
                bans.Add("aubrey penhew");
                bans.Add("augustus larkin text  sketch  art  drawing");
                bans.Add("augustus larkin");
                bans.Add("authorisies");
                bans.Add("belden e");
                bans.Add("blank world-wide mover ravdys telegraph service fee_ paid kindly");
                bans.Add("book");
                bans.Add("businesscard  screenshot  text");
                bans.Add("cable letter dl");
                bans.Add("cable text");
                bans.Add("cablegram");
                bans.Add("cairo");
                bans.Add("cairo-based portion");
                bans.Add("carlyle expedition stop need reliable investigative team stop meet");
                bans.Add("carlyle");
                bans.Add("cartoon  brick  stone");
                bans.Add("cartoon  illustration  portrait");
                bans.Add("cartoon  illustration");
                bans.Add("cartoon  sketch  text  art  illustration");
                bans.Add("cee cartoon  drawing  text  art");
                bans.Add("child art  portrait  text");
                bans.Add("china");
                bans.Add("close up");
                bans.Add("close");
                bans.Add("clues");
                bans.Add("comment");
                bans.Add("companion");
                bans.Add("companions");
                bans.Add("company");
                bans.Add("conclusive");
                bans.Add("conditions");
                bans.Add("connecticut");
                bans.Add("copy");
                bans.Add("countcarlyle");
                bans.Add("criticismand suggestion");
                bans.Add("dals");
                bans.Add("dar o ship  text  vehicle  boat");
                bans.Add("dear janak");
                bans.Add("dear jawak");
                bans.Add("dear mr. carlyle");
                bans.Add("dear mr. elias");
                bans.Add("deferred cable");
                bans.Add("derot orh alt f");
                bans.Add("detail text  drawing  sketch");
                bans.Add("director text  screenshot  typography  design  graphic  font  text");
                bans.Add("dr. robert huston");
                bans.Add("drawing  monitor  sketch  television");
                bans.Add("drawing  sketch  painting  text  art  animal  cartoon");
                bans.Add("drawing  sketch  painting");
                bans.Add("earth");
                bans.Add("east africa");
                bans.Add("edward gavigan");
                bans.Add("effort rigors");
                bans.Add("egypt");
                bans.Add("el");
                bans.Add("elias jackson");
                bans.Add("elias");
                bans.Add("elias' belief");
                bans.Add("end");
                bans.Add("england");
                bans.Add("episode");
                bans.Add("erica carlyle");
                bans.Add("events");
                bans.Add("expedition stop please");
                bans.Add("explorer augustus larkin");
                bans.Add("fact");
                bans.Add("fair");
                bans.Add("fashioncontrary");
                bans.Add("focus");
                bans.Add("follow");
                bans.Add("fon");
                bans.Add("font  plaque  text");
                bans.Add("four months");
                bans.Add("four other portions");
                bans.Add("friend jackson elias");
                bans.Add("friends");
                bans.Add("friendship");
                bans.Add("full rated teleclt");
                bans.Add("full zate telegrams");
                bans.Add("gaspar figueroa");
                bans.Add("gate");
                bans.Add("george sorel");
                bans.Add("glactle");
                bans.Add("goal");
                bans.Add("good friend");
                bans.Add("great interest");
                bans.Add("great rift valley");
                bans.Add("greatest flaw");
                bans.Add("group");
                bans.Add("habitual grin");
                bans.Add("hand");
                bans.Add("handwriting  letter  calligraphy  typography  script  document  text");
                bans.Add("handwriting  letter  receipt  text");
                bans.Add("handwriting  letter  receipt  typography  document  text");
                bans.Add("handwriting  letter  text  bird  document  text");
                bans.Add("handwriting  screenshot  letter  font  receipt  text");
                bans.Add("handwriting  sketch  stationary  cartoon  envelope  accessory  graffiti");
                bans.Add("handwriting  text  design  graphic  letter  typography");
                bans.Add("handwriting  text  letter  bird  migration  document  text");
                bans.Add("handwriting  text  letter  businesscard  text");
                bans.Add("handwriting  text  letter  document  text");
                bans.Add("help");
                bans.Add("history");
                bans.Add("hong kong");
                bans.Add("hotel chelsea");
                bans.Add("hotel maury stop");
                bans.Add("hudson terminal");
                bans.Add("human face  glasses  text  person  spectacles  goggles");
                bans.Add("human face  illustration  smile  cartoon  face  painting");
                bans.Add("human face  illustration");
                bans.Add("human face  portrait");
                bans.Add("human face  sketch  face");
                bans.Add("human face  statue");
                bans.Add("human face  text  sketch");
                bans.Add("human face  text");
                bans.Add("human face");
                bans.Add("human");
                bans.Add("human, basis");
                bans.Add("hunting");
                bans.Add("hypatia masters");
                bans.Add("important visitors mombasa");
                bans.Add("inadequacy");
                bans.Add("india");
                bans.Add("indoor  art  accessory  illustration");
                bans.Add("indoor  sketch  man  art");
                bans.Add("indoor  text  art  gallery  room  painting  picture");
                bans.Add("information");
                bans.Add("innocents");
                bans.Add("intention");
                bans.Add("interviews");
                bans.Add("investigators");
                bans.Add("italians");
                bans.Add("items");
                bans.Add("jack brady");
                bans.Add("jackson america");
                bans.Add("jan");
                bans.Add("january");
                bans.Add("jesse hughes");
                bans.Add("jirón ancash");
                bans.Add("johnstone kenyatta");
                bans.Add("jonah kensington");
                bans.Add("juicy notes");
                bans.Add("justice");
                bans.Add("key figure");
                bans.Add("knowledge");
                bans.Add("known book");
                bans.Add("la");
                bans.Add("lake titicaca");
                bans.Add("land");
                bans.Add("las harvard cambridge");
                bans.Add("lco");
                bans.Add("lead");
                bans.Add("leader");
                bans.Add("leaders");
                bans.Add("length");
                bans.Add("letter");
                bans.Add("lexington avenue");
                bans.Add("life");
                bans.Add("lima stop augustus larkin");
                bans.Add("lima");
                bans.Add("lludson terminal");
                bans.Add("local resident hilton adams");
                bans.Add("locals");
                bans.Add("london");
                bans.Add("long interview");
                bans.Add("looks");
                bans.Add("lt. mark selkirk");
                bans.Add("lt. martin poole");
                bans.Add("lt. poole");
                bans.Add("lt. selkirk");
                bans.Add("luis de mendoza");
                bans.Add("luis de");
                bans.Add("lyle");
                bans.Add("m. warren besart text  letter  handwriting  typography  calligraphy ");
                bans.Add("screenshot  document  text");
                bans.Add("mainder");
                bans.Add("majority");
                bans.Add("man");
                bans.Add("mange threads");
                bans.Add("manone");
                bans.Add("many bearers");
                bans.Add("many people");
                bans.Add("many skreads");
                bans.Add("massachusetts");
                bans.Add("material");
                bans.Add("matters");
                bans.Add("members");
                bans.Add("mercenary");
                bans.Add("middle");
                bans.Add("mind");
                bans.Add("miriam atwright harvard university library");
                bans.Add("miss carlyle");
                bans.Add("miss erica carlyle");
                bans.Add("miss hypatia masters");
                bans.Add("miss");
                bans.Add("most cases");
                bans.Add("motivatribesmen");
                bans.Add("mous party");
                bans.Add("mr. carlyle");
                bans.Add("mr. elias");
                bans.Add("mr. harvis");
                bans.Add("mr. jack brady");
                bans.Add("mr. jackson elias");
                bans.Add("mr. royston whittingdon");
                bans.Add("mrs. victoria post");
                bans.Add("mwm");
                bans.Add("mychoanalyse files");
                bans.Add("n.y");
                bans.Add("naihave");
                bans.Add("nails\" nelson");
                bans.Add("nairobi");
                bans.Add("name");
                bans.Add("nayra");
                bans.Add("nearby mountain top");
                bans.Add("neville jermyn");
                bans.Add("new direction");
                bans.Add("new murder");
                bans.Add("new story");
                bans.Add("new york city");
                bans.Add("new york pillar/riposte");
                bans.Add("new york socialite hypatia masters");
                bans.Add("new york socialite");
                bans.Add("new york stop jackson elias blank form");
                bans.Add("newspaper  font  receipt  text");
                bans.Add("newspaper  font  receipt");
                bans.Add("next");
                bans.Add("night message wlt");
                bans.Add("notable discoveries");
                bans.Add("note");
                bans.Add("noted");
                bans.Add("notes");
                bans.Add("number");
                bans.Add("offices");
                bans.Add("official version");
                bans.Add("officials");
                bans.Add("old  abandoned  text  decay  map");
                bans.Add("old  art");
                bans.Add("old  painting");
                bans.Add("old  wall  child art");
                bans.Add("old  watercraft  harbor  transport");
                bans.Add("one faraz najjar");
                bans.Add("one meeting");
                bans.Add("one");
                bans.Add("ooks");
                bans.Add("open-\"almost");
                bans.Add("operations");
                bans.Add("opinion");
                bans.Add("other activities");
                bans.Add("other expedition members");
                bans.Add("other members");
                bans.Add("other volumes");
                bans.Add("owner/editor");
                bans.Add("painting");
                bans.Add("parallel researches");
                bans.Add("part");
                bans.Add("past");
                bans.Add("pcs");
                bans.Add("penhew founancient pictographs");
                bans.Add("people");
                bans.Add("pero");
                bans.Add("person  cartoon  couch");
                bans.Add("person  man");
                bans.Add("person  portrait  text");
                bans.Add("person");
                bans.Add("peru description");
                bans.Add("peru explorer");
                bans.Add("peru");
                bans.Add("ph.d");
                bans.Add("photo");
                bans.Add("pick");
                bans.Add("pipe");
                bans.Add("place");
                bans.Add("plains");
                bans.Add("plan");
                bans.Add("plateau");
                bans.Add("point");
                bans.Add("points");
                bans.Add("possibility");
                bans.Add("possible connection");
                bans.Add("possible disappearthe");
                bans.Add("possible structure");
                bans.Add("power");
                bans.Add("preliminary identification expertly-conducted trial");
                bans.Add("presidente");
                bans.Add("prof. anthony cowles");
                bans.Add("prof. memesio sanchez sketch  cartoon  drawing  art  illustration");
                bans.Add("prof. memesio sanchez");
                bans.Add("prof. sanchez");
                bans.Add("professor nemesio sanchez");
                bans.Add("project");
                bans.Add("pseudonym");
                bans.Add("punctual man");
                bans.Add("puno");
                bans.Add("purobi");
                bans.Add("purpose");
                bans.Add("quickest");
                bans.Add("quithere");
                bans.Add("quotes");
                bans.Add("reason");
                bans.Add("rebecca shosenburg manhattan");
                bans.Add("rebecca west");
                bans.Add("receives");
                bans.Add("regard");
                bans.Add("region");
                bans.Add("repere peldeve");
                bans.Add("report");
                bans.Add("reports");
                bans.Add("reptile");
                bans.Add("republica");
                bans.Add("research");
                bans.Add("researcher");
                bans.Add("researches");
                bans.Add("reseveral kikuyu-villager reports");
                bans.Add("respite");
                bans.Add("response");
                bans.Add("reuters");
                bans.Add("reuters)-leading expedition");
                bans.Add("reuters)-uplands egyptologist");
                bans.Add("ringleaders tion");
                bans.Add("riposte");
                bans.Add("rived");
                bans.Add("robert our huston");
                bans.Add("roger carlyle");
                bans.Add("roger corydon");
                bans.Add("royston whittingdon");
                bans.Add("rumor");
                bans.Add("rumors");
                bans.Add("sacrifice");
                bans.Add("safest way");
                bans.Add("sam mariga");
                bans.Add("sandy labors");
                bans.Add("satisfaction");
                bans.Add("scanadditional members");
                bans.Add("se. guillermo e. billinghurst");
                bans.Add("search");
                bans.Add("second half");
                bans.Add("second-hand accounts");
                bans.Add("service class");
                bans.Add("service grain");
                bans.Add("set eight");
                bans.Add("several good sources");
                bans.Add("several languages");
                bans.Add("shanghai fuh");
                bans.Add("shanghai");
                bans.Add("she");
                bans.Add("ship");
                bans.Add("sign");
                bans.Add("silas n kwane");
                bans.Add("silas n'kwane");
                bans.Add("single sheet");
                bans.Add("singular curios");
                bans.Add("sir aubrey penhew police representatives");
                bans.Add("sir aubrey penhew");
                bans.Add("sir");
                bans.Add("site");
                bans.Add("skeptic");
                bans.Add("sketch  art  painting  text  cartoon");
                bans.Add("sketch  drawing  art  cartoon  painting  text");
                bans.Add("sketch  drawing  art  text  painting");
                bans.Add("sketch  drawing  text  book  cartoon  art  illustration");
                bans.Add("sketch  drawing  text  painting  art");
                bans.Add("sketch  tattoo  abstract  art  cartoon");
                bans.Add("sketch  text  cartoon  art");
                bans.Add("sketch  text");
                bans.Add("skinned vicof");
                bans.Add("sky  lake  water  boat  outdoor  watercraft  ship  yellow  vehicle");
                bans.Add("sky  water  outdoor  lake  ship  landscape  boat  watercraft");
                bans.Add("slaughefforts");
                bans.Add("slides");
                bans.Add("smile  face  portrait  woman");
                bans.Add("soinsseitunica exame");
                bans.Add("someone");
                bans.Add("something");
                bans.Add("sons");
                bans.Add("sorts");
                bans.Add("source");
                bans.Add("southwest pacific");
                bans.Add("stance");
                bans.Add("standard time");
                bans.Add("standard");
                bans.Add("stories");
                bans.Add("strange");
                bans.Add("stratford");
                bans.Add("street");
                bans.Add("suitable nl night letter sign");
                bans.Add("surest");
                bans.Add("surrounding site");
                bans.Add("sydney");
                bans.Add("systematization");
                bans.Add("tales");
                bans.Add("tative massacre");
                bans.Add("telegraphy nm");
                bans.Add("tends");
                bans.Add("terms");
                bans.Add("terror");
                bans.Add("text  book  cartoon  art  poster  sketch");
                bans.Add("text  child art  painting");
                bans.Add("text  colorfulness  screenshot  document  text");
                bans.Add("text  design  screenshot  text");
                bans.Add("text  drawing  book");
                bans.Add("text  drawing  sketch  book  cartoon  handwriting  illustration");
                bans.Add("text  font  screenshot  text");
                bans.Add("text  letter  handwriting  screenshot  text");
                bans.Add("text  map  map");
                bans.Add("text  newspaper  screenshot  font  text");
                bans.Add("text  newspaper  screenshot");
                bans.Add("text  newspaper");
                bans.Add("text  painting  statue  cartoon  portrait  spectacles");
                bans.Add("text  screenshot  letter  colorfulness  document  text");
                bans.Add("text  screenshot  letter  document  text");
                bans.Add("text  screenshot  letter  handwriting  receipt  text");
                bans.Add("text  screenshot  receipt  text");
                bans.Add("text  screenshot  typography  font  text");
                bans.Add("text  screenshot");
                bans.Add("the way");
                bans.Add("the");
                bans.Add("tho nddross");
                bans.Add("ties");
                bans.Add("tig");
                bans.Add("time");
                bans.Add("tion massacre");
                bans.Add("top");
                bans.Add("trademark");
                bans.Add("trinidad riso");
                bans.Add("trinidad rizo text  art  sketch");
                bans.Add("triumph");
                bans.Add("two dozen expedition members");
                bans.Add("two men");
                bans.Add("two months");
                bans.Add("united kingdom");
                bans.Add("unnamed whites");
                bans.Add("usa");
                bans.Add("vengeance");
                bans.Add("vicious carlyle expeditims");
                bans.Add("view");
                bans.Add("vintage photo");
                bans.Add("w. 1");
                bans.Add("waldorfmr");
                bans.Add("wallines");
                bans.Add("way");
                bans.Add("weer alnight");
                bans.Add("white  screenshot  receipt");
                bans.Add("white  screenshot");
                bans.Add("wit");
                bans.Add("word");
                bans.Add("works");
                bans.Add("world");
                bans.Add("writings");
                bans.Add("year");
                bans.Add("years");

                List<string> retVal = new List<string>();
                retVal.AddRange(bans);
                return retVal;
            }
        }
    }
}
