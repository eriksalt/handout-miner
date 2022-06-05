using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace handout_miner_shared
{
    public class SourceHandout
    {
        public SourceHandout() { }
        public string Adventure { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; } = DateTime.MinValue;

        [JsonIgnore]
        public FileInfo File { get; set; } = default!;
        public string Description { get; set; } = string.Empty;

    }
}
