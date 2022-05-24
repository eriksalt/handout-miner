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
            StringBuilder bldr = new StringBuilder();
            bldr.AppendLine("Carlyle,carlyle,carlisle,Carlisle");
            bldr.AppendLine("aberdare,aberdare forest");
            bldr.AppendLine("aberdare,aberdare forest, aberdare national park");
            bldr.AppendLine("cambridge,cambridge, massachusetts");
            bldr.AppendLine("collingswood,collingswood house");
            bldr.AppendLine("el peru,peru");
            bldr.AppendLine("hotel chelsea,chelsea hotel");
            bldr.AppendLine("manhattan, n.y,manhtattan");
            bldr.AppendLine("n.y,new york city,nyc,n.y.c,new york");
            bldr.AppendLine("nairobi dear janak,nairobi");
            bldr.AppendLine("stratford,stratford, connecticut");
            return new SynonymMap(
                name: _config.synonym_map_name,
                synonyms: bldr.ToString()
            );
        }
    }
}