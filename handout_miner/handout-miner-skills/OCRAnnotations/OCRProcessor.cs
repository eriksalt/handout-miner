using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace handout_miner_skills.OCRAnnotations
{
    internal static class OCRProcessor
    {
        public static List<OCRWordList> GetWordData(List<OCREntity> inputs, double original_height, double original_width, double normalized_height, double normalized_width)
        {
            double height_scale = original_height / normalized_height;
            double width_scale = original_width / normalized_width;
            Dictionary<string, OCRWordList> data = new Dictionary<string, OCRWordList>();
            foreach (OCREntity entity in inputs)
            {
                string word = entity.Text.ToLower().Trim(" !@#$%^&*()-_=+[{]}\\|'\");:,<.>/?".ToCharArray()).Trim();
                OCRWordReference wordReference = new OCRWordReference();
                wordReference.SetFromOCREntity(entity, height_scale, width_scale);
                if (!data.ContainsKey(word)) data.Add(word, new OCRWordList() { Text = word });
                (data[word]).Uses.Add(wordReference);

            }

            return data.Values.ToList();
        }
    }
}
