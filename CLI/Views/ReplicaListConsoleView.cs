using AssimilationSoftware.MediaSync.Core.Mappers;
using AssimilationSoftware.MediaSync.Core.Model;
using Spectre.Console;
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
                var row = new TableRow([
                    new Markup(library?.Name ?? "Unknown Library"),
                    new Markup(machine?.Name ?? "Unknown Machine"),
                    new Markup(p.ID.ToString()),
                    showPaths ? new Markup(p.LocalPath) : new Markup("N/A")
                ]);
                table.Rows.Add(row);
            }
            AnsiConsole.Write(table);
            Console.WriteLine(string.Empty);
        }
    }
}
