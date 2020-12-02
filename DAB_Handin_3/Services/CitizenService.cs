using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using DAB_Handin_3.Models;

namespace DAB_Handin_3.Services
{
    public class CitizenService
    {
        private readonly IMongoCollection<Citizen> _citizens;

        public CitizenService(ICovidDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _citizens = database.GetCollection<Citizen>(settings.CitizenCollectionName);
        }
        public List<Citizen> Get() => _citizens.Find(ci => true).ToList();
    }
}
