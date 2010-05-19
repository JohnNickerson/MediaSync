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
                string[] profiles = ConfigurationManager.AppSettings[Environment.MachineName].Split(';');
                foreach (string profile in profiles)
                {
                    SyncOptions opts = SyncOptions.Load(profile);
                    Service s = new Service(opts, new ConsoleView());
                    s.Sync();
                }
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
