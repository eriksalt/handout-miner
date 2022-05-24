using Azure.Search.Documents.Indexes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentProcessor
{
    internal class IndexerManager
    {
        AzureConfig _config;
        public IndexerManager(AzureConfig config)
        {
            _config = config;
        }

        public SearchIndexer Create()
        {
            return new SearchIndexer(
                name: _config.indexer_name,
                dataSourceName: _config.search_datasource_name,
                targetIndexName: _config.index_name)
            {
                SkillsetName = _config.skillset_name,
                Parameters = new IndexingParameters()
                {
                    BatchSize = 1,
                    MaxFailedItems = 0,
                    MaxFailedItemsPerBatch = 0,
                    Configuration =
                    {
                        ["dataToExtract"] = BlobIndexerDataToExtract.ContentAndMetadata,
                        ["imageAction"] = BlobIndexerImageAction.GenerateNormalizedImages,
                        ["normalizedImageMaxWidth"] = 2000,
                        ["normalizedImageMaxHeight"] = 2000
                    }
                },
                FieldMappings =
                {
                    new FieldMapping(sourceFieldName: "metadata_storage_name")           { TargetFieldName = "fileName"        },
                },
                OutputFieldMappings =
                {
                    new FieldMapping(sourceFieldName: "/document/finalText")                                        { TargetFieldName = "text"                     },
                    new FieldMapping(sourceFieldName: "/document/normalizedPeople")                                 { TargetFieldName = "people"                    },
                    new FieldMapping(sourceFieldName: "/document/normalizedPhrases")                                { TargetFieldName = "phrases"                    },
                    new FieldMapping(sourceFieldName: "/document/normalizedLocations")                              { TargetFieldName = "locations"                },
                    new FieldMapping(sourceFieldName: "/document/normalizedOrganizations")                          { TargetFieldName = "organizations"        },
                    new FieldMapping(sourceFieldName: "/document/normalizedDates")                                  { TargetFieldName = "dates"                },
                    new FieldMapping(sourceFieldName: "/document/normalized_images/*/Tags/*/name")                  {TargetFieldName="imageTags"},
                    new FieldMapping(sourceFieldName: "/document/normalized_images/*/Description/captions/*/text")  {TargetFieldName="imageCaption"},
                    new FieldMapping(sourceFieldName: "/document/normalizedGeolocations")                           {TargetFieldName="geolocations"},
                    new FieldMapping(sourceFieldName: "/document/metadata_storage_path")                            {TargetFieldName="imagelink"},
                    new FieldMapping(sourceFieldName: "/document/hocrData")                                         {TargetFieldName="hocrData"},
                    new FieldMapping(sourceFieldName: "/document/normalized_images/0/width")                        {TargetFieldName="width"},
                    new FieldMapping(sourceFieldName: "/document/blobdescription")                                  {TargetFieldName="blobMetadata"},
                    new FieldMapping(sourceFieldName: "/document/normalized_images/0/height")                       {TargetFieldName="height"}

                }
            };
        }
    }
}
