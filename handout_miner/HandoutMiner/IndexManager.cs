using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using handout_miner_shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandoutMiner
{
    internal class IndexManager
    {
        AzureConfig _config;
        public IndexManager(AzureConfig config)
        {
            _config = config;  
        }

        public SearchIndex CreateIndex()
        {
            SearchIndex retval = new SearchIndex(name: _config.index_name)
            {
                Fields = new List<SearchField>()
                {
                    new SearchField(SkillFieldNames.IndexFields.ID, SearchFieldDataType.String) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = true, IsFacetable = false, IsKey = true },
                    new SearchField(SkillFieldNames.IndexFields.FileName, SearchFieldDataType.String) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false },
                    new SearchField(SkillFieldNames.IndexFields.ImageTags, SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false },
                    new SearchField(SkillFieldNames.IndexFields.ImageCaption, SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false },
                    new SearchField(SkillFieldNames.IndexFields.People, SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true },
                    new SearchField(SkillFieldNames.IndexFields.Locations, SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true },
                    new SearchField(SkillFieldNames.IndexFields.Phrases, SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true },
                    new SearchField(SkillFieldNames.IndexFields.Geolocations, SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true },
                    new SearchField(SkillFieldNames.IndexFields.BlobMetaData, SearchFieldDataType.String) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true },

                    new SearchField(SkillFieldNames.IndexFields.Dates, SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true },
                    new SearchField(SkillFieldNames.IndexFields.Text, SearchFieldDataType.String) { IsSearchable = true, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false, SynonymMapNames = { _config.synonym_map_name } },
                    new SearchField(SkillFieldNames.IndexFields.ImageLink, SearchFieldDataType.String) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false},
                    new SearchField(SkillFieldNames.IndexFields.HOCRData, SearchFieldDataType.String) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false},
                    
                    new SearchField(SkillFieldNames.IndexFields.LocationSource, SearchFieldDataType.String) { IsSearchable = false, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true},
                    new SearchField(SkillFieldNames.IndexFields.SessionSource, SearchFieldDataType.String) { IsSearchable = false, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true},
                    new SearchField(SkillFieldNames.IndexFields.AdventureSource, SearchFieldDataType.String) { IsSearchable = false, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true},

                    new SearchField(SkillFieldNames.IndexFields.Height, SearchFieldDataType.Int64){ IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false},
                    new SearchField(SkillFieldNames.IndexFields.Width, SearchFieldDataType.Int64) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false}
                    

                }
            };

            //Make the suggester
            FieldBuilder fieldBuilder = new FieldBuilder();
            var suggester = new SearchSuggester("suggester", new[] { "phrases", "people", "locations", "dates", "imageTags", "imageCaption", "blobMetadata", "geolocations" });
            retval.Suggesters.Add(suggester);

            return retval;
        }
    }
}
