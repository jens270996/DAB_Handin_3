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
        public CreateView()
        {
            service = new CovidDbService(CovidDatabaseSettings.DatabaseSettings);
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
                    service.AddCitizen(new Citizen { SSN = tokens[1], FirstName = tokens[2], LastName = tokens[3], Age = int.Parse(tokens[4]), 
                                                        Sex = tokens[5], Muni = muni, ID = });
                    // jens' ID funktion over
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
            // Name - ID - Openhour - CloseHour - Muni
            service.AddTestCenter(new TestCenter
            {
                CloseHour = close, OpenHour = open, Name = tokens[1],
                Muni = tokens[2], ID = 
            });
            //jens ID func over

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
                TestCenter testCenter = testCenters.Where(m => m.Name == name).First();
                service.AddTestCenterManagement( new TestManagement{Phone = res[0], Email = res[1]}, );
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
                cit = service.GetCitizens().Find(c => c.ID == borgerid);

                cent = service.GetTestCenters().Find(c => c.ID == centerid);
                    

                if (cit.ID == borgerid && cent.ID == centerid)
                {
                    string pos ="neg";
                    if (tokens[2] == "p")
                        pos = "pos";

                    Test test = new Test()
                    {
                        TC = cent.Name,
                        Res = pos,
                        Date = DateTime.Now,
                        Status = tokens[3]
                    };

                    service.AddTest(test,cit.ID);
                        
                    }
                }
            }
        }

        private void AddLocation()
        {

            // tilføj lokation
            Console.WriteLine("Indtast Navn på borgers kommune:");
            var muni = Console.ReadLine();
            var mun = service.Get
            .Municipalities.Find(c => c.Name == muni).First();
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