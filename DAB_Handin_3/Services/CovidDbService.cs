using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using DAB_Handin_3.Models;
using System.Linq;

namespace DAB_Handin_3.Services
{
    public class CovidDbService
    {
        private readonly IMongoCollection<Citizen> _citizens;
        private readonly IMongoCollection<LocationDate> _locationDates;
        private readonly IMongoCollection<Location> _locations;
        public CovidDbService(ICovidDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _citizens = database.GetCollection<Citizen>(settings.CitizenCollectionName);
            _locationDates = database.GetCollection<LocationDate>(settings.LocationDatesCollectionName);
            _locations = database.GetCollection<Location>(settings.LocationCollectionName);

        }
        public List<Citizen> GetCitizens() => _citizens.Find(ci => true).ToList();
        public List<LocationDate> GetLocationDates() => _locationDates.Find(ld => true).ToList();

        public List<Citizen> GetPossibleInfected(int ID)
        {
            List<DateTime> infectedDates = new List<DateTime>();
            List<Citizen> infectedCitizens = new List<Citizen>();
            try
            {
                var cit = _citizens.Find(c => c.ID == ID).First();
                foreach(var test in cit.Tests)
                {
                    if(test.Res=="positive")
                    {
                        for(int i =0;i<4;i++)
                        {
                            infectedDates.Add(test.Date.ToUniversalTime().AddDays(i).Date);
                        }
                    }
                }
                var possibleLocationDates=_locationDates.Find(l => infectedDates.Contains(
                    (l.Date.ToUniversalTime().Date))&&
                    (l.Citizens.Where(c=>c.ID==ID).Any()))
                    .ToList();

                foreach(var loc in possibleLocationDates)
                {
                    foreach(var borger in loc.Citizens)
                    {
                        if (!infectedCitizens.Contains(borger))
                            infectedCitizens.Add(borger);
                    }
                }


            }
            catch(Exception e)
            {
                Console.WriteLine($"BAdBadError: {e}");
            }
            return infectedCitizens;

        }

        public List<Citizen> GetAllCurrentlyInfected()
        {
            List<Citizen> citizens = new List<Citizen>();
            try
            {
                citizens = _citizens.Find(c => c.Tests
                  .Where(t => t.Res == "positiv" &&
                  t.Date.ToUniversalTime().AddDays(14).Date >= DateTime.Now.Date).Any()).ToList();
                
            }
            catch(Exception e)
            {
                Console.WriteLine($"BAdBadError: {e}");
            }
            return citizens;
        }

        public long InfectedInterval(int minAge, int maxAge, string gender)
        {
            List<Citizen> all = GetAllCurrentlyInfected();
            return all.Where(c => c.Age >= minAge && c.Age <= maxAge && gender == c.Sex).Count();
        }

        public void AddCitizen(Citizen c)
        {
            _citizens.InsertOne(c);
        }
        public void AddTest(Test test, int citizenID)
        {
            var citizen = _citizens.Find(ci => true).ToList().Find(c => c.ID == citizenID);
            
            if (citizen!=null)
            {
                citizen.Tests.Add(test);
                _citizens.ReplaceOne(c => c._id == citizen._id, citizen);

            }
        }

        public void AddLocationDate(int LocationID,Citizen c,DateTime dateTime)
        {
            var locations=_locationDates.Find(c => c.LocID==LocationID&&c.Date.Date==dateTime.Date).ToList();

            if (locations.Any())
            {
                locations.First().Citizens.Add(c);
                _locationDates.ReplaceOne(l => l._id == locations.First()._id, locations.First());
            }
            else
            {
                if()
                _locationDates.InsertOne(new LocationDate { LocID = LocationID, Date = dateTime.Date, Citizens = new List<Citizen> { c } });
            }
        }

       
    }
}
