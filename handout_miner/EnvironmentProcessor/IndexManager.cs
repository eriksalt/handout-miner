﻿using Azure.Search.Documents.Indexes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentProcessor
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
                    new SearchField("people", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false },
                    new SearchField("locations", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false },
                    new SearchField("organizations", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false },
                    new SearchField("dateTimes", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false },
                    new SearchField("namedEntities", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false },
                    new SearchField("entities", SearchFieldDataType.Collection(SearchFieldDataType.String)) { IsSearchable = false, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false },
                    new SearchField("text", SearchFieldDataType.String) { IsSearchable = true, IsFilterable = false, IsHidden = false, IsSortable = false, IsFacetable = false, SynonymMapNames = { _config.synonym_map_name } }


                }
            };

            return retval;
        }
    }
}