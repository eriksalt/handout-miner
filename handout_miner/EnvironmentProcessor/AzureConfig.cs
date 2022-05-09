using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentProcessor
{
    internal class AzureConfig
    {
        public string storage_connection_string { get { return ConfigurationManager.AppSettings["storage_connection_string"]; } }
        public string storage_main_container_name { get { return "handout-miner-sources"; } }
        public string storage_projection_container_name { get { return "handout-miner-projection"; } }
        public string source_files_directory { get { return @"D:\Dev\Git\handout-miner\sources_test"; } }//return @"D:\Dev\Git\handout_miner\srouces_raw"; } }
        public string search_datasource_name { get { return "handout-miner-datasource"; } }
        public string search_service_name { get { return "saltyhmsearch"; } }
        public string search_service_dns_suffix { get { return "search.windows.net"; } }
        public string azure_ap_version { get { return "2021-04-30-Preview"; } }
        public Uri search_service_endpoint { get { return new Uri(string.Format("https://{0}.{1}", search_service_name, search_service_dns_suffix)); } }
        public string search_key { get { return ConfigurationManager.AppSettings["search_key"]; } }
        public string skillset_name { get { return "handout-miner-skillset"; } }
        public string cog_srv_key { get { return ConfigurationManager.AppSettings["cog_srv_key"]; } }
        public string cog_srv_endpoint { get { return "https://saltyhmcogsrv.cognitiveservices.azure.com/"; } }
        public string synonym_map_name { get { return "handout-miner-synonyms"; } }
        public string index_name { get { return "handout-miner-index"; } }
        public string indexer_name { get { return "handout-miner-indexer"; } }
        public string custom_skills_site { get { return "https://handoutminerskills.azurewebsites.net"; } }
        public string custom_skills_key { get { return ConfigurationManager.AppSettings["custom_skills_key"]; } }
        public string blob_metadata_name { get { return "blobdescription"; } }
    }
}


/*
 *         public string storage_connection_string { get { return "DefaultEndpointsProtocol=https;AccountName=saltyhmstorage;AccountKey=gMUKNw/PY/Nu/g+Sjpy0pUx1MLBH6mlE07yuTY2/Sbkc4V5YWDSxkFbWHBW0pwtPNirSX2s1qJepYoTfqJD2yw==;EndpointSuffix=core.windows.net"; } }
        public string storage_main_container_name { get { return "handout-miner-sources"; } }
        public string storage_projection_container_name { get { return "handout-miner-projection"; } }
        public string source_files_directory { get { return @"D:\Dev\Git\handout_miner\sources_test"; } }//return @"D:\Dev\Git\handout_miner\srouces_raw"; } }
        public string search_datasource_name { get { return "handout-miner-datasource"; } }
        public string search_service_name { get { return "saltyhmsearch"; } }
        public string search_service_dns_suffix { get { return "search.windows.net"; } }
        public string azure_ap_version { get { return "2021-04-30-Preview"; } }
        public Uri search_service_endpoint { get { return new Uri(string.Format("https://{0}.{1}", search_service_name, search_service_dns_suffix)); } }
        public string search_key { get { return "38C5358677E433D6BAFDE6F086562250"; } }
        public string skillset_name { get { return "handout-miner-skillset"; } }
        public string cog_srv_key { get { return "fc6be7194d1247bf9403f35c94805fb5"; } }
        public string cog_srv_endpoint { get { return "https://saltyhmcogsrv.cognitiveservices.azure.com/"; } }
        public string synonym_map_name { get { return "handout-miner-synonyms"; } }
        public string index_name { get { return "handout-miner-index"; } }
        public string indexer_name { get { return "handout-miner-indexer"; } }
        public string custom_skills_site { get { return "https://handoutminerskills.azurewebsites.net"; } }
        public string custom_skills_key { get { return "COCFtyQQe_hVwr_z8XpEXBrjjdgBQPIcxryO4KjZqL3TAzFuG9yOpA=="; } }

*/