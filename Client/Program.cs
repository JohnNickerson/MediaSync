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
                bool simulate = true;
                ulong size = 100 * (10 ^ 6);
                Service s = new Service(paths[0], paths[1], size, simulate, new ConsoleView());

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
