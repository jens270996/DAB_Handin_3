using DAB_Handin_3.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using DAB_HANDIN_3;
using DAB_Handin_3.Models;
using DAB_Handin_3.Services;
using System.Linq;
using System.Runtime.InteropServices;

namespace DAB_Handin_3
{
    class Program
    {
        static void Main(string[] args)
        {

            var service = new CovidDbService(CovidDatabaseSettings.DatabaseSettings);

            StatisticsView statView = new StatisticsView();
            CreateView createView = new CreateView();
            //var service = new CitizenService(CovidDatabaseSettings.DatabaseSettings);
            var citizens=service.GetCitizens();
            int id=citizens.First().ID;
            service.AddTest(new Test { Date = DateTime.Now, Res = "pos", Status = "Fine" },id);
            citizens = service.GetCitizens();
            foreach(var cit in citizens)
            {

                Console.WriteLine($"Citizen:\n  Name: {cit.FirstName}, Muni: {cit.Muni}");
                Console.WriteLine("Tests:");
                foreach(var test in cit.Tests)
                {
                    Console.WriteLine($"Test result: {test.Res}, Test date: {test.Date}");
                }
            }

            bool finish = false;

            do
            {
                Console.Clear();
                Console.WriteLine("***** Velkommen til Covid19 tracking app ***** \n" +
                                  "\n Følgende muligheder er tilgængelige: \n - Exit \n - Vis antal aktive Covid19 patienter per kommune" +
                                  "\n - Se deataljeret statistik over smittede baseret på aldersgruppe og køn \n - Udregn mulige smittede ved nyeste smittetilfælde" +
                                  "\n - Tilføj nyt smittetilfælde, testcenter, testsag eller ny lokation" +
                                  "\n ");
                Console.WriteLine(" Indtast et af de følgende bogstaver for at åbne en mulighed: \n E = exit " +
                                  "\n A = Aktive pr. kommune \n S = Åben statistik \n U = Mulige nye smittede \n N = Tilføj data ");
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                switch (input[0])
                {
                    case 'E':
                        finish = true; // exit
                        break;

                    case 'A':
                        //Calculate the number of active Covid19 cases - a person is infected 14 days after a positive
                        //test. Results should be shown per Municipality.
                        var totalInfected = service.GetAllCurrentlyInfected();
                        Console.WriteLine("Total antal smittede: {0}", totalInfected.Count);
                        
                        // udskriv per municipality
                        Dictionary<int, int> muniDictionary = new Dictionary<int, int>();
                        foreach (var cit in totalInfected)
                        {
                            bool added = muniDictionary.TryAdd(cit.Muni, 1);
                            if (!added)
                            {
                                muniDictionary.TryGetValue(cit.Muni, out var currentCount);
                                muniDictionary[cit.Muni] = currentCount + 1;
                            }
                        }

                        Console.WriteLine("\n Kommune:             Antal smittede:");
                        foreach (KeyValuePair<int, int> kvp in muniDictionary)
                        {
                            Console.WriteLine(" {0}, {1} ", kvp.Key, kvp.Value);
                        }
                        Console.WriteLine("Tryk på en knap for at vælge en ny mulighed");
                        Console.ReadKey();
                        break;

                    case 'S':
                        // åben stat menu
                        statView.OpenStatMenu();
                        break;

                    case 'U':
                        //Given a new infected citizen, “calculate” which other citizen may be infected .
                        Console.WriteLine("Indtast id på smittet person.");
                        string citId = Console.ReadLine();
                        if (string.IsNullOrEmpty(citId)) continue;
                        var possibleInfectedList =  service.GetPossibleInfected(int.Parse(citId));
                        
                        Console.WriteLine("\n Muligt smittede borgere: ");
                       
                        foreach (var cit in possibleInfectedList)
                        {
                            Console.WriteLine($"Test result: {cit.FirstName}, Test date: {cit.LastName}");
                        }
                        Console.WriteLine("Tryk på en knap for at vælge en ny mulighed");
                        Console.ReadKey();
                        break;

                    case 'N':
                        //åben create menu
                        createView.OpenCreateMenu();
                        break;

                    default:
                        break;
                }

            } while (!finish);

        }
    }
}
