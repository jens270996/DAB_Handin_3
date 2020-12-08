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
        private readonly IMongoCollection<Municipality> _municipalities;
        private readonly IMongoCollection<TestCenter> _testCenters;
        public CovidDbService(ICovidDatabaseSettings settings)
        {

            var client = new MongoClient(settings.ConnectionString);
            
            var database = client.GetDatabase(settings.DatabaseName);
            
            //create collections
            //if collection doesn't exist, it will be created on first insert
            _citizens = database.GetCollection<Citizen>(settings.CitizenCollectionName);
            _locationDates = database.GetCollection<LocationDate>(settings.LocationDatesCollectionName);
            _locations = database.GetCollection<Location>(settings.LocationCollectionName);
            _municipalities = database.GetCollection<Municipality>(settings.MunicipalityCollectionName);
            _testCenters = database.GetCollection<TestCenter>(settings.TestCenterCollectionName);
        }
        public List<Citizen> GetCitizens() => _citizens.Find(ci => true).ToList();
        public List<LocationDate> GetLocationDates() => _locationDates.Find(ld => true).ToList();
        public List<Municipality> GetMunicipalities() => _municipalities.Find(mu => true).ToList();

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
                    (l.CitizenIDs.Where(c=>c==ID).Any()))
                    .ToList();

                foreach(var loc in possibleLocationDates)
                {
                    foreach(var borger in loc.CitizenIDs)
                    {
                        var c=_citizens.Find(c => c.ID == borger).First();
                        if (!infectedCitizens.Contains(c))
                            infectedCitizens.Add(c);
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
        public void AddTestCenter(TestCenter t)
        {
            _testCenters.InsertOne(t);
            
        }
        public void AddTestCenterManagement(TestManagement m, int centerID)
        {
            var center=_testCenters.Find(t => t.ID == centerID).First();
            center.TestManagement = m;
            _testCenters.ReplaceOne(t => t.ID == centerID, center);
        }

        public void AddLocationDate(int LocationID,Citizen c,DateTime dateTime)
        {
            var locations=_locationDates.Find(c => c.LocID==LocationID&&c.Date.Date==dateTime.Date).ToList();

            if (locations.Any())
            {
                locations.First().CitizenIDs.Add(c.ID);
                _locationDates.ReplaceOne(l => l._id == locations.First()._id, locations.First());
            }
            else
            {
                if(_locations.Find(l=>l.id==LocationID).Any())
                    _locationDates.InsertOne(new LocationDate { LocID = LocationID, Date = dateTime.Date, CitizenIDs = new List<int> { c.ID } });
            }
        }
        public void AddLocation(Location loc)
        {
            if(!(_locations.Find(l=>l.id==loc.id).Any()))
                _locations.InsertOne(loc);
        }
        public void AddMunicipality(Municipality muni)
        {
           
            _municipalities.InsertOne(muni);
        }

        public int GetHighestCitizenID()
        {
            return _citizens.Find(c => true).SortByDescending(c => c.ID).First().ID;
        }


        public void SetUpDatabase()
        {


            //indexes

            //citizen index on ID
            var citizenBuilder = Builders<Citizen>.IndexKeys;
            var citizenIndexModel = new CreateIndexModel<Citizen>(citizenBuilder.Ascending(c => c.ID));

            //
            var locationDatesBuilder = Builders<LocationDate>.IndexKeys;
            var locationDatesIndexModel = new CreateIndexModel<LocationDate>(locationDatesBuilder.Ascending(l => l.Date)
                .Ascending(l => l.CitizenIDs));

            //seed
            var citizens = new List<Citizen>{new Citizen(1, "Henrik", "Hansen", "201209-9154", 11, "either",751),
                new Citizen(2, "Lene", "Mortensen", "230735-2189", 85, "female",751),
                new Citizen(3, "Hans", "Larsen", "100282-0686", 38, "female",751),
                new Citizen(4, "Hans", "Mortensen", "200503-1945", 17, "female",751),
                new Citizen(5, "Lise", "Frederiksen", "151006-5513", 14, "either",751),
                new Citizen(6, "Hans", "Olsen", "020637-9743", 83, "female",751),
                new Citizen(7, "Mette", "Hansen", "250273-3438", 47, "female",751),
                new Citizen(8, "Morten", "Olsen", "160819-2120", 1, "male",751),
                new Citizen(9, "Bob", "Mortensen", "180920-7789", 0, "either",751),
                new Citizen(10, "Lene", "Olsen", "210991-9650", 29, "either",751),
                new Citizen(11, "Alice", "Frederiksen", "180292-3819", 28, "male",751),
                new Citizen(12, "Alice", "Jensen", "071140-6096", 80, "either", 751),
                new Citizen(13, "Alice", "Olsen", "040522-4917", 98, "female", 751),
                new Citizen(14, "Hanne", "Olsen", "210377-8834", 43, "either",751),
                new Citizen(15, "Mette", "Olsen", "140505-8822", 15, "female",751),
                new Citizen(16, "Bob", "Larsen", "161029-0902", 91, "either",751),
                new Citizen(17, "Morten", "Jensen", "250382-2831", 38, "female",751),
                new Citizen(18, "Jens", "Mortensen", "100662-6508", 58, "female", 751),
                new Citizen(19, "Mathias", "Frederiksen", "260633-8511", 87, "either", 751),
                new Citizen(20, "Hanne", "Mortensen", "140944-8894", 76, "male", 751),
                new Citizen(21, "Alice", "Olsen", "150563-4848", 57, "female", 751),
                new Citizen(22, "Jens", "Mortensen", "010833-4669", 87, "female", 751),
                new Citizen(23, "Hanne", "Larsen", "260306-1597", 14, "male", 751),
                new Citizen(24, "Jens", "Larsen", "100354-5700", 66, "female", 751),
                new Citizen(25, "Lene", "Mortensen", "130581-6589", 39, "male", 751),
                new Citizen(26, "Henrik", "Jensen", "030239-2849", 81, "female",101),
                new Citizen(27, "Jens", "Frederiksen", "260533-9722", 87, "either", 101),
                new Citizen(28, "Mette", "Hansen", "140964-3558", 56, "male", 101),
                new Citizen(29, "Alice", "Hansen", "051103-4086", 17, "either", 101),
                new Citizen(30, "Jens", "Olsen", "061062-6328", 58, "male", 101),
                new Citizen(31, "Mette", "Mortensen", "021137-0508", 83, "female", 101),
                new Citizen(32, "Hans", "Hansen", "240877-0805", 43, "either", 101),
                new Citizen(33, "Mathias", "Larsen", "190426-6611", 94, "either", 101),
                new Citizen(34, "Henrik", "Hansen", "081130-0002", 90, "male", 101),
                new Citizen(35, "Morten", "Mortensen", "170551-0100", 69, "female", 101),
                new Citizen(36, "Alice", "Jensen", "240477-4467", 43, "either", 101),
                new Citizen(37, "Lise", "Hansen", "090490-6312", 30, "either", 101),
                new Citizen(38, "Mathias", "Mortensen", "151266-4158", 54, "male", 101),
                new Citizen(39, "Alice", "Frederiksen", "280139-6939", 81, "either", 101),
                new Citizen(40, "Alice", "Mortensen", "080193-9200", 27, "either", 101),
                new Citizen(41, "Mette", "Hansen", "280340-2357", 80, "male", 101),
                new Citizen(42, "Lise", "Larsen", "160527-9487", 93, "male", 101),
                new Citizen(43, "Lise", "Mortensen", "110813-0713", 7, "female", 101),
                new Citizen(44, "Mette", "Frederiksen", "081035-4520", 85, "male", 101),
                new Citizen(45, "Bob", "Hansen", "080915-7389", 5, "female", 101),
                new Citizen(46, "Bob", "Hansen", "220370-4840", 50, "either", 101),
                new Citizen(47, "Hanne", "Jensen", "200975-6615", 45, "either", 101),
                new Citizen(48, "Lise", "Mortensen", "141131-0659", 89, "female", 101),
                new Citizen(49, "Jesper", "Larsen", "131046-3640", 74, "either", 101),
                new Citizen(50, "Lise", "Mortensen", "190391-3178", 29, "either", 101),
                new Citizen(51, "Alice", "Olsen", "120859-8434", 61, "male",740),
                new Citizen(52, "Alice", "Frederiksen", "011063-7581", 57, "male", 740),
                new Citizen(53, "Mathias", "Frederiksen", "250942-2588", 78, "female", 740),
                new Citizen(54, "Henrik", "Frederiksen", "120291-8868", 29, "either", 740),
                new Citizen(55, "Lene", "Hansen", "150518-3294", 2, "female", 740),
                new Citizen(56, "Henrik", "Mortensen", "080314-8040", 6, "female", 740),
                new Citizen(57, "Henrik", "Mortensen", "280875-4848", 45, "female", 740),
                new Citizen(58, "Lene", "Frederiksen", "160141-3010", 79, "male", 740),
                new Citizen(59, "Mette", "Frederiksen", "141219-7378", 1, "female", 740),
                new Citizen(60, "Lene", "Jensen", "130673-2333", 47, "male", 740),
                new Citizen(61, "Henrik", "Larsen", "130660-0632", 60, "either", 740),
                new Citizen(62, "Jesper", "Mortensen", "150580-9963", 40, "either", 740),
                new Citizen(63, "Jesper", "Mortensen", "080121-5419", 99, "female", 740),
                new Citizen(64, "Hanne", "Larsen", "080141-4287", 79, "female", 740),
                new Citizen(65, "Jesper", "Mortensen", "050466-5745", 54, "either", 740),
                new Citizen(66, "Hanne", "Frederiksen", "170312-3999", 8, "female", 740),
                new Citizen(67, "Henrik", "Olsen", "160925-4371", 95, "male", 740),
                new Citizen(68, "Bob", "Jensen", "120904-3356", 16, "male", 740),
                new Citizen(69, "Lise", "Frederiksen", "110802-9350", 18, "either", 740),
                new Citizen(70, "Lise", "Larsen", "050687-4750", 33, "either", 740),
                new Citizen(71, "Lise", "Hansen", "230252-6329", 68, "either", 740),
                new Citizen(72, "Alice", "Olsen", "101249-7038", 71, "either", 740),
                new Citizen(73, "Jesper", "Mortensen", "160343-8991", 77, "male", 740),
                new Citizen(74, "Jens", "Olsen", "170699-2395", 21, "either", 740),
                new Citizen(75, "Bob", "Frederiksen", "091280-7353", 40, "male", 740),
                new Citizen(76, "Jesper", "Mortensen", "160960-4286", 60, "male",851),
                new Citizen(77, "Jesper", "Larsen", "190314-6674", 6, "male", 851),
                new Citizen(78, "Morten", "Olsen", "060761-7447", 59, "male", 851),
                new Citizen(79, "Mette", "Hansen", "190502-8797", 18, "male", 851),
                new Citizen(80, "Mette", "Jensen", "011220-0166", 0, "female", 851),
                new Citizen(81, "Lise", "Larsen", "160619-8717", 1, "female", 851),
                new Citizen(82, "Mathias", "Olsen", "070724-2622", 96, "female", 851),
                new Citizen(83, "Jesper", "Hansen", "111207-6379", 13, "either", 851),
                new Citizen(84, "Bob", "Hansen", "090733-5712", 87, "male", 851),
                new Citizen(85, "Lene", "Jensen", "080859-4024", 61, "female", 851),
                new Citizen(86, "Bob", "Olsen", "031156-2270", 64, "male", 851),
                new Citizen(87, "Bob", "Jensen", "230876-3875", 44, "female", 851),
                new Citizen(88, "Henrik", "Olsen", "280556-3992", 64, "either", 851),
                new Citizen(89, "Lise", "Frederiksen", "260498-6785", 22, "female", 851),
                new Citizen(90, "Jesper", "Hansen", "261123-7154", 97, "male", 851),
                new Citizen(91, "Mette", "Jensen", "040592-9658", 28, "male", 851),
                new Citizen(92, "Lene", "Jensen", "270372-5739", 48, "male", 851),
                new Citizen(93, "Mette", "Jensen", "080192-6842", 28, "either", 851),
                new Citizen(94, "Mette", "Olsen", "170702-3222", 18, "male", 851),
                new Citizen(95, "Henrik", "Larsen", "220931-5049", 89, "male", 851),
                new Citizen(96, "Jesper", "Olsen", "280634-1809", 86, "male", 851),
                new Citizen(97, "Bob", "Frederiksen", "040221-5872", 99, "either", 851),
                new Citizen(98, "Lise", "Olsen", "240156-0145", 64, "either", 851),
                new Citizen(99, "Hanne", "Hansen", "250866-4855", 54, "female", 851),
                new Citizen(100, "Morten", "Larsen", "150103-4148", 17, "male", 851) };

            _municipalities.InsertMany(new List<Municipality>{
                new Municipality(101, "København", 632340),
                 new Municipality(147, "Frederiksberg", 104305),
                new Municipality(151, "Ballerup", 48602),
                new Municipality(153, "Brøndby", 35090),
                new Municipality(155, "Dragør", 14494),
                new Municipality(157, "Gentofte", 74830),
                new Municipality(159, "Gladsaxe", 69262),
                new Municipality(161, "Glostrup", 23128),
                new Municipality(163, "Herlev", 28953),
                new Municipality(165, "Albertslund", 27731),
                new Municipality(167, "Hvidovre", 53527),
                new Municipality(169, "Høje-Taastrup", 50759),
                new Municipality(173, "Lyngby-Taarbæk", 56214),
                new Municipality(175, "Rødovre", 40652),
                new Municipality(183, "Ishøj", 22989),
                new Municipality(185, "Tårnby", 42989),
                new Municipality(187, "Vallensbæk", 16633),
                new Municipality(190, "Furesø", 40965),
                new Municipality(201, "Allerød", 25633),
                new Municipality(210, "Fredensborg", 40865),
                new Municipality(217, "Helsingør", 62695),
                new Municipality(219, "Hillerød", 51183),
                new Municipality(223, "Hørsholm", 24864),
                new Municipality(230, "Rudersdal", 56728),
                new Municipality(240, "Egedal", 43354),
                new Municipality(250, "Frederikssund", 45223),
                new Municipality(253, "Greve", 50558),
                new Municipality(259, "Køge", 60979),
                new Municipality(260, "Halsnæs", 31384),
                new Municipality(265, "Roskilde", 87914),
                new Municipality(269, "Solrød", 23255),
                new Municipality(270, "Gribskov", 41048),
                new Municipality(306, "Odsherred", 32957),
                new Municipality(316, "Holbæk", 71541),
                new Municipality(320, "Faxe", 36576),
                new Municipality(326, "Kalundborg", 48436),
                new Municipality(329, "Ringsted", 34852),
                new Municipality(330, "Slagelse", 79073),
                new Municipality(336, "Stevns", 22805),
                new Municipality(340, "Sorø", 29881),
                new Municipality(350, "Lejre", 27996),
                new Municipality(360, "Lolland", 41105),
                new Municipality(370, "Næstved", 83143),
                new Municipality(376, "Guldborgsund", 60722),
                new Municipality(390, "Vordingborg", 45566),
                new Municipality(400, "Bornholm", 39499),
                new Municipality(410, "Middelfart", 38853),
                new Municipality(411, "Christiansø", 84000),
                new Municipality(420, "Assens", 40965),
                new Municipality(430, "Faaborg-Midtfyn", 51556),
                new Municipality(440, "Kerteminde", 23812),
                new Municipality(450, "Nyborg", 32009),
                new Municipality(461, "Odense", 204895),
                new Municipality(479, "Svendborg", 58296),
                new Municipality(480, "Nordfyns", 29665),
                new Municipality(482, "Langeland", 12491),
                new Municipality(492, "Ærø", 5964),
                new Municipality(510, "Haderslev", 55670),
                new Municipality(530, "Billund", 26608),
                new Municipality(540, "Sønderborg", 74220),
                new Municipality(550, "Tønder", 37366),
                new Municipality(561, "Esbjerg", 115483),
                new Municipality(563, "Fanø", 3488),
                new Municipality(573, "Varde", 49961),
                new Municipality(575, "Vejen", 42742),
                new Municipality(580, "Aabenraa", 58761),
                new Municipality(607, "Fredericia", 51377),
                new Municipality(615, "Horsens", 90966),
                new Municipality(621, "Kolding", 93175),
                new Municipality(630, "Vejle", 115748),
                new Municipality(657, "Herning", 89127),
                new Municipality(661, "Holstebro", 58591),
                new Municipality(665, "Lemvig", 19722),
                new Municipality(671, "Struer", 21036),
                new Municipality(706, "Syddjurs", 42962),
                new Municipality(707, "Norddjurs", 37089),
                new Municipality(710, "Favrskov", 48397),
                new Municipality(727, "Odder", 22844),
                new Municipality(730, "Randers", 97805),
                new Municipality(740, "Silkeborg", 94026),
                new Municipality(741, "Samsø", 3657),
                new Municipality(746, "Skanderborg", 62678),
                new Municipality(751, "Aarhus", 349983),
                new Municipality(756, "Ikast-Brande", 41369),
                new Municipality(760, "Ringkøbing-Skjern", 56594),
                new Municipality(766, "Hedensted", 46722),
                new Municipality(773, "Morsø", 20247),
                new Municipality(779, "Skive", 45851),
                new Municipality(787, "Thisted", 43423),
                new Municipality(791, "Viborg", 96921),
                new Municipality(810, "Brønderslev", 36304),
                new Municipality(813, "Frederikshavn", 59654),
                new Municipality(820, "Vesthimmerlands", 36727),
                new Municipality(825, "Læsø", 1786),
                new Municipality(840, "Rebild", 30113),
                new Municipality(846, "Mariagerfjord", 41800),
                new Municipality(849, "Jammerbugt", 38324),
                new Municipality(851, "Aalborg", 217075),
                new Municipality(860, "Hjørring", 64483)
               });

            _citizens.InsertMany(citizens);

            //citizens.Sort(delegate(Citizen c1,Citizen c2) { return c1.Muni.CompareTo(c2.Muni); });
            //var e = citizens.GetEnumerator();
            var Citizen101 = new List<string>();
            var Citizen740 = new List<string>();
            var Citizen851 = new List<string>();
            var Citizen751 = new List<string>();
            //while (e.MoveNext()&&e.Current.Muni)
            //{

            //}
            foreach (var item in citizens)
            {
                //_municipalities.FindOneAndUpdate()
                //item.Muni
                switch (item.Muni)
                {
                    case 101:
                        Citizen101.Add(item.SSN);
                        break;
                    case 740:
                        Citizen740.Add(item.SSN);
                        break;
                    case 751:
                        Citizen751.Add(item.SSN);
                        break;
                    case 851:
                        Citizen851.Add(item.SSN);
                        break;
                    default:
                        break;
                }
                //item.Muni
            }

            var mun101 = _municipalities.Find(c => c.ID == 101).First();
            mun101.Citizens.AddRange(Citizen101);
            _municipalities.ReplaceOne(m => m.ID == 101, mun101);
            var mun740 = _municipalities.Find(c => c.ID == 740).First();
            mun740.Citizens.AddRange(Citizen740);
            _municipalities.ReplaceOne(m => m.ID == 740, mun740);
            var mun751 = _municipalities.Find(c => c.ID == 751).First();
            mun751.Citizens.AddRange(Citizen751);
            _municipalities.ReplaceOne(m => m.ID == 751, mun751);
            var mun851 = _municipalities.Find(c => c.ID == 851).First();
            mun851.Citizens.AddRange(Citizen851);
            _municipalities.ReplaceOne(m => m.ID == 851, mun851);


        }
       
    }
}
