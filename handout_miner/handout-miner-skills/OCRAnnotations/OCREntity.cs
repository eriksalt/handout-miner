using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace handout_miner_skills.OCRAnnotations
{
    public class OCREntity
    {
        
        [JsonPropertyName("boundingBox")]
        public List<OCRPoint> BoundingBox { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
