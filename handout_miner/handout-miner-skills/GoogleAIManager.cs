using Google.Cloud.Language.V1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace handout_miner_skills {
    public class GoogleAIManager {
        private LanguageServiceClient client = null!;
        private static GoogleAIManager instance = null!;
        private GoogleAIManager() { }
        public static GoogleAIManager Instance {
            get {
                if (instance == null) {
                    instance = new GoogleAIManager();
                    instance.Initialzie();
                }
                return instance;
            }
        }

        private void Initialzie() {
            string cred_key_1 = "gcloud_ai_key_1";
            string cred_json_1 = Environment.GetEnvironmentVariable(cred_key_1)!;
            string cred_key_2 = "gcloud_ai_key_2";
            string cred_json_2 = Environment.GetEnvironmentVariable(cred_key_2)!;
            string cred_json = cred_json_1 + cred_json_2;

            LanguageServiceClientBuilder client_bldr = new LanguageServiceClientBuilder();
            client_bldr.JsonCredentials = cred_json;
            client = client_bldr.Build();
        }

        public IEnumerable<string> GetPhrases(string input) {
            Document document = Document.FromPlainText(input);
            AnalyzeEntitiesResponse response = client.AnalyzeEntities(document);
            foreach (var entity in response.Entities) {
                yield return entity.Name;   
            }
        }
    }
}
