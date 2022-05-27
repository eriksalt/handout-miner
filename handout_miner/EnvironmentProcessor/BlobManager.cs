using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentProcessor
{
    internal static class BlobManager
    {
        private static AzureConfig _config = new AzureConfig();
        
        public static  async Task Clear()
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

        public static async Task Upload()
        {
            BlobContainerClient client = new BlobContainerClient(_config.storage_connection_string, _config.storage_main_container_name);

            foreach (string file in System.IO.Directory.EnumerateFiles(_config.source_files_directory))
            {
                Console.WriteLine($"Uploading: {file}");
                using (FileStream stream = System.IO.File.OpenRead(file))
                {
                    await client.UploadBlobAsync(System.IO.Path.GetFileName(file), stream);
                }
            }
        }

        public static async Task ApplyMetadata()
        {
            await MetadataManager.UploadMetadata();
        }
    }
}
