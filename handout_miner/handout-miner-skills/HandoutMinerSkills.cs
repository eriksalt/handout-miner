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

namespace handout_miner_skills
{
    public class GeoPoint
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
    }
    public static class Skills
    {
        private static readonly string nominatumUriPart1 = "https://nominatim.openstreetmap.org/search?q=";
        private static readonly string nominatumUriPart2 = "&format=jsonv2&limit=1&accept-language=en-us";

        [FunctionName("remove-hyphenation")]
        public static async Task<IActionResult> RemoveHyphenation(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext executionContext)
        {
            return ExecuteSkill(req, log, executionContext.FunctionName,
            (inRecord, outRecord) =>
            {
                var sourceText = inRecord.Data["sourceText"] as string;
                outRecord.Data["resultText"] = sourceText.Replace("- ", "");
                log.LogInformation($"Replace:{sourceText}-->{(string)outRecord.Data["resultText"]}");
                return outRecord;
            });
        }

        [FunctionName("concatenate")]
        public static async Task<IActionResult> Concatenate(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log,
            ExecutionContext executionContext)
        {
            return ExecuteSkill(req, log, executionContext.FunctionName,
            (inRecord, outRecord) =>
            {
                string firstText = inRecord.Data["firstText"] as string;
                string secondText = inRecord.Data["secondText"] as string;
                string resultText = firstText +  " " + secondText;
                log.LogInformation($"Processed: {resultText}");
                outRecord.Data["resultText"] = resultText;
                return outRecord;
            });
        }

        [FunctionName("geo-point-from-name")]
        public static async Task<IActionResult> RunGeoPointFromName(
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
                    List<string> addresses = new List<string>();
                    List<GeoPoint> geoPoints = new List<GeoPoint>();
                    CompileAddresses(address, addresses, log);
                    foreach (string strAddress in addresses)
                    {
                        if (string.IsNullOrEmpty(strAddress))
                        {
                            log.LogInformation("Address is not found.");
                            continue; 
                        }
                        log.LogInformation($"Looking up '{strAddress}'");
                        string uri = nominatumUriPart1 + strAddress + nominatumUriPart2;
                        log.LogInformation($"Calling: {uri}");

                        JObject response = (await WebApiSkillHelpers.SimpleFetchAsync(uri, System.Net.Http.HttpMethod.Get));
                        if (response is null)
                        {
                            log.LogInformation($"'{strAddress}' not found in Open Street Maps.");
                            continue;
                        }
                        double lat = response["lat"].Value<double>();
                        double lon = response["lon"].Value<double>();
                        GeoPoint point = new GeoPoint() { latitude = lat, longitude = lon };
                        log.LogInformation($"Adding new GPS coordinates {point.latitude},{point.longitude} for {response["display_name"]}");
                        geoPoints.Add(point);
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
                log.LogInformation($"Found array for object '{address}' of type '{address.GetType()}', recursing");
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
            log.LogInformation($"{skill_name} Custom Skill: C# HTTP trigger function processed a request.");

            IEnumerable<WebApiRequestRecord> requestRecords = WebApiSkillHelpers.GetRequestRecords(req);
            if (requestRecords == null)
            {
                log.LogError($"{skill_name} - Invalid request record array.");
                return new BadRequestObjectResult($"{skill_name} - Invalid request record array.");
            }

            WebApiSkillResponse response = WebApiSkillHelpers.ProcessRequestRecords(skill_name, requestRecords, processRecord);

            return new OkObjectResult(response);
        }


    }
}