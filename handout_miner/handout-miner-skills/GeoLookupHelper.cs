using Geocoding;
using Geocoding.Google;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace handout_miner_skills {
    public class GeoLookupHelper {
        private static GeoLookupHelper instance = null!;
        private IGeocoder _geocoder = null!;
        
        private GeoLookupHelper() { }
        public static GeoLookupHelper Instance{
            get{
                if (instance == null) {
                    instance = new GeoLookupHelper();
                    instance.Initialize();
                }
                return instance;
            }
}

        private void Initialize() {
            string cred_key = "gcloud_map_key";
            string cred = Environment.GetEnvironmentVariable(cred_key);
            _geocoder = new GoogleGeocoder() { ApiKey = cred };

        }
        public async Task<string> Lookup(ILogger log, string address) {
            List<Address> addresses = (await _geocoder.GeocodeAsync(address)).ToList(); ;
            if (addresses.Count == 0) return String.Empty;
            Address place = addresses[0];
            double lat = place.Coordinates.Latitude;
            double lon = place.Coordinates.Longitude;

            string normalizedAddress = address.Replace(',', ' ');
            normalizedAddress = address.Replace('"', ' ');
            normalizedAddress = address.Replace('\'', ' ');
            log.LogInformation($"{address}=>{normalizedAddress}");

            log.LogInformation($"Adding new GPS coordinates {lat},{lon} for {normalizedAddress}");
            return $"{normalizedAddress}|{lat}|{lon}";
        }


    }
}
