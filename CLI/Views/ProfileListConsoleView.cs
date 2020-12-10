using AssimilationSoftware.Cuneiform;
using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.Core.Model;
using System;

namespace AssimilationSoftware.MediaSync.CLI.Views
{
    internal class ProfileListConsoleView
    {
        private readonly ViewModel _vm;

        internal ProfileListConsoleView(ViewModel vm)
        {
            this._vm = vm;
        }

        internal void Run(bool showPaths)
        {
            Table table = new Table();
            table.AddColumns("Machine");
            foreach (SyncSet p in _vm.Profiles.Values)
            {
                var col = new Column
                {
                    Heading = p.Name,
                    Alignment = showPaths ? ColumnAlignment.Left : ColumnAlignment.Centre
                };
                table.Columns.Add(col);
            }

            var reserveRow = new Row();
            reserveRow.Data.Add("Reserve Space (MB)");
            foreach (var m in _vm.Machines)
            {
                var row = new Row();
                row.Data.Add(m);
                var sharow = new Row();
                sharow.Data.Add(null);
                foreach (SyncSet p in _vm.Profiles.Values)
                {
                    if (p.ContainsParticipant(m))
                    {
                        if (showPaths)
                        {
                            var party = p.GetIndex(m);
                            row.Data.Add(party.LocalPath);
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
                    reserveRow.Data.Add(p.ReserveSpace/1000000);
                }
                table.Rows.Add(row);
                if (showPaths)
                {
                    table.Rows.Add(sharow);
                }
            }

            if (showPaths)
            {
                table.Rows.Add(new Row());
                table.Rows.Add(reserveRow);
            }
            Console.WriteLine(table.ToDisplayString());
            Console.WriteLine(string.Empty);
        }
    }
}
