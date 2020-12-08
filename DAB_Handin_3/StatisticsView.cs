using System;
using DAB_Handin_3.Models;
using DAB_Handin_3.Services;

namespace DAB_HANDIN_3

{
    public class StatisticsView
    {
        public StatisticsView()
        {
            AllBoolsFalse();
            smittede = 0;
        }

        bool finish;
        double smittede;
        bool kvinder;
        bool mænd;
        bool andre;
        bool et;
        bool elleve;
        bool enogtyve;
        bool enogtredive;
        bool enogfyrre;
        bool enoghalvtreds;
        bool enogtres;
        bool enoghalvfjers;
        bool enogfirs;

        public void AllBoolsFalse()
        {
            kvinder = false;
            mænd = false;
            andre = false;
            et = false;
            elleve = false;
            enogtyve = false;
            enogtredive = false;
            enogfyrre = false;
            enoghalvtreds = false;
            enogtres = false;
            enoghalvfjers = false;
            enogfirs = false;
        }

        public void SetInfectedStat(ref bool _bool, int minAge, int maxAge, string gender)
        {
            var service = new CovidDbService(CovidDatabaseSettings.DatabaseSettings);
            var numberOfInfected = service.InfectedInterval(minAge, maxAge, gender);
            _bool = !_bool;
            if (_bool)
            {
                AllBoolsFalse();
                _bool = !_bool;
                smittede = numberOfInfected;
            }
            else
            { 
                AllBoolsFalse();
                smittede = 0;
            }
        }


        public void OpenStatMenu()
        {
            do
            {
                Console.Clear();
                Console.WriteLine("***** Statistik menu ***** \n");
                Console.WriteLine("Mulige tilvalg: \n Kvinder    [{0}]\n Mænd       [{1}]  \n Andre køn  [{2}] \n År 0-10    [{3}] \n År 11-20   [{4}] \n År 21-30   [{5}] " +
                                  "\n År 31-40   [{6}] \n År 41-50   [{7}] \n År 51-60   [{8}] \n År 61-70   [{9}] \n År 71-80   [{10}] \n År 81+     [{11}]"
                                  , kvinder, mænd, andre, et, elleve, enogtyve, enogtredive, enogfyrre, enoghalvtreds, enogtres, enoghalvfjers, enogfirs);
                Console.WriteLine("\n Antal smittede: {0}", smittede);
                Console.WriteLine("\n Brug de følgende muligheder for at slå en mulighed til eller fra:" +
                                  "\n K = Kvinder " +
                                  "\n M = Mænd" +
                                  "\n O = Andre køn" +
                                  "\n A = 0-10" +
                                  "\n B = 11-20" +
                                  "\n C = 21-30" +
                                  "\n D = 31-40" +
                                  "\n E = 41-50" +
                                  "\n F = 51-60" +
                                  "\n G = 61-70" +
                                  "\n H = 71-80" +
                                  "\n I = 81+" +
                                  "\n T = Tilbage til hovedmenu");
                
                string input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                switch (input[0])
                {
                    case 'T':
                        finish = true; // exit
                        break;

                    case 'K':
                        SetInfectedStat(ref kvinder,0,150, "female");
                        break;

                    case 'M':
                       SetInfectedStat(ref mænd,0,150,"male");
                        break;
                        
                    case 'O':
                        SetInfectedStat(ref andre,0,150,"either");
                        break;

                    case 'A':
                        SetInfectedStat(ref et, 0,11,"all");
                        break;

                    case 'B':
                        SetInfectedStat(ref elleve,11,20,"all");
                        break;

                    case 'C':
                        SetInfectedStat(ref enogtyve,21,30,"all");
                        break;

                    case 'D':
                      SetInfectedStat(ref enogtredive,31,40,"all");
                        break;

                    case 'E':
                        SetInfectedStat(ref enogfyrre,41,50,"all");
                        break;

                    case 'F':
                        SetInfectedStat(ref enoghalvtreds,51,60,"all");
                        break;

                    case 'G':
                        SetInfectedStat(ref enogtres,61,70,"all");
                        break;

                    case 'H':
                        SetInfectedStat(ref enoghalvfjers,71,80,"all");
                        break;

                    case 'I':
                        SetInfectedStat(ref enogfirs,80,150,"all");
                        break;

                    default:
                        break;
                }

            } while (!finish);
        }

    }
}