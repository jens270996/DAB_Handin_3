using DAB_Handin_3.Services;
using System;
using DAB_Handin_3.Models;
using DAB_Handin_3.Services;
using System.Linq;

namespace DAB_Handin_3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var service = new CovidDbService(CovidDatabaseSettings.DatabaseSettings);


            


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
        }
    }
}
