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
                Service s = new Service(paths[0], paths[1]);
                // Loop while there are active requests.
                while (true)
                {
                    s.CheckInbox();
                    s.RequestFiles();
                    s.ClearEmptyFolders();
                    //s.RemoveMissedRequests();
                    if (s.OutgoingRequests == 0)
                    {
                        break;
                    }
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
