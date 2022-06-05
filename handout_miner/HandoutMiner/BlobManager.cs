using Azure.Storage.Blobs;
using handout_miner_shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandoutMiner
{
    internal static class BlobManager
    {
        private static AzureConfig _config = new AzureConfig();
        
        public static  async Task Clean()
        {
            BlobServiceClient blob_client = new BlobServiceClient(_config.storage_connection_string);
            try
            {
                await blob_client.DeleteBlobContainerAsync(_config.storage_main_container_name);
            }
            catch (Azure.RequestFailedException azex) { if (azex.Status != 404) { throw azex; } }
            try
            {
                await blob_client.DeleteBlobContainerAsync(_config.storage_projection_container_name);
            }
            catch (Azure.RequestFailedException azex) { if (azex.Status != 404) { throw azex; } }
            try
            {
                await blob_client.DeleteBlobContainerAsync(_config.storage_image_container_name);
            }
            catch (Azure.RequestFailedException azex) { if (azex.Status != 404) { throw azex; } }
        }

        public static  async Task Create()
        {
            BlobServiceClient blob_client = new BlobServiceClient(_config.storage_connection_string);
            await blob_client.CreateBlobContainerAsync(_config.storage_main_container_name, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            await blob_client.CreateBlobContainerAsync(_config.storage_projection_container_name, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            await blob_client.CreateBlobContainerAsync(_config.storage_image_container_name, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
        }

        public static async Task Upload(List<SourceHandout> handouts)
        {
            BlobContainerClient client = new BlobContainerClient(_config.storage_connection_string, _config.storage_main_container_name);

            foreach (SourceHandout handout in handouts)
            {
                Console.WriteLine($"Uploading: {handout.File.Name}");
                using (FileStream stream = System.IO.File.OpenRead(handout.File.FullName))
                {
                    await client.UploadBlobAsync(handout.File.Name, stream);
                }
            }
        }

        public static async Task ApplyMetadata(List<SourceHandout> handouts)
        {
            await MetadataManager.UploadMetadata(handouts);
        }
    }
}
