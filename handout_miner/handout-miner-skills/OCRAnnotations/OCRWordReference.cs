using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace handout_miner_skills.OCRAnnotations
{
    public class OCRWordReference
    {
        [JsonPropertyName("top")]
        public int Top { get; set; } = 0;
        [JsonPropertyName("left")]
        public int Left { get; set; } = 0;
        [JsonPropertyName("right")] 
        public int Right { get; set; } = 0;
        [JsonPropertyName("bottom")] 
        public int Bottom { get; set; } = 0;

        public void SetFromOCREntity(OCREntity entity)
        {
            List<OCRPoint> points = entity.BoundingBox;
            Left = points.Select(pt => pt.X).Min();
            Top = points.Select(pt => pt.Y).Min();
            Right = points.Select(pt => pt.X).Max();
            Bottom = points.Select(pt => pt.Y).Max();
        }
    }
}
