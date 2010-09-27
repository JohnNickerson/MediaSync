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
			if (args.Length > 0 && args[0] == "create")
			{
				Console.WriteLine("Profile name:");
				string filename = string.Format("{0}-{1}.xml", Environment.MachineName, Console.ReadLine());
				SyncOptions opts = new SyncOptions();
				Console.WriteLine("Source path:");
				opts.SourcePath = Console.ReadLine();
				Console.WriteLine("Shared path:");
				opts.SharedPath = Console.ReadLine();
				opts.Simulate = false;
				Console.WriteLine("Reserved space:");
				opts.ReserveSpace = ulong.Parse(Console.ReadLine());
				Console.WriteLine("Exclude list (semicolon-separated):");
				opts.ExcludePatterns = Console.ReadLine().Split(';');
				SyncOptions.Save(filename, opts);
			}
            else if (ConfigurationManager.AppSettings.AllKeys.Contains(Environment.MachineName))
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
