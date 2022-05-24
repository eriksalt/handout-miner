using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentProcessor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Process().GetAwaiter().GetResult();
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
        public static async Task Process()
        {
            EnvironmentBuildScript script = new EnvironmentBuildScript();

            
            await ProcessStep("Waiting 5 seconds in case process was started in error", 0, async () => await Wait(5000));
            await ProcessStep("Clear Storage Containers", 75000, async () => await script.DeleteBlobContainers());
            await ProcessStep("Create Storage Containers", 10000, async () => await script.CreateBlobStorageContainers());
            await ProcessStep("Upload Storage", 5000, async () => await script.UploadSourceBlobs());
            await ProcessStep("Upload Blob Metadata", 3000, async () => await script.UpdateBlobMetadata());
            await ProcessStep("Clean Search Index", 4000, async () => await script.CleanSearchEnvironment());
            await ProcessStep("Setup Search Index", 1000, async () => await script.SetupSearchEnvironment());
            Console.WriteLine("-----------------Complete.");
            Console.WriteLine("PRESS ENTER TO EXIT.");
            Console.ReadLine();
            /*
            //await LocationManager.ClearLocationStore();
            //await LocationManager.SetupBannedLocations();
            //await LocationManager.SetupLocationChanges();

            await ProcessStep("create datestore", 1000, async () => await DateManager.CreateDateStorage());
            await ProcessStep("clear datestore", 1000, async () => await DateManager.ClearDateStorage());
            await ProcessStep("load datestore", 1000, async () => await DateManager.UploadDateStorage());


            Console.WriteLine("Deleting Blob Containers");
            await script.DeleteBlobContainers();
            Console.WriteLine("--complete. waiting for azure to proces");
            System.Threading.Thread.Sleep(75000);

            Console.WriteLine("Creating Blob Containers");
            await script.CreateBlobStorageContainers();
            Console.WriteLine("--complete. waiting for azure to proces");
            System.Threading.Thread.Sleep(15000);

            Console.WriteLine("Uploading Blobs");
            await script.UploadSourceBlobs();
            Console.WriteLine("--complete. waiting for azure to proces");
            System.Threading.Thread.Sleep(9000);

            Console.WriteLine("Updating Blob Metadata");
            await script.UpdateBlobMetadata();
            Console.WriteLine("--complete. waiting for azure to proces");
            System.Threading.Thread.Sleep(3000);
            
            Console.WriteLine("Cleaning up old search index");
            await script.CleanSearchEnvironment();
            Console.WriteLine("--complete.  Waiting for azure to process");
            System.Threading.Thread.Sleep(4000);
            Console.WriteLine("Setting up indexer");
            await script.SetupSearchEnvironment();
            */

        }
    }
}
