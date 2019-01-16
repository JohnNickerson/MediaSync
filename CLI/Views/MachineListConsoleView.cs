using System;
using AssimilationSoftware.MediaSync.Core;

namespace AssimilationSoftware.MediaSync.CLI.Views
{
    public class MachineListConsoleView
    {
        private readonly ViewModel vm;

        public MachineListConsoleView(ViewModel vm)
        {
            this.vm = vm;
        }

        internal void Run()
        {
            var participants = vm.Machines;
            if (participants.Count > 0)
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine("Current machines:");
                Console.WriteLine(string.Empty);
                foreach (var p in participants)
                {
                    Console.WriteLine("\t\t{0}{1}", p, (p.ToLower() == vm.MachineId.ToLower() ? " <-- This machine" : ""));
                }
            }
            else
            {
                Console.WriteLine("No machines currently configured.");
            }
            Console.WriteLine(string.Empty);
        }
    }
}
