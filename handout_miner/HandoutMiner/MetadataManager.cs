using Azure.Storage.Blobs;
using handout_miner_shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandoutMiner
{
    public static class MetadataManager
    {
        private static AzureConfig _config = new AzureConfig();
        public static async Task UploadMetadata(List<SourceHandout> handouts)
        {
            foreach (SourceHandout handout in handouts)
            {
                await SourceHandoutManager.SaveToMetadata(handout, _config.storage_connection_string, _config.storage_main_container_name);
            }
        }
    }
}
