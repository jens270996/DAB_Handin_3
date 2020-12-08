using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DAB_Handin_3.Models
{

    public class CovidDatabaseSettings : ICovidDatabaseSettings
    {
        private static CovidDatabaseSettings _settings = new CovidDatabaseSettings(@"appsettings.json");
        public static CovidDatabaseSettings DatabaseSettings
        {
            get
            {
                return _settings;
            }
        }
        private CovidDatabaseSettings()
        {

        }
        private CovidDatabaseSettings(string s)
        {
            using (StreamReader r = new StreamReader(s))
            {
                string json = r.ReadToEnd();
                ICovidDatabaseSettings settings = JsonConvert.DeserializeObject<CovidDatabaseSettings>(json);

                LocationDatesCollectionName = settings.LocationDatesCollectionName;
                    CitizenCollectionName = settings.CitizenCollectionName;
                LocationCollectionName = settings.LocationCollectionName;
                MunicipalityCollectionName = settings.MunicipalityCollectionName;
                TestCenterCollectionName = settings.TestCenterCollectionName;
                    ConnectionString = settings.ConnectionString;
                    DatabaseName = settings.DatabaseName;
                
                
            }


        }
        public string LocationCollectionName { get; set; }
        public string CitizenCollectionName { get; set; }
        public string TestCenterCollectionName { get; set; }
        public string LocationDatesCollectionName { get; set; }
        public string MunicipalityCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface ICovidDatabaseSettings
    {
        public string CitizenCollectionName { get; set; }
        public string LocationDatesCollectionName { get; set; }
        public string TestCenterCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string LocationCollectionName { get; set; }
        public string MunicipalityCollectionName { get; set; }
    }

}
