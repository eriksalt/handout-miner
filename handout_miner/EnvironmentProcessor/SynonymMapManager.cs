using Azure.Search.Documents.Indexes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentProcessor
{
    internal class SynonymMapManager
    {
        AzureConfig _config;

        public SynonymMapManager(AzureConfig config)
        {
            _config = config;  
        }

        public SynonymMap GetSynonymMap()
        {
            return new SynonymMap(
                name: _config.synonym_map_name,
                synonyms: @"Carlyle,carlyle,carlisle,Carlisle"
            );
        }
    }
}
