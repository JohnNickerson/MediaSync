using AssimilationSoftware.MediaSync.CLI.Properties;
using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.CLI.Views
{
    internal class ProfileListConsoleView
    {
        private ViewModel vm;

        internal ProfileListConsoleView(ViewModel vm)
        {
            this.vm = vm;
        }

        internal void Run(bool showpaths)
        {
            // Print a summary of profiles.
            System.Console.WriteLine(string.Empty);
            System.Console.WriteLine("Current profiles ('*' indicates this machine is participating)");
            System.Console.WriteLine(string.Empty);
            foreach (SyncSet p in vm.Profiles)
            {
                var star = p.ContainsParticipant(Settings.Default.MachineName);
                System.Console.WriteLine("{0}\t{1}", (star ? "*" : ""), p.Name);
                // Show participating paths if detailed view is selected.
                if (showpaths && star)
                {
                    var party = p.GetIndex(Settings.Default.MachineName);
                    System.Console.WriteLine("\t\t{0}", party.LocalPath);
                    System.Console.WriteLine("\t\t{0}", party.SharedPath);
                }
            }
            System.Console.WriteLine(string.Empty);
        }
    }
}
