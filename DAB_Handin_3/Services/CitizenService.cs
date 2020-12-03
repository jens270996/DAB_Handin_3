﻿using MongoDB.Driver;
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

        public CovidDbService(ICovidDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _citizens = database.GetCollection<Citizen>(settings.CitizenCollectionName);
            _locationDates = database.GetCollection<LocationDate>(settings.LocationDatesCollectionName);

        }
        public List<Citizen> Get() => _citizens.Find(ci => true).ToList();


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
    }
}
