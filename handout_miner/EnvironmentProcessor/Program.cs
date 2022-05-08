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
            
            System.Threading.Thread.Sleep(10000);

            //await script.CreateBlobStorageContainers();
            //System.Threading.Thread.Sleep(10000);
            //await script.UploadSourceBlobs();

            await script.SetupSearchEnvironment();
            return true;
        }
    }
}
