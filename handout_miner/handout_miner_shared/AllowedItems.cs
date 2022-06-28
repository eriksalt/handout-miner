using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandoutMiner.Shared {
    public abstract class AllowedItems {
        protected int MaxDistance { get; set; } = 1;
        public AllowedItems() {
            Initialize();
        }
        public Dictionary<string, string> Aliases { get; set; } =  new Dictionary<string, string>();
        protected abstract void Initialize();
        public string AllowedString(string input) {
            foreach (string alias in Aliases.Keys) {
                int distance = TextDistance.Calculate(input, alias);
                if (distance <= MaxDistance)
                    return Aliases[alias];
            }
            return String.Empty;
        }
    }
}
