using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace handout_miner_skills.OCRAnnotations
{
    public class OCRPoint
    {
        public OCRPoint()
        {
            X = 0;
            Y = 0;
        }
        public OCRPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
        [JsonPropertyName("x")]
        public int X { get; set; }
        [JsonPropertyName("y")]
        public int Y { get; set; }
    }
}
