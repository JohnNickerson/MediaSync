using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.MediaSync.Core;

namespace AssimilationSoftware.MediaSync.CLI.Views
{
    public class MachineListConsoleView
    {
        private ViewModel vm;

        public MachineListConsoleView(ViewModel vm)
        {
            this.vm = vm;
        }

        internal void Run()
        {
            var participants = vm.Machines;
            if (participants.Count() > 0)
            {
                System.Console.WriteLine(string.Empty);
                System.Console.WriteLine("Current machines:");
                System.Console.WriteLine(string.Empty);
                foreach (var p in participants)
                {
                    System.Console.WriteLine("\t\t{0}{1}", p.Name, (p.Name.ToLower() == vm.MachineId.ToLower() ? " <-- This machine" : ""));
                }
            }
            else
            {
                Console.WriteLine("No machines currently configured.");
            }
            System.Console.WriteLine(string.Empty);
        }
    }
}
