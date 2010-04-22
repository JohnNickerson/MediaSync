using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace PhotoSync
{
    class Program
    {
        static void Main(string[] args)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(Environment.MachineName))
            {
                string[] paths = ConfigurationManager.AppSettings[Environment.MachineName].Split(';');
                Service s = new Service(paths[0], paths[1]);

                while (true)
                {
                    s.CheckInbox();
                    s.IndexFiles();
                    s.ClearEmptyFolders();
                    System.Threading.Thread.Sleep(new TimeSpan(0, 5, 0));
                }
            }
            else
            {
                Console.WriteLine(Environment.MachineName);
            }
            Console.WriteLine("Finished. Press a key to exit.");
            Console.ReadKey();
        }
    }
}
