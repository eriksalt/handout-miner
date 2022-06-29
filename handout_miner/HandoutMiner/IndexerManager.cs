using Azure.Search.Documents.Indexes.Models;
using handout_miner_shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandoutMiner
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
                    new FieldMapping(sourceFieldName: "metadata_storage_name")           { TargetFieldName = SkillFieldNames.IndexFields.FileName        },
                },
                OutputFieldMappings =
                {
                    new FieldMapping(sourceFieldName: "/document/finalText")                                        { TargetFieldName = SkillFieldNames.IndexFields.Text},
                    new FieldMapping(sourceFieldName: "/document/normalizedPeople")                                 { TargetFieldName = SkillFieldNames.IndexFields.People},
                    new FieldMapping(sourceFieldName: "/document/normalizedPhrases")                                { TargetFieldName = SkillFieldNames.IndexFields.Phrases},
                    new FieldMapping(sourceFieldName: "/document/normalizedLocations")                              { TargetFieldName = SkillFieldNames.IndexFields.Locations},
                    new FieldMapping(sourceFieldName: "/document/normalizedDates")                                  { TargetFieldName = SkillFieldNames.IndexFields.Dates},
                    new FieldMapping(sourceFieldName: "/document/normalized_images/*/Tags/*/name")                  {TargetFieldName=SkillFieldNames.IndexFields.ImageTags},
                    new FieldMapping(sourceFieldName: "/document/normalized_images/*/Description/captions/*/text")  {TargetFieldName=SkillFieldNames.IndexFields.ImageCaption},
                    new FieldMapping(sourceFieldName: "/document/normalizedGeolocations")                           {TargetFieldName=SkillFieldNames.IndexFields.Geolocations},
                    new FieldMapping(sourceFieldName: "/document/metadata_storage_path")                            {TargetFieldName=SkillFieldNames.IndexFields.ImageLink},
                    new FieldMapping(sourceFieldName: "/document/hocrData")                                         {TargetFieldName=SkillFieldNames.IndexFields.HOCRData},
                    new FieldMapping(sourceFieldName: "/document/normalized_images/0/width")                        {TargetFieldName=SkillFieldNames.IndexFields.Width},
                    new FieldMapping(sourceFieldName: "/document/blobdescription")                                  {TargetFieldName=SkillFieldNames.IndexFields.BlobMetaData},
                    new FieldMapping(sourceFieldName: "/document/locationSource")                                   {TargetFieldName=SkillFieldNames.IndexFields.LocationSource},
                    new FieldMapping(sourceFieldName: "/document/sessionSource")                                    {TargetFieldName=SkillFieldNames.IndexFields.SessionSource},
                    new FieldMapping(sourceFieldName: "/document/adventure")                                        {TargetFieldName=SkillFieldNames.IndexFields.AdventureSource},
                    new FieldMapping(sourceFieldName: "/document/normalized_images/0/height")                       {TargetFieldName=SkillFieldNames.IndexFields.Height}

                }
            };
        }
    }
}
