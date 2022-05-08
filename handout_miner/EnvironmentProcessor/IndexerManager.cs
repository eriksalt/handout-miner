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
                    new FieldMapping(sourceFieldName: "/document/finalText/pages/*/people")                         { TargetFieldName = "people"                    },
                    new FieldMapping(sourceFieldName: "/document/finalText/pages/*/locations")                      { TargetFieldName = "locations"                },
                    new FieldMapping(sourceFieldName: "/document/finalText/pages/*/organizations")                  { TargetFieldName = "organizations"        },
                    new FieldMapping(sourceFieldName: "/document/finalText/pages/*/dateTimes")                      { TargetFieldName = "dateTimes"                },
                    new FieldMapping(sourceFieldName: "/document/finalText/pages/*/namedEntities/*/text")           { TargetFieldName = "namedEntities"        },
                    new FieldMapping(sourceFieldName: "/document/finalText/pages/*/entities/*/text")                { TargetFieldName = "entities"                  },
                    new FieldMapping(sourceFieldName: "/document/normalized_images/*/Tags/*/name")                  {TargetFieldName="imageTags"},
                    new FieldMapping(sourceFieldName: "/document/normalized_images/*/Description/captions/*/text")  {TargetFieldName="imageCaption"}
                    

                }
            };
        }
    }
}
