using System;
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
                List<string> bans = new List<string>();
                bans.Add("new york pillar/riposte");
                bans.Add("text  newspaper  screenshot");
                bans.Add("sir aubrey penhew");
                bans.Add("roger carlyle");
                bans.Add("dr. robert huston");
                bans.Add("new york socialite");
                bans.Add("countcarlyle");
                bans.Add("shanghai");
                bans.Add("china");
                bans.Add("las harvard cambridge");
                bans.Add("new york city");
                bans.Add("mr. jackson elias");
                bans.Add("mr. elias");
                bans.Add("lexington avenue");
                bans.Add("massachusetts");
                bans.Add("book");
                bans.Add("arrival");
                bans.Add("sketch  drawing  text  painting  art");
                bans.Add("human face  portrait");
                bans.Add("person");
                bans.Add("many skreads");
                bans.Add("belden e");
                bans.Add("carlyle");
                bans.Add("ooks");
                bans.Add("fon");
                bans.Add("quithere");
                bans.Add("text  screenshot  letter  document  text");
                bans.Add("george sorel");
                bans.Add("rebecca west");
                bans.Add("jonah kensington");
                bans.Add("stratford");
                bans.Add("connecticut");
                bans.Add("reuters)-uplands egyptologist");
                bans.Add("united kingdom");
                bans.Add("riposte");
                bans.Add("nairobi");
                bans.Add("drawing  sketch  painting");
                bans.Add("cartoon  brick  stone");
                bans.Add("hotel chelsea");
                bans.Add("handwriting  text  letter  document  text");
                bans.Add("dear janak");
                bans.Add("text  painting  statue  cartoon  portrait  spectacles");
                bans.Add("human face  sketch  face");
                bans.Add("prof. memesio sanchez");
                bans.Add("mrs. victoria post");
                bans.Add("miss erica carlyle");
                bans.Add("miss carlyle");
                bans.Add("aberdare forest");
                bans.Add("reuters");
                bans.Add("handwriting  sketch  stationary  cartoon  envelope  accessory  graffiti");
                bans.Add("text  child art  painting");
                bans.Add("10 lantern street");
                bans.Add("sketch  text  cartoon  art");
                bans.Add("photo");
                bans.Add("text  screenshot  letter  colorfulness  document  text");
                bans.Add("lt. mark selkirk");
                bans.Add("lt. selkirk");
                bans.Add("roger corydon");
                bans.Add("sam mariga");
                bans.Add("neville jermyn");
                bans.Add("hong kong");
                bans.Add("jack brady");
                bans.Add("nails\" nelson");
                bans.Add("indoor  sketch  man  art");
                bans.Add("human face  statue");
                bans.Add("miss hypatia masters");
                bans.Add("white  screenshot  receipt");
                bans.Add("reuters)-leading expedition");
                bans.Add("royston whittingdon");
                bans.Add("text  newspaper");
                bans.Add("egypt");
                bans.Add("mr. carlyle");
                bans.Add("white  screenshot");
                bans.Add("text  screenshot  typography  font  text");
                bans.Add("prof. anthony cowles");
                bans.Add("southwest pacific");
                bans.Add("human face  text  sketch");
                bans.Add("smile  face  portrait  woman");
                bans.Add("carlyle expedition stop need reliable investigative team stop meet");
                bans.Add("new york stop jackson elias blank form");
                bans.Add("handwriting  letter  receipt  text");
                bans.Add("hudson terminal");
                bans.Add("newspaper  font  receipt  text");
                bans.Add("explorer augustus larkin");
                bans.Add("cee cartoon  drawing  text  art");
                bans.Add("handwriting  letter  calligraphy  typography  script  document  text");
                bans.Add("sky  lake  water  boat  outdoor  watercraft  ship  yellow  vehicle");
                bans.Add("abstract  tattoo  graffiti");
                bans.Add("drawing  sketch  painting  text  art  animal  cartoon");
                bans.Add("old  abandoned  text  decay  map");
                bans.Add("text  drawing  sketch  book  cartoon  handwriting  illustration");
                bans.Add("handwriting  letter  receipt  typography  document  text");
                bans.Add("jirón ancash");
                bans.Add("augustus larkin");
                bans.Add("luis de");
                bans.Add("art  text");
                bans.Add("mr. jack brady");
                bans.Add("aubrey penhew");
                bans.Add("hypatia masters");
                bans.Add("648 west 47th street new york");
                bans.Add("handwriting  text  letter  businesscard  text");
                bans.Add("indoor  text  art  gallery  room  painting  picture");
                bans.Add("old  wall  child art");
                bans.Add("drawing  monitor  sketch  television");
                bans.Add("hand");
                bans.Add("reptile");


                return bans;
            }
        }
    }
}
