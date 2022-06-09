using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace handout_miner_shared
{
    public static class SourceHandoutManager
    {
        private const string description_metadata_name = "blobdescription";
        private const string session_metadata_name = "session";
        private const string adventure_metadata_name = "adventure";
        private const string source_metadata_name = "source";
        private static Dictionary<string, string> GetDescriptions()
        {
            Dictionary<string, string> descriptions = new();

            descriptions.Add("003.png".ToLower(), ".Augustus Larkin.  Treasure hunter with black veins and a circular spiral tattoo on his chest. Organized a bunch of foreigners and Elias Jackson to explore the pyramid near Puno, Peru.");
            descriptions.Add("004.png".ToLower(), ".Luis de Mendoza, a Kharisiri fat sucking vampire, who killed and are Trinidad Riso.  Tunred out to be the ancient Conquistador Gaspar Figueroa.");
            descriptions.Add("005.png".ToLower(), ".Professor Nemesio Sanchez - professor at the Universidad Nacional Mayor de San Marcos (National University of San Marcos).");
            descriptions.Add("006.png".ToLower(), ".Trinidad Riso, a researcher into folklore at the She has been helping Prof. Sanchez has a copy of the Final Confessions of Gaspar Figueroa.");
            descriptions.Add("017.png".ToLower(), ".A harbor in Shanghai, China.");
            descriptions.Add("018.png".ToLower(), ".Book of matches from the Stumbling Tiger Bar, in Shanghai, China.");
            descriptions.Add("020.png".ToLower(), ".Business cards for the Emerson Imports company in New York City, with the name of Silas N'Kwane on the back.");
            descriptions.Add("021.png".ToLower(), ".A brand found on the head of Elias Jackson after he was murdered in his hotel room in the Chelsea Hotel, new york city.");
            descriptions.Add("023.png".ToLower(), ".A young Kharisiri vomiting up fat in Puno, Pero.");
            descriptions.Add("025.png".ToLower(), ".Break in the gold inlay of the pyramid at Puno, Peru near Lake Titicaca, Peru.");
            descriptions.Add("026.png".ToLower(), ".The pyramid compound found near Puno, Peru and Lake Titicaca, Peru.");
            descriptions.Add("027.png".ToLower(), ".Wall detail on a pyramid at Puno, Peru near Lake Titicaca, Peru.");
            descriptions.Add("028.png".ToLower(), ".Tattoo found on Augustus Larkin in a hotel room in Lima, Peru.");
            descriptions.Add("029.png".ToLower(), ".Floating reed canoe on Lake Titicaca, Peru by Puno, Peru.");
            descriptions.Add("030.png".ToLower(), ".Nayra, a wise woman living on the floating village of Puno, Peru on Lake Titicaca, Peru.");
            descriptions.Add("031.png".ToLower(), ".Map of Peru.");
            descriptions.Add("032.png".ToLower(), ".gold mask from the pyramid at Puno,Peru by Lake Titicaca, Peru. It was found in the hotel room of Luis De Mendoza in Lima, Peru.");
            descriptions.Add("033.png".ToLower(), ".Gold inlay which had brokwn off from the pyamid at Puno, Peru by Lake Titicaca, Peru. It was found in the office of Trinidad Riso.");
            descriptions.Add("034.png".ToLower(), ".Luis De Mendoza, a Kharisiri, or fat sucking vampire, killing and eating Trinidad Riso.");
            descriptions.Add("039.png".ToLower(), ".Augustus Larkin as found in his hotel room in Lima, Peru.");
            descriptions.Add("046.jpg".ToLower(), ".Details of a vision, or dream, had by one of the PCs while touching the gold mask found in Lima, Peru.");
            descriptions.Add("047.png".ToLower(), ".Pranga, an african throwing knife. Found in the room of Elias Jackson.");
            descriptions.Add("049.png".ToLower(), ".Captain Robson, the police detective responsible for investigating the new york city murders where people had their foreheads carved.");
            descriptions.Add("050.png".ToLower(), ".Erica Carlyle, the brother of Roger Carlyle and now the head of the Carlyle Estate.");
            return descriptions;
        }

        public static IEnumerable<SourceHandout> CollectHandouts(System.IO.DirectoryInfo sourceDirectory)
        {
            Dictionary<string, string> descriptions = GetDescriptions();
            foreach(DirectoryInfo adventureDirectory in sourceDirectory.EnumerateDirectories())
            {
                foreach(DirectoryInfo sessionDirectory in adventureDirectory.EnumerateDirectories())
                {
                    foreach (DirectoryInfo handoutSourceDirectory in sessionDirectory.EnumerateDirectories())
                    {
                        foreach (FileInfo file in handoutSourceDirectory.EnumerateFiles())
                        {
                            SourceHandout handout = new SourceHandout()
                            {
                                Adventure = adventureDirectory.Name.ToLower(),
                                SessionDate = DateTime.ParseExact(sessionDirectory.Name, "yyyyMMdd", null),
                                Source = handoutSourceDirectory.Name.ToLower(),
                                File = file
                            };
                            if (descriptions.ContainsKey(file.Name.ToLower()))
                            {
                                handout.Description = descriptions[file.Name];
                            }
                            yield return handout;
                        }
                    }
                }
            }
        }

        public static async Task SaveToMetadata(SourceHandout handout, string storage_connection_string, string storage_main_container_name)
        {
            BlobClient client = new BlobClient(storage_connection_string, storage_main_container_name, handout.File.Name);
            Dictionary<string, string> metadata = new Dictionary<string, string>();
            SetMetadataIfNotEmpty(handout, description_metadata_name, handout.Description, metadata);
            SetMetadataIfNotEmpty(handout, session_metadata_name, handout.SessionDate.ToString("MMMM d, yyyy"), metadata);
            SetMetadataIfNotEmpty(handout, adventure_metadata_name, handout.Adventure, metadata);
            SetMetadataIfNotEmpty(handout, source_metadata_name, handout.Source, metadata);
            await client.SetMetadataAsync(metadata);
        }

        public static void SetMetadataIfNotEmpty(SourceHandout handout, string metadata_name, string metadta_value, Dictionary<string, string> metadata)
        {
            string value = metadta_value;
            if (string.IsNullOrWhiteSpace(value))
                value = "_";
            Console.WriteLine($"{handout.File.Name} metatdata-{metadata_name}: {value}");
            metadata.Add(metadata_name, value);
        }
    }
}

