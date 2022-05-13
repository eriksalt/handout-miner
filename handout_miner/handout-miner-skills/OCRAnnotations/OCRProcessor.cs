using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace handout_miner_skills.OCRAnnotations
{
    internal static class OCRProcessor
    {
        public static List<OCRWordList> GetWordData(List<OCREntity> inputs)
        {
            Dictionary<string, OCRWordList> data = new Dictionary<string, OCRWordList>();
            foreach (OCREntity entity in inputs)
            {
                string word = entity.Text.ToLower().Trim(" !@#$%^&*()-_=+[{]}\\|'\");:,<.>/?".ToCharArray()).Trim();
                OCRWordReference wordReference = new OCRWordReference();
                wordReference.SetFromOCREntity(entity);
                if (!data.ContainsKey(word)) data.Add(word, new OCRWordList() { Text = word });
                (data[word]).Uses.Add(wordReference);

            }

            return data.Values.ToList();
        }
    }
}
