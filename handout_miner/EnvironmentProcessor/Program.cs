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

            await script.CleanSearchEnvironment();
            //await script.DeleteBlobContainers();

            //System.Threading.Thread.Sleep(10000);

            //await script.CreateBlobStorageContainers();
            //await script.UploadSourceBlobs();

            //await script.UpdateBlobMetadata();

            //System.Threading.Thread.Sleep(4000);
            
            await script.SetupSearchEnvironment();
            return true;
        }
    }
}
