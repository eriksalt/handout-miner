using handout_miner_shared;
using HandoutMiner.Shared;

namespace HandoutMiner
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AllowedLocations tmp = new AllowedLocations();//this is here to check for errors in loading locations
            AllowedPeople tmpb = new AllowedPeople();//this is here to check for errors in loading people
            AllowedPhrases tmpc = new AllowedPhrases();//this is here to check for errors in loading phrases
            AzureConfig config = new AzureConfig();
            List<SourceHandout> handouts = SourceHandoutManager.CollectHandouts(config.source_files_directory).OrderBy(handout=>handout.File.FullName).ToList();
            
            //ProcessAdjustments().GetAwaiter().GetResult();
            //ProcessBlobStorage(handouts).GetAwaiter().GetResult();
            ProcessSearch().GetAwaiter().GetResult();
        }

        private static async Task ProcessBlobStorage(List<SourceHandout> handouts)
        {
            await ProcessStep("Clean Blob Storage", 75000, () => BlobManager.Clean());
            await ProcessStep("Create Blob Storage", 4000, () => BlobManager.Create());
            await ProcessStep("Upload Blob Storage", 4000, () => BlobManager.Upload(handouts)); 
            await ProcessStep("Apply Metadata", 0, () => BlobManager.ApplyMetadata(handouts));
        }
        private static async Task ProcessSearch()
        {
            await ProcessStep("Clean Azure Search Index", 4000, () => SearchManager.Clean());
            await ProcessStep("Create Azure Search Index", 0, () => SearchManager.Create());
        }        
        
        private static async Task ProcessAdjustments()
        {
            AdjustmentsManager.Initialize();
            await ProcessStep("Clean Adjustments", 4000, async () => { AdjustmentsManager.Clean(); await Task.CompletedTask; });
            await ProcessStep("Create Adjustments", 12000, async () => { AdjustmentsManager.Create(); await Task.CompletedTask; });
            await ProcessStep("Upload Adjustments", 90000, async () => { AdjustmentsManager.Upload(); await Task.CompletedTask; });
        }

        public static async Task Wait(int time)
        {
            int waitgroupinterval = 10000;
            int waitgroups = time / waitgroupinterval;
            int leftover = time % waitgroupinterval;
            for (int i = 0; i < waitgroups; i++)
            {
                Console.Write(".");
                System.Threading.Thread.Sleep(waitgroupinterval);
            }
            Console.Write(".");
            System.Threading.Thread.Sleep(leftover);
            Console.WriteLine(".");
            await Task.CompletedTask;
        }

        public static async Task ProcessStep(string stepName, int sleeptime, Func<Task> action)
        {
            DateTime startTime = DateTime.Now;
            Console.WriteLine($"{startTime.ToLongTimeString()} - Starting: {stepName}");
            Task task = action.Invoke();
            await task;
            DateTime endTime = DateTime.Now;
            var diff = endTime - startTime;
            double secDiff = diff.TotalSeconds;
            Console.WriteLine($"{startTime.ToLongTimeString()} - Finished {stepName} in {secDiff} seconds. Waiting {((double)sleeptime) / 1000.0} seconds...");
            await Wait(sleeptime);
        }
    }
}