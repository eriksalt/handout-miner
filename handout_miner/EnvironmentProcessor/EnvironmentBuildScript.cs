using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Indexes;

namespace EnvironmentProcessor
{
    internal class EnvironmentBuildScript
    {
        AzureConfig _config;
        Dictionary<string, string> _metadata = new Dictionary<string, string>();        

        public EnvironmentBuildScript()
        {
            _config = new AzureConfig();
            _metadata.Add("017.png", "Shanghai");
        }

        public async Task DeleteBlobContainers()
        {
            // Create a BlobServiceClient that will authenticate through Active Directory
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

        public async Task CreateBlobStorageContainers()
        {
            BlobServiceClient blob_client = new BlobServiceClient(_config.storage_connection_string);
            await blob_client.CreateBlobContainerAsync(_config.storage_main_container_name, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            await blob_client.CreateBlobContainerAsync(_config.storage_projection_container_name, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
            await blob_client.CreateBlobContainerAsync(_config.storage_image_container_name, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
        }

        public async Task UploadSourceBlobs()
        {
            BlobContainerClient client = new BlobContainerClient(_config.storage_connection_string, _config.storage_main_container_name);
            

            foreach (string file in System.IO.Directory.EnumerateFiles(_config.source_files_directory))
            {
                
                using (FileStream stream = System.IO.File.OpenRead(file))
                {
                    await client.UploadBlobAsync(System.IO.Path.GetFileName(file), stream);
                }
            }
        }

        public async Task UpdateBlobMetadata()
        {
            foreach (string file in System.IO.Directory.EnumerateFiles(_config.source_files_directory))
            {
                string filename = System.IO.Path.GetFileName(file);
                BlobClient client = new BlobClient(_config.storage_connection_string, _config.storage_main_container_name, filename);
                Dictionary<string, string> meta= new Dictionary<string, string>();
                if (_metadata.ContainsKey(filename))
                    meta.Add(_config.blob_metadata_name, $".{_metadata[filename]}.");
                else 
                    meta.Add(_config.blob_metadata_name, "."); 
                await client.SetMetadataAsync(meta);
            }
        }

        public async Task CleanSearchEnvironment()
        {
            SearchIndexerClient indexer_client = CreateIndexerClient();
            SearchIndexClient index_client = CreateIndexClient();
            
            await indexer_client.DeleteDataSourceConnectionAsync(_config.search_datasource_name);
            await index_client.DeleteIndexAsync(_config.index_name);
            await indexer_client.DeleteIndexerAsync(_config.indexer_name);
            await indexer_client.DeleteSkillsetAsync(_config.skillset_name);
            await index_client.DeleteSynonymMapAsync(_config.synonym_map_name);
            
            
        }

        public async Task SetupSearchEnvironment()
        {
            SearchIndexerClient indexer_client = CreateIndexerClient();
            SearchIndexClient index_client = CreateIndexClient();

            await CreateDataSourceConnection(indexer_client);
            await CreateSkillSet(indexer_client);
            await CreateSynonymMap(index_client);
            await CreateIndex(index_client);
            await CreateIndexer(indexer_client);
        }

        private async Task CreateIndexer(SearchIndexerClient indexer_client)
        {
            IndexerManager mgr = new IndexerManager(_config);
            SearchIndexer indexer = mgr.Create();
            await indexer_client.CreateIndexerAsync(indexer);
        }

        private async Task CreateSkillSet(SearchIndexerClient indexer_client)
        {
            SkillsetManager skill_mgr = new SkillsetManager(_config);
            SearchIndexerSkillset skillset = skill_mgr.GetSkillset();
            await indexer_client.CreateSkillsetAsync(skillset);
        }

        private async Task CreateIndex(SearchIndexClient index_client)
        {
            IndexManager mgr = new IndexManager(_config);
            SearchIndex index = mgr.CreateIndex();
            await index_client.CreateIndexAsync(index);
        }

        private async Task CreateDataSourceConnection(SearchIndexerClient indexer_client)
        {
            SearchIndexerDataSourceConnection datasource = new SearchIndexerDataSourceConnection(
                name: _config.search_datasource_name,
                type: SearchIndexerDataSourceType.AzureBlob,
                connectionString: _config.storage_connection_string,
                container: new SearchIndexerDataContainer(_config.storage_main_container_name))
            {
                Description = "Data source for handout_miner"
            };
            await indexer_client.CreateDataSourceConnectionAsync(datasource);
        }

        private async Task CreateSynonymMap(SearchIndexClient index_client)
        {
            SynonymMapManager map_mgr = new SynonymMapManager(_config);
            SynonymMap map = map_mgr.GetSynonymMap();
            await index_client.CreateSynonymMapAsync(map);
        }

        public SearchIndexClient CreateIndexClient() { return new SearchIndexClient(_config.search_service_endpoint, new Azure.AzureKeyCredential(_config.search_key)); }
        public SearchIndexerClient CreateIndexerClient() { return new SearchIndexerClient(_config.search_service_endpoint, new Azure.AzureKeyCredential(_config.search_key)); }
    }
}
