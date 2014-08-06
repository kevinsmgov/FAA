using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var faa = new FAA.FAA(args[0]);
            foreach (var plane in faa.AircraftRegistrationSet._MASTER_txt)
            {
                var aircraft = plane._ACFTREF_txtRow;
                var engine = plane._ENGINE_txtRow;
                Console.WriteLine(String.Format("{0} {1} {2} {3}", plane._N_NUMBER, aircraft.MFR, aircraft.MODEL, engine != null ? engine.HORSEPOWER : 0));
            }
        }
    }
}
