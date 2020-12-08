﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAB_Handin_3.Models;
using DAB_Handin_3.Services;


namespace DAB_HANDIN_3
{
    public class CreateView
    {
        private CovidDbService service;
        private int currentCitId;
        private int currentTestCenterId;
        public CreateView()
        {
            service = new CovidDbService(CovidDatabaseSettings.DatabaseSettings);
            currentCitId = service.GetHighestCitizenID();
        }

        private void AddCitizen()
        {
            List<Municipality> muniList = service.GetMunicipalities();
           
            // tilføj ny borger
            Console.WriteLine("Indtast Navn på borgers kommune:");
            var muni = Console.ReadLine();
            if (muniList.Any(m => m.Name == muni))
            {
                // SSN - FirstN - LastN - Age - Sex - Muni -ID
                Console.WriteLine("Indtast SSN, fornavn, efternavn, alder, køn: \"ssn fornavn efternavn alder køn\"");
                var tokens = Console.ReadLine().Split(" ");
                int val;
                if (tokens.Length == 5 && int.TryParse(tokens[1], out val))
                {
                    currentCitId++;
                    Municipality municipality = muniList.Find(m => m.Name == muni);
                    int munId = municipality.ID; 
                    service.AddCitizen(new Citizen {ID = currentCitId, FirstName = tokens[1], LastName = tokens[2], SSN = tokens[0], Age = int.Parse(tokens[3]), Sex = tokens[4], Muni = munId});
                }
                else
                {
                    Console.WriteLine("Ugyldig data.");
                    Console.WriteLine("Tryk på en knap for at vælge en ny mulighed");
                    Console.ReadKey();
                }
            }
            else
            {
                Console.WriteLine("Ugyldig kommunenavn.");
                Console.WriteLine("Tryk på en knap for at vælge en ny mulighed");
                Console.ReadKey();
            }
        }

        private void AddTestCenter()
        {
            //tilføj testcenter
            string[] hoursarr;
            int open;
            int close;
            do
            {
                Console.WriteLine("Indtast Test center åbningstider: \"open close\"");
                string hours = Console.ReadLine();
                hoursarr = hours.Split(" ");
            }
            while (hoursarr.Length != 2 || !int.TryParse(hoursarr[0], out open) || !int.TryParse(hoursarr[1], out close));

            Console.WriteLine("Indtast Test center Navn og kommune: \"navn kommune\"");
            var tokens = Console.ReadLine().Split(" ");
            currentTestCenterId++;
            service.AddTestCenter(new TestCenter
            {
                CloseHour = close, OpenHour = open, Name = tokens[1],
                Muni = tokens[2], ID = currentTestCenterId
            });
        }

        private void AddManagment()
        {
            //tilføj Testledelse
            Console.WriteLine("Skriv navn på testcenter der ledes:");
            var name = Console.ReadLine();
            Console.WriteLine("Indtast telefon nr. og email: \"tlf email\"");
            string[] res = Console.ReadLine().Split(" ");
            var testCenters = service.GetTestCenters();
            if (testCenters.Any(m => m.Name == name))
            {
                TestCenter testCenter;
                testCenter = testCenters.Find(t => t.Name == name);
                service.AddTestCenterManagement( new TestManagement{Phone = res[0], Email = res[1]}, testCenter.ID);
            }
            else
            {
                Console.WriteLine("Ugyldig Center navn.");
                Console.WriteLine("Tryk på en knap for at vælge en ny mulighed");
                Console.ReadKey();
            }
        }

        private void AddTestResult()
        {
            //tilføj testresultat
            Console.WriteLine("Indtast oplysninger om resultat: \"BorgerID centerID resultat(p/n) status");
            var tokens = Console.ReadLine().Split(" ");
            int borgerid;
            int centerid;
            Citizen cit = null;
            TestCenter cent = null;
            if (int.TryParse(tokens[0], out borgerid) && int.TryParse(tokens[1], out centerid))
            {
                cit = new UnitOfWork(new CovidContext()).Citizens.Find(c => c.ID == borgerid).First();
                cent = new UnitOfWork(new CovidContext()).TestCenters.Find(c => c.TestCenterId == centerid).First();

                if (cit.ID == borgerid && cent.TestCenterId == centerid)
                {
                    bool pos = false;
                    if (tokens[2] == "p")
                        pos = true;
                    using (var unitOfWork = new UnitOfWork(new CovidContext()))
                    {
                        TestDate test = new TestDate()
                        {
                            TestCenterID = centerid
                            ,
                            Citizen_ID = borgerid
                            ,
                            Date = DateTime.Now
                            ,
                            Status = tokens[3]
                            ,
                            Result = pos
                        };

                        unitOfWork.TestDates.Add(test);
                        unitOfWork.Complete();
                    }
                }
            }
        }

        private void AddLoaction()
        {

            // tilføj lokation
            Console.WriteLine("Indtast Navn på borgers kommune:");
            var muni = Console.ReadLine();
            var mun = new UnitOfWork(new CovidContext()).Municipalities.Find(c => c.Name == muni).First();
            if (mun.Name == muni)
            {
                Console.WriteLine("Indtast adressen på den nye lokation");
                string address = Console.ReadLine();
                using (var unitOfWork = new UnitOfWork(new CovidContext()))
                {
                    if (address != null)
                    {
                        Location location = new Location(address);
                        unitOfWork.Locations.Add(location);
                        unitOfWork.Complete();
                    }
                    else
                    {
                        Console.WriteLine("Ugyldig adresse.");
                        Console.WriteLine("Tryk på en knap for at vælge en ny mulighed");
                        Console.ReadKey();
                    }
                }
            }
            else
            {
                Console.WriteLine("Ugyldigt kommunenavn.");
                Console.WriteLine("Tryk på en knap for at vælge en ny mulighed");
                Console.ReadKey();
            }
        }

        public void OpenCreateMenu()
        {
            bool finish = false;

            do
            {
                Console.Clear();
                Console.WriteLine("***** Tilføj data menu ***** \n");
                Console.WriteLine("Vælg en af følgende muligheder: \n - Tilføj en ny borger \n - Tilføj et nyt testcenter med ledelse" +
                                  "\n - Tilføj nyt testresultat \n - Tilføj ny lokation ");
                Console.WriteLine("\n Indtast et af de følgende bogstaver for at åbne en mulighed: \n T = Tilbage " +
                                  "\n B = Tilføj borger \n C = Tilføj testcenter \n O = Tilføj testledelse \n R = Tilføj testresultat \n L = Tilføj lokation ");

                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                switch (input[0])
                {
                    case 'T':
                        finish = true; // exit
                        break;

                    case 'B':
                        AddCitizen();
                        break;

                    case 'C':
                        AddTestCenter();
                        break;

                    case 'O':
                        AddManagment();
                        break;

                    case 'R':
                        AddTestResult();
                        break;

                    case 'L':
                        AddLoaction();
                        break;

                    default:
                        break;
                }

            } while (!finish);
        }
    }
}