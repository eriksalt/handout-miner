using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace handout_miner_skills.OCRAnnotations
{
    public class OCRWordList
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("uses")]
        public List<OCRWordReference> Uses { get; set; } = new List<OCRWordReference>();
    }   
}
