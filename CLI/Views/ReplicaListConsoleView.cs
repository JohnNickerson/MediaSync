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

        internal void Run(bool verbose)
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
                    new Markup(verbose?p.ID.ToString() : p.ID.ToString().Substring(0, 8)),
                    new Markup(p.LocalPath)
                ]);
                table.Rows.Add(row);
            }
            AnsiConsole.Write(table);
            Console.WriteLine(string.Empty);
        }
    }
}
