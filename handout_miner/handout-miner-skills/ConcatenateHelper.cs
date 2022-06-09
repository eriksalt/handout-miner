using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace handout_miner_skills
{
    public class ConcatenateHelper
    {
        public List<string> InputFieldNames { get; } = new();
        public char Seperator { get; set; } = ' ';
        public string OutputFieldName { get; set; } = "resultText";
        public WebApiResponseRecord Concatenate(ILogger log, WebApiRequestRecord inRecord, WebApiResponseRecord outRecord)
        {
            StringBuilder bldr = new StringBuilder();

            foreach(string inputFieldName in InputFieldNames)
            {
                log.LogInformation($"Looking for field {inputFieldName}");
                if (inRecord.Data.ContainsKey(inputFieldName))
                {
                    log.LogInformation($"{inputFieldName}:{inRecord.Data[inputFieldName].ToString()}");
                    bldr.Append(inRecord.Data[inputFieldName].ToString());
                    bldr.Append(Seperator);
                }
                else
                {
                    log.LogInformation($"{inputFieldName} not found.");
                }
            }

            string resultText = bldr.ToString();
            if (resultText.Length > 0) resultText = resultText.Substring(0, resultText.Length - 1);
            log.LogInformation($"Processed: {resultText}");
            outRecord.Data[OutputFieldName] = resultText;

            return outRecord;
        }
    }
}
