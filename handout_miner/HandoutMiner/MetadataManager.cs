using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandoutMiner
{
    public static class MetadataManager
    {
        private static Dictionary<string, string> _metadata = new Dictionary<string, string>();
        private static AzureConfig _config = new AzureConfig();

        private static void LoadMetadata()
        {
            _metadata.Clear();
            _metadata.Add("017.png", ".Shanghai, China.");
        }
        public static async Task UploadMetadata()
        {
            LoadMetadata();
            foreach (string file in System.IO.Directory.EnumerateFiles(_config.source_files_directory))
            {
                string filename = System.IO.Path.GetFileName(file);
                BlobClient client = new BlobClient(_config.storage_connection_string, _config.storage_main_container_name, filename);
                Dictionary<string, string> meta = new Dictionary<string, string>();
                if (_metadata.ContainsKey(filename))
                {
                    meta.Add(_config.blob_metadata_name, $"{_metadata[filename]}");
                    Console.WriteLine($"Metadata:{filename}=>{_metadata[filename]}");
                }
                else
                {
                    meta.Add(_config.blob_metadata_name, "_");
                    Console.WriteLine($"Metadata:{filename}=>_");
                }
                await client.SetMetadataAsync(meta);
            }
        }
    }
}
