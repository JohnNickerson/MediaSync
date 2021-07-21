using AssimilationSoftware.Cuneiform;
using AssimilationSoftware.MediaSync.Core.Mappers;
using AssimilationSoftware.MediaSync.Core.Model;
using System;

namespace AssimilationSoftware.MediaSync.CLI.Views
{
    internal class ReplicaListConsoleView
    {
        private readonly DataStore _vm;

        internal ReplicaListConsoleView(DataStore vm)
        {
            this._vm = vm;
        }

        internal void Run(bool showPaths)
        {
            var table = new Table();
            table.AddColumns("Library", "Machine", "Replica", "Path");

            foreach (Replica p in _vm.ListReplicas())
            {
                var library = _vm.GetLibraryById(p.LibraryId);
                var machine = _vm.GetMachineById(p.MachineId);
                var row = new Row();
                row.Data.Add(library?.Name);
                row.Data.Add(machine.Name);
                row.Data.Add(p.ID);
                row.Data.Add(p.LocalPath);
                table.Rows.Add(row);
            }

            table.MaxWidth = Console.WindowWidth - 2; // Known Cuneiform bug: tables get squished unless a maximum width is specified.
            Console.WriteLine(table.ToDisplayString());
            Console.WriteLine(string.Empty);
        }
    }
}
