using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
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
                    new SearchField("id", SearchFieldDataType.String) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = true, IsFacetable = false, IsKey = true },
                    new SearchField("fileName", SearchFieldDataType.String) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false },
                    new SearchField("imageTags", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false },
                    new SearchField("imageCaption", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false },
                    new SearchField("people", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true },
                    new SearchField("locations", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true },
                    new SearchField("phrases", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true },
                    new SearchField("geolocations", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true },
                    new SearchField("blobMetadata", SearchFieldDataType.String) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true },

                    new SearchField("dates", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = true, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true },
                    new SearchField("text", SearchFieldDataType.String) { IsSearchable = true, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false, SynonymMapNames = { _config.synonym_map_name } },
                    new SearchField("imagelink", SearchFieldDataType.String) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false},
                    new SearchField("hocrData", SearchFieldDataType.String) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false},
                    new SearchField("session", SearchFieldDataType.String) { IsSearchable = false, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true},
                    new SearchField("adventure", SearchFieldDataType.String) { IsSearchable = false, IsFilterable = true, IsHidden = false, IsSortable = false, IsFacetable = true},
                    new SearchField("height", SearchFieldDataType.Int64){ IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false},
                    new SearchField("width", SearchFieldDataType.Int64) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false}
                }
            };

            //Make the suggester
            FieldBuilder fieldBuilder = new FieldBuilder();
            var suggester = new SearchSuggester("suggester", new[] { "phrases", "people", "locations", "dates", "imageTags", "imageCaption", "blobMetadata", "geolocations", "session", "adventure" });
            retval.Suggesters.Add(suggester);

            return retval;
        }
    }
}
