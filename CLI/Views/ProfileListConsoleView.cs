using AssimilationSoftware.Cuneiform;
using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.Core.Model;
using System;

namespace AssimilationSoftware.MediaSync.CLI.Views
{
    internal class ProfileListConsoleView
    {
        private readonly ViewModel vm;

        internal ProfileListConsoleView(ViewModel vm)
        {
            this.vm = vm;
        }

        internal void Run(bool showPaths)
        {
            Table table = new Table();
            table.AddColumns("Machine");
            foreach (SyncSet p in vm.Profiles)
            {
                var col = new Column
                {
                    Heading = p.Name,
                    Alignment = showPaths ? ColumnAlignment.Left : ColumnAlignment.Centre
                };
                table.Columns.Add(col);
            }
            foreach (var m in vm.Machines)
            {
                var row = new Row();
                row.Data.Add(m);
                var sharow = new Row();
                sharow.Data.Add(null);
                foreach (SyncSet p in vm.Profiles)
                {
                    if (p.ContainsParticipant(m))
                    {
                        if (showPaths)
                        {
                            var party = p.GetIndex(m);
                            row.Data.Add(party.LocalPath);
                            sharow.Data.Add(party.SharedPath);
                        }
                        else
                        {
                            row.Data.Add("*");
                        }
                    }
                    else
                    {
                        row.Data.Add(null);
                        sharow.Data.Add(null);
                    }
                }
                table.Rows.Add(row);
                if (showPaths)
                {
                    table.Rows.Add(sharow);
                }
            }
            Console.WriteLine(table.ToDisplayString());
            Console.WriteLine(string.Empty);
        }
    }
}
