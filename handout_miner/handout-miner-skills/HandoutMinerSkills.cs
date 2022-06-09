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

namespace handout_miner_skills
{
    public static class Skills
    {
        private static readonly string nominatumUriPart1 = "https://nominatim.openstreetmap.org/search?q=";
        private static readonly string nominatumUriPart2 = "&format=jsonv2&limit=1&accept-language=en-us";
        static char[] puctuation = "!@#$%^&*()_+-=[]\\{}|;':\",./<>? \r\n\t".ToCharArray();
        static char[] whitespace = " \t\r\n".ToCharArray();
        private static char[] square_brackets = "[]".ToCharArray();
        private static AdjustmentManagers adjustments = new AdjustmentManagers();
        
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

        [FunctionName("normalize-people-arrays")]
        public static async Task<IActionResult> NormalizePeopleArrays(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
           ILogger log,
           ExecutionContext executionContext)
        {
            await Task.CompletedTask;
            adjustments.Initialize(log);
            return ExecuteSkill(req, log, executionContext.FunctionName,
            (inRecord, outRecord) =>
            {
                NormalizeText(log, inRecord, outRecord, adjustments.Bans[AdjustmentNames.People], adjustments.Changes[AdjustmentNames.People],
                    (input) => (input.Split(' ').Length > 1) ? input : string.Empty);
                return outRecord;
            });
        }

        [FunctionName("normalize-phrase-arrays")]
        public static async Task<IActionResult> NormalizePhraseArrays(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
           ILogger log,
           ExecutionContext executionContext)
        {
            await Task.CompletedTask;
            adjustments.Initialize(log);
            return ExecuteSkill(req, log, executionContext.FunctionName,
            (inRecord, outRecord) =>
            {
                NormalizeText(log, inRecord, outRecord, adjustments.Bans[AdjustmentNames.Phrases], adjustments.Changes[AdjustmentNames.Phrases]);
                return outRecord;
            });
        }

        [FunctionName("normalize-location-arrays")]
        public static async Task<IActionResult> NormalizeLocationArrays(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
           ILogger log,
           ExecutionContext executionContext)
        {
            await Task.CompletedTask;
            adjustments.Initialize(log);
            return ExecuteSkill(req, log, executionContext.FunctionName,
            (inRecord, outRecord) =>
            {
                NormalizeText(log, inRecord, outRecord, adjustments.Bans[AdjustmentNames.Locations], adjustments.Changes[AdjustmentNames.Locations]);
                return outRecord;
            });
        }        

        private static void NormalizeText(ILogger log, WebApiRequestRecord inRecord, WebApiResponseRecord outRecord, List<string> bans, List<(string,string)> changes, Func<string, string> lastProcess=null)
        {
            List<string> inputs = new List<string>();
            if (inRecord.Data.ContainsKey("inputValues"))
            {
                inputs.AddRange(WebApiSkillHelpers.DeserializeInputValues(inRecord.Data["inputValues"].ToString()));
            }
            List<string> outputs = new List<string>();
            foreach (string input in inputs)
            {
                string output = input.ToLower().Trim(puctuation).Normalize();
                log.LogInformation($"NormalizedText:{input}=>{output}");
                if (bans.Contains(output))
                {
                    log.LogInformation($"|{input}| was banned, ignoring.");
                    continue;
                }
                else if(changes.Any(x => x.Item1 == output))
                {
                    log.LogInformation($"|{output}| was changed, replacing with {changes.First(x => x.Item1 == output).Item2}");
                    output = changes.First(x => x.Item1 == output).Item2;
                }
                if (lastProcess == null)
                {
                    log.LogInformation($"No lastProcess, adding |{output}|");
                    outputs.Add(output);
                }
                else 
                {
                    string lastOutput = lastProcess(output);
                    log.LogInformation("Calling lastProcess");
                    if (string.IsNullOrWhiteSpace(lastOutput))
                    {
                        log.LogInformation($"Last process returned empty string. Ignoring.");
                    }
                    else if (lastOutput==output)
                    {
                        log.LogInformation($"lastProcess did not change output, adding |{output}|");
                        outputs.Add(output);
                    }
                    else
                    {
                        log.LogInformation($"lastProcess changed |{output}| to |{lastOutput}|.Adding.");
                        outputs.Add(lastOutput);
                    }
                }
            }
            outRecord.Data["normalizedValues"] = outputs.Distinct<string>().ToList();
        }


        [FunctionName("normalize-datetime-arrays")]
        public static async Task<IActionResult> NormalizeDateTimeArrays(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext executionContext)
        {
            await Task.CompletedTask;
            adjustments.Initialize(log);
            return ExecuteSkill(req, log, executionContext.FunctionName,
            (inRecord, outRecord) =>
            {
                List<string> inputs = new List<string>();
                if (inRecord.Data.ContainsKey("inputValues"))
                {
                    inputs.AddRange(WebApiSkillHelpers.DeserializeInputValues(inRecord.Data["inputValues"].ToString()));
                }
                List<string> dates = new List<string>();
                List<string> unprocessed_dates = new List<string>();
                foreach (string input in inputs)
                {
                    log.LogInformation($"Processing: {input}");
                    if (!AddIfGoodDate(dates, input, log))
                    {
                        log.LogInformation($"Could not process: {input}, adding to unprocessed queue");
                        unprocessed_dates.Add(input);
                    }
                }
                int estimatedYear = EstimateYearFromProcessedDates(dates);
                log.LogInformation($"Estimated Year: {estimatedYear}");
                if (estimatedYear > 0)
                {
                    foreach (string potential_date in unprocessed_dates)
                    {
                        string modified_date = $"{potential_date}, {estimatedYear}";
                        log.LogInformation($"Trying modified date:{modified_date}");
                        AddIfGoodDate(dates, modified_date, log);
                    }
                }

                outRecord.Data["normalizedValues"] = dates.Distinct<string>().ToList();
                return outRecord;
            });
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


        [FunctionName("normalize-geolocation-arrays")]
        public static async Task<IActionResult> NormalizeGeolocationArrays(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext executionContext)
        {
            await Task.CompletedTask;
            return ExecuteSkill(req, log, executionContext.FunctionName,
            (inRecord, outRecord) =>
            {
                List<string> inputs = new List<string>();
                if (inRecord.Data.ContainsKey("inputValues"))
                {
                    inputs.AddRange(WebApiSkillHelpers.DeserializeInputValues(inRecord.Data["inputValues"].ToString()));
                }
                List<string> geolocations = new List<string>();
                foreach (string input in inputs)
                {
                    string loc = input;
                    string normalizedLocation = loc.Trim().ToLower();
                    log.LogInformation($"{loc}=>{normalizedLocation}");
                    geolocations.Add(normalizedLocation);
                }
                outRecord.Data["normalizedValues"] = geolocations.Distinct<string>().ToList();
                return outRecord;
            });
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

        [FunctionName("geo-point-from-name")]
        public static async Task<IActionResult> GeoPointFromName(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
           ILogger log,
           ExecutionContext executionContext)
        {
            log.LogInformation("Geo Point From Name Custom skill: C# HTTP trigger function processed a request.");

            string skillName = executionContext.FunctionName;
            IEnumerable<WebApiRequestRecord> requestRecords = WebApiSkillHelpers.GetRequestRecords(req);
            if (requestRecords == null)
            {
                return new BadRequestObjectResult($"{skillName} - Invalid request record array.");
            }

            WebApiSkillResponse response = await WebApiSkillHelpers.ProcessRequestRecordsAsync(skillName, requestRecords,
                async (inRecord, outRecord) => {
                    log.LogInformation($"Processing: {DictionaryToString(inRecord.Data, ":", ",")}");
                    object address = inRecord.Data["address"];
                    List<string> addresses = new();
                    List<string> geoPoints = new();
                    CompileAddresses(address, addresses, log);
                    foreach (string strAddress in addresses)
                    {
                        log.LogInformation($"Looking up '{strAddress}'");
                        if (string.IsNullOrEmpty(strAddress))
                        {
                            log.LogInformation("Address is not found.");
                            continue; 
                        }
                        
                        string uri = nominatumUriPart1 + strAddress + nominatumUriPart2;
                        //log.LogInformation($"Calling: {uri}");

                        JObject response = (await WebApiSkillHelpers.SimpleFetchAsync(uri, System.Net.Http.HttpMethod.Get));
                        if (response is null)
                        {
                            log.LogInformation($"'{strAddress}' not found in Open Street Maps.");
                            continue;
                        }
                        double lat = response["lat"].Value<double>();
                        double lon = response["lon"].Value<double>();

                        string normalizedAddress = strAddress.Replace(',', ' ');
                        normalizedAddress = strAddress.Replace('"', ' ');
                        normalizedAddress = strAddress.Replace('\'', ' ');
                        log.LogInformation($"{strAddress}=>{normalizedAddress}");

                        log.LogInformation($"Adding new GPS coordinates {lat},{lon} for {normalizedAddress}");
                        geoPoints.Add($"{normalizedAddress}|{lat}|{lon}");
                    }
                    outRecord.Data["results"] = geoPoints.ToArray();
                    return outRecord;
                });

            return new OkObjectResult(response);
        }

        public static void CompileAddresses(object address, List<string> addressList,ILogger log)
        {
            if(address is Newtonsoft.Json.Linq.JArray)
            {
                //log.LogInformation($"Found array for object '{address}' of type '{address.GetType()}', recursing");
                IEnumerable e = address as IEnumerable;
                if (e != null)
                {
                    foreach (object obj in e)
                    {
                        CompileAddresses(obj, addressList, log);
                    }
                }
            }
            else if (address is string)
            {
                log.LogInformation($"Adding {address} to queue from string");
                addressList.Add(address as string);
            }
            else if (address is Newtonsoft.Json.Linq.JValue)
            {
                string strAddress = (address as Newtonsoft.Json.Linq.JValue).Value<string>();
                log.LogInformation($"Adding {strAddress} to queue from JValue");
                addressList.Add(strAddress);

            }
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