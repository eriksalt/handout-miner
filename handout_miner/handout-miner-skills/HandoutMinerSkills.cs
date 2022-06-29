using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Collections;
using Newtonsoft.Json;
using handout_miner_skills.hocr;
using handout_miner_skills.OCRAnnotations;
using System.Text;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text.Number;
using Microsoft.Recognizers.Text.NumberWithUnit;
using HandoutMiner.Shared;

using handout_miner_shared;
namespace handout_miner_skills
{
    public static class Skills
    {

        static char[] punctuation = "!@#$%^&*()_+-=[]\\{}|;':\",./<>? \r\n\t".ToCharArray();
        static char[] whitespace = " \t\r\n".ToCharArray();
        private static char[] square_brackets = "[]".ToCharArray();
        private static AdjustmentManagers adjustments = new AdjustmentManagers();

        [FunctionName("generate-clue-sources")]
        public static async Task<IActionResult> GenerateClueSources(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext executionContext) {
            await Task.CompletedTask;
            return ExecuteSkill(req, log, executionContext.FunctionName,
            (inRecord, outRecord) => {
                string adventure = inRecord.Data[SkillFieldNames.Inputs.AdventureText].ToString();
                string session = inRecord.Data[SkillFieldNames.Inputs.SessionText].ToString();
                string location = inRecord.Data[SkillFieldNames.Inputs.LocationText].ToString();

                string locationSource = adventure + '|' + session + '|' + location;
                log.LogInformation($"Recieved |{adventure}|{session}|{location}|");
                string sessionSource = adventure + '|' + session;
                log.LogInformation($"SessionSource: {sessionSource}");
                log.LogInformation($"LocationSource: {locationSource}");

                outRecord.Data[SkillFieldNames.Outputs.LocationSource] = locationSource;
                outRecord.Data[SkillFieldNames.Outputs.SessionSource] = sessionSource;
                return outRecord;
            });
        }


        [FunctionName("remove-hyphenation")]
        public static async Task<IActionResult> RemoveHyphenation(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext executionContext)
        {
            await Task.CompletedTask;
            return ExecuteSkill(req, log, executionContext.FunctionName,
            (inRecord, outRecord) =>
            {
                var sourceText = inRecord.Data["sourceText"] as string;
                outRecord.Data["resultText"] = sourceText.Replace("- ", "");
                log.LogInformation($"Replace:{sourceText}-->{(string)outRecord.Data["resultText"]}");

                return outRecord;
            });
        }

        [FunctionName("bar-separate")]
        public static async Task<IActionResult> BarSeparate(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext executionContext)
        {
            await Task.CompletedTask;
            return ExecuteSkill(req, log, executionContext.FunctionName,
            (inRecord, outRecord) =>
            {
                ConcatenateHelper helper = new ConcatenateHelper() { Seperator = '|' };
                helper.InputFieldNames.Add("firstText");
                helper.InputFieldNames.Add("secondText");
                helper.InputFieldNames.Add("thirdText");
                helper.InputFieldNames.Add("fourthText");
                helper.InputFieldNames.Add("fifthText");
                return helper.Concatenate(log, inRecord, outRecord);
            });
        }

        [FunctionName("concatenate")]
        public static async Task<IActionResult> Concatenate(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext executionContext)
        {
            await Task.CompletedTask;
            return ExecuteSkill(req, log, executionContext.FunctionName,
            (inRecord, outRecord) =>
            {
                ConcatenateHelper helper = new ConcatenateHelper();
                helper.InputFieldNames.Add("firstText");
                helper.InputFieldNames.Add("secondText");
                return helper.Concatenate(log, inRecord, outRecord);
            });
        }

        [FunctionName("process-entities")]
        public static async Task<IActionResult> ProcessEntities(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
           ILogger log,
           ExecutionContext executionContext) {
            log.LogInformation("Geo Point From Name Custom skill: C# HTTP trigger function processed a request.");
            adjustments.Initialize(log);
            string skillName = executionContext.FunctionName;
            IEnumerable<WebApiRequestRecord> requestRecords = WebApiSkillHelpers.GetRequestRecords(req);
            if (requestRecords == null) {
                return new BadRequestObjectResult($"{skillName} - Invalid request record array.");
            }
            WebApiSkillResponse response = await WebApiSkillHelpers.ProcessRequestRecordsAsync(skillName, requestRecords,
                async (inRecord, outRecord) => {
                    List<string> peopleInputs = new();
                    peopleInputs.AddRange(WebApiSkillHelpers.DeserializeInputValues(inRecord.Data[SkillFieldNames.Inputs.People].ToString()));
                     List<string> dateInputs = new();
                    dateInputs.AddRange(WebApiSkillHelpers.DeserializeInputValues(inRecord.Data[SkillFieldNames.Inputs.Dates].ToString()));
                    List<string> locationInputs = new();
                    locationInputs.AddRange(WebApiSkillHelpers.DeserializeInputValues(inRecord.Data[SkillFieldNames.Inputs.Locations].ToString()));
                    List<string> phraseInputs = new();
                    phraseInputs.AddRange(WebApiSkillHelpers.DeserializeInputValues(inRecord.Data[SkillFieldNames.Inputs.Phrases].ToString()));
                    string inputText = inRecord.Data[SkillFieldNames.Inputs.SourceText].ToString();

                    List<string> googlePhrases = GoogleAIManager.Instance.GetPhrases(inputText).ToList();
                    peopleInputs.AddRange(googlePhrases);
                    locationInputs.AddRange(googlePhrases);
                    phraseInputs.AddRange(googlePhrases);

                    List<string> outputPeople = NormalizeText(log, peopleInputs, new AllowedPeople(), SkillFieldNames.Outputs.People);
                    List<string> outputLocations = NormalizeText(log, locationInputs, new AllowedLocations(), SkillFieldNames.Outputs.Locations);
                    List<string> outputPhrases = NormalizeText(log, phraseInputs, new AllowedPhrases(), SkillFieldNames.Outputs.Phrases);
                    List<string> outputDates = NormalizeDates(log, dateInputs, SkillFieldNames.Outputs.Dates);

                    List<string> outputGeoPoints = await GenerateGeoPoints(log, outputLocations);

                    outRecord.Data[SkillFieldNames.Outputs.People] = outputPeople;
                    outRecord.Data[SkillFieldNames.Outputs.Locations] = outputLocations;
                    outRecord.Data[SkillFieldNames.Outputs.Phrases] = outputPhrases;    
                    outRecord.Data[SkillFieldNames.Outputs.Dates] = outputDates;
                    outRecord.Data[SkillFieldNames.Outputs.Geolocations] = outputGeoPoints;

                    return outRecord;
                });
            return new OkObjectResult(response);
        }

        private static async Task<List<string>> GenerateGeoPoints(ILogger log, List<string> addresses) {
            List<string> geopoints = new();
            foreach (string address in addresses) {
                GeoLookupHelper helper = GeoLookupHelper.Instance;
                string geopoint = await helper.Lookup(log, address);
                if (!string.IsNullOrWhiteSpace(geopoint))
                    geopoints.Add(geopoint); 
            }
            return geopoints;
        }

        private static List<string> NormalizeText(ILogger log, List<string> inputs, AllowedItems items, string traceID) {
            log.LogInformation($"Normalizing {traceID}");
            List<string> outputs = new List<string>();
            foreach (string input in inputs) {
                string normalizedInput= input.ToLower().Trim(punctuation).Normalize();
                log.LogInformation($"NormalizedText:{input}=>{normalizedInput}");
                string output = items.AllowedString(normalizedInput);
                if (string.IsNullOrEmpty(output)) {
                    log.LogInformation($"|{normalizedInput}| not found in allowed items, ignoring.");
                    continue;
                } else {
                    log.LogInformation($"{normalizedInput}=>{output}, adding");
                    outputs.Add(output.ToLower().Trim());
                }
                
            }
            return outputs.Distinct<string>().ToList();
        }

        private static List<string> NormalizeDates(ILogger log, List<string> inputs, string traceID) {
            log.LogInformation($"Normalizing {traceID}");
            List<string> dates = new List<string>();
            List<string> unprocessed_dates = new List<string>();
            foreach (string input in inputs) {
                log.LogInformation($"Processing: {input}");
                if (!AddIfGoodDate(dates, input, log)) {
                    log.LogInformation($"Could not process: {input}, adding to unprocessed queue");
                    unprocessed_dates.Add(input);
                }
            }
            int estimatedYear = EstimateYearFromProcessedDates(dates);
            log.LogInformation($"Estimated Year: {estimatedYear}");
            if (estimatedYear > 0) {
                foreach (string potential_date in unprocessed_dates) {
                    string modified_date = $"{potential_date}, {estimatedYear}";
                    log.LogInformation($"Trying modified date:{modified_date}");
                    AddIfGoodDate(dates, modified_date, log);
                }
            }

            return dates.Distinct<string>().ToList();
        }

        private static bool AddIfGoodDate(List<string>outputDates, string input, ILogger log)
        {

            List<string> bans = adjustments.Bans[AdjustmentNames.Dates];
            List<(string, string)> changes = adjustments.Changes[AdjustmentNames.Dates];
            
            log.LogInformation($"Checking validity of {input}.");
            string normalizedDate = NormalizeDate(input);
            if (string.IsNullOrWhiteSpace(normalizedDate))
            {
                log.LogInformation($"{input} does not appear to be a date.");
                return false;
            }
            log.LogInformation($"Succesfully converted input to:{normalizedDate}");
            System.DateTime dt;
            bool couldParse= DateTime.TryParse(normalizedDate, out dt);
            if(!couldParse)
            {
                log.LogInformation($"Could not parse {normalizedDate} into System.DatTime");
                return false;
            }
            if(dt.Year>1960)
            {
                log.LogInformation($"{DateString(dt)} has an invalid year");
                return false;
            }
            if(bans.Contains(DateString(dt)))
            {
                log.LogInformation($"{DateString(dt)} is banned.");
                return false;
            }
            string outputString = DateString(dt);
            if (changes.Any(x => x.Item1 == outputString))
            {
                log.LogInformation($"{outputString} was changed, replacing with {changes.First(x => x.Item1 == outputString).Item2}");
                outputString = changes.First(x => x.Item1 == outputString).Item2;
            }
            else
            {
                log.LogInformation($"{outputString} was not changed, adding.");
            }
            log.LogInformation($"{outputString} passed all checks, adding to results.");
            outputDates.Add(DateString(dt));
            return true;
        }
        private static string DateString(System.DateTime dt) => dt.ToString("MMMM d, yyyy");

        private static int EstimateYearFromProcessedDates(List<string> dates)
        {
            if (dates.Count <1) return -1;
            if(dates.Count ==1)
            {
                DateTime dt = DateTime.Parse(dates[0]);
                return dt.Year;
            }
            int estimatedYear = -1;
            foreach(string date in dates)
            {
                DateTime dt = DateTime.Parse(date);
                if (estimatedYear > 0 && dt.Year != estimatedYear)
                    return -1;
                estimatedYear = dt.Year;
            }
                
            return estimatedYear;

        }


        static string NormalizeDate(string input)
        {

            var cult = Culture.English;
            List<ModelResult> results = DateTimeRecognizer.RecognizeDateTime(input, cult);
            if (results.Count == 0) return string.Empty;
            ModelResult result = results[0];
            var resolution = result.Resolution;
            object values = resolution["values"];
            List<Dictionary<string, string>> things = (List<Dictionary<string, string>>)values;
            Dictionary<string, string> map = things[0];
            if( (!map.ContainsKey("type")) || map["type"]!="date") return string.Empty;    
            string output = map["value"];
            DateTime dateTime = DateTime.Parse(output);
            return dateTime.ToString("MMMM dd, yyyy").ToLower();
        }

        [FunctionName("generate-ocr-data")]
        public static async Task<IActionResult> GnerateOCRData(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext executionContext)
        {
            await Task.CompletedTask;
            return ExecuteSkill(req, log, executionContext.FunctionName,
            (inRecord, outRecord) =>
            {
                string input = unwrap_possible_double_array( inRecord.Data["words"].ToString());
                double original_height = GetFirstDouble(inRecord, "original_height"); 
                double original_width = GetFirstDouble(inRecord, "original_width"); 
                double normalized_height = GetFirstDouble(inRecord, "normalize_height"); 
                double normalized_width = GetFirstDouble(inRecord, "normalized_width");  

                List<OCREntity> words = JsonConvert.DeserializeObject<List<OCREntity>>(input);
                List<OCRWordList> results = OCRProcessor.GetWordData(words,original_height, original_width, normalized_height, normalized_width);
                outRecord.Data["results"]=results;
                return outRecord;

            });
        }

        private static double GetFirstDouble(WebApiRequestRecord inRecord, string name)
        {
            string strValue = inRecord.Data[name].ToString();
            return (double)(JsonConvert.DeserializeObject<int[]>(strValue)[0]);

        }

        public static string DictionaryToString(this Dictionary<string, object> source, string keyValueSeparator, string sequenceSeparator)
        {
            if (source == null)
                throw new ArgumentException("Parameter source can not be null.");

            var pairs = source.Select(x => string.Format("'{0}'{1}'{2}'", x.Key, keyValueSeparator, x.Value.ToString()));

            return "{" + string.Join(sequenceSeparator, pairs) + "}";
        }
        private static IActionResult ExecuteSkill(HttpRequest req, ILogger log, string skill_name, Func<WebApiRequestRecord, WebApiResponseRecord, WebApiResponseRecord> processRecord)
        {
            //log.LogInformation($"{skill_name} Custom Skill: C# HTTP trigger function processed a request.");

            IEnumerable<WebApiRequestRecord> requestRecords = WebApiSkillHelpers.GetRequestRecords(req);
            if (requestRecords == null)
            {
                log.LogError($"{skill_name} - Invalid request record array.");
                return new BadRequestObjectResult($"{skill_name} - Invalid request record array.");
            }

            WebApiSkillResponse response = WebApiSkillHelpers.ProcessRequestRecords(skill_name, requestRecords, processRecord);

            return new OkObjectResult(response);
        }

        private static async Task<IActionResult> ExecuteSkillAsync(HttpRequest req, ILogger log, string skill_name, Func<WebApiRequestRecord, WebApiResponseRecord, Task<WebApiResponseRecord>> processRecord) {
            //log.LogInformation($"{skill_name} Custom Skill: C# HTTP trigger function processed a request.");

            IEnumerable<WebApiRequestRecord> requestRecords = WebApiSkillHelpers.GetRequestRecords(req);
            if (requestRecords == null) {
                log.LogError($"{skill_name} - Invalid request record array.");
                return new BadRequestObjectResult($"{skill_name} - Invalid request record array.");
            }

            WebApiSkillResponse response = await WebApiSkillHelpers.ProcessRequestRecordsAsync(skill_name, requestRecords, processRecord);

            return new OkObjectResult(response);
        }

        public static string unwrap_possible_double_array(string data)
        {
            string input = data.Trim();
            input = input.Trim(square_brackets);
            input = input.Trim();
            input = input.Trim(square_brackets);
            input = "[" + input.Trim() + "]";
            return input;
        }

    }
       
}