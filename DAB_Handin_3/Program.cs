using DAB_Handin_3.Services;
using System;
using DAB_Handin_3.Models;
using DAB_Handin_3.Services;

namespace DAB_Handin_3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var service = new CitizenService(CovidDatabaseSettings.DatabaseSettings);
            var citizens=service.Get();

            foreach(var cit in citizens)
            {
                Console.WriteLine($"Citizen:\n  Name: {cit.FirstName}, Muni: {cit.Muni}");
            }
        }
    }
}
