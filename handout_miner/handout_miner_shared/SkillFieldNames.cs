using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace handout_miner_shared {
    public static class SkillFieldNames {
        public static class Inputs {
            public const string AdventureText = "AdventureText";
            public const string SessionText = "SessionText";
            public const string LocationText = "LocationText";

            public const string SourceText  = "sourceText";
            
            public const string MetaData = "metadata";
            public const string Locations = "locations";
            public const string People = "people";
            public const string Dates = "dates";
            public const string Phrases = "phrases";
            public const string Geolocations = "geolocations";
        }

        public static class Outputs {
            public const string SessionSource = "sessionSource";
            public const string LocationSource = "locationSource";
            public const string ResultText = "resultText";
            public const string Locations = "locations";
            public const string People = "people";
            public const string Dates = "dates";
            public const string Phrases = "phrases";
            public const string Geolocations = "geolocations";  
        }

        public static class IndexFields {
            public const string ID = "id";
            public const string FileName = "fileName";
            public const string ImageTags = "imageTags";
            public const string ImageCaption = "imageCaption";
            public const string People = "people";
            public const string Locations = "locations";
            public const string Phrases = "phrases";
            public const string Geolocations = "geolocations";
            public const string BlobMetaData = "blobMetadata";
            public const string Dates = "dates";
            public const string Text = "text";
            public const string ImageLink = "imagelink";
            public const string HOCRData = "hocrData";           
            public const string LocationSource = "locationSource";
            public const string SessionSource = "sessionSource";
            public const string AdventureSource = "adventureSource";
            public const string Height = "height";
            public const string Width = "width";
        }
    }
}
