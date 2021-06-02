using AssimilationSoftware.Cuneiform;
using AssimilationSoftware.MediaSync.Core.Mappers;
using AssimilationSoftware.MediaSync.Core.Model;
using System;

namespace AssimilationSoftware.MediaSync.CLI.Views
{
    internal class ProfileListConsoleView
    {
        private readonly DataStore _vm;

        internal ProfileListConsoleView(DataStore vm)
        {
            this._vm = vm;
        }

        internal void Run(bool showPaths)
        {
            var table = new Table();
            table.AddColumns("Library", "Machine", "Replica");

            foreach (Replica p in _vm.ListReplicas())
            {
                var library = _vm.GetLibraryById(p.LibraryId);
                var machine = _vm.GetMachineById(p.MachineId);
                var row = new Row();
                row.Data.Add(library?.Name);
                row.Data.Add(machine.Name);
                if (showPaths)
                {
                    row.Data.Add($"{p.ID} ({p.LocalPath})");
                }
                else
                {
                    row.Data.Add(p.ID);
                }
                table.Rows.Add(row);
            }
            Console.WriteLine(table.ToDisplayString());
            Console.WriteLine(string.Empty);
        }
    }
}
