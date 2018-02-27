using AssimilationSoftware.Cuneiform;
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
            Table table = new Table();
            table.AddColumns("Machine");
            foreach (SyncSet p in vm.Profiles)
            {
                var col = new Column { Heading = p.Name };
                if (showpaths)
                {
                    col.Alignment = ColumnAlignment.Left;
                }
                else
                {
                    col.Alignment = ColumnAlignment.Centre;
                }
                table.Columns.Add(col);
            }
            foreach (var m in vm.Machines)
            {
                var row = new Cuneiform.Row();
                row.Data.Add(m);
                var sharow = new Row();
                sharow.Data.Add(null);
                foreach (SyncSet p in vm.Profiles)
                {
                    if (p.ContainsParticipant(m))
                    {
                        if (showpaths)
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
                if (showpaths)
                {
                    table.Rows.Add(sharow);
                }
            }
            Console.WriteLine(table.ToDisplayString());
            System.Console.WriteLine(string.Empty);
        }
    }
}
