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

        private int Round(int val, double scale_value)
        {
            return val;
            //double retVal = ((double)val) * scale_value;
            //return (int)(Math.Round(retVal));
        }

        public void SetFromOCREntity(OCREntity entity, double height_scale, double width_scale)
        {
            List<OCRPoint> points = entity.BoundingBox;
            Left = Round(points.Select(pt => pt.X).Min(), width_scale);
            Top = Round(points.Select(pt => pt.Y).Min(), height_scale);
            Right = Round(points.Select(pt => pt.X).Max(), width_scale);
            Bottom = Round(points.Select(pt => pt.Y).Max(), height_scale);
        }
    }
}
