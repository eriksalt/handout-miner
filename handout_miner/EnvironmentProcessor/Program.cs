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
            
            bool retVal = Process().GetAwaiter().GetResult();
            System.Console.WriteLine(retVal);


        }

        public static async Task<bool> Process()
        {
            EnvironmentBuildScript script = new EnvironmentBuildScript();

            //await LocationManager.ClearLocationStore();
            //await LocationManager.SetupBannedLocations();
            //await LocationManager.SetupLocationChanges();
       
            
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
            Console.WriteLine("-----------------Complete.");
            Console.WriteLine("PRESS ENTER TO EXIT.");
            Console.ReadLine();
            
            return true;
        }
    }
}
