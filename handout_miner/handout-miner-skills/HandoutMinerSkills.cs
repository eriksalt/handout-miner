using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System;
//using AzureCognitiveSearch.PowerSkills.Common;

namespace handout_miner_skills
{
    public static class Skills
    {
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