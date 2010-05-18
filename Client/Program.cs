using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(Environment.MachineName))
            {
                string[] paths = ConfigurationManager.AppSettings[Environment.MachineName].Split(';');
                bool simulate = false;
                ulong size = 100 * (ulong)Math.Pow(10, 6);
                Service s = new Service(paths[0], paths[1], size, simulate, new ConsoleView());
                s.Exclusions.Add(new System.Text.RegularExpressions.Regex(@"Thumbs\.db"));

                s.Sync();
            }
            else
            {
                Console.WriteLine("Please add a configuration entry for the following machine name:");
                Console.WriteLine(Environment.MachineName);
            }
            Console.WriteLine("Finished. Press a key to exit.");
            Console.ReadKey();
        }
    }
}
