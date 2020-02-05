using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core
{
    public class FileActionsCount
    {
        public int CopyToLocalCount { get; set; }
        public int CopyToSharedCount { get; set; }
        public int DeleteLocalCount { get; set; }
        public int DeleteMasterCount { get; set; }
        public int RenameLocalCount { get; set; }
        public int NoActionCount { get; set; }

        public bool AnyChanges => CopyToLocalCount + CopyToSharedCount + DeleteLocalCount + DeleteMasterCount + RenameLocalCount > 0;

        public string GetDisplayString(string header = null)
        {
            var table = new StringBuilder();
            table.Append(header);
            table.Append(new string(' ', Math.Max(0, 12 - (header?.Length ?? 0))));
            table.AppendLine(" | Local   | Shared  |");
            table.AppendLine("-------------+---------+---------+");
            table.AppendLine($"Copy To      | {CopyToLocalCount,7} | {CopyToSharedCount,7} |");
            table.AppendLine($"Delete       | {DeleteLocalCount,7} | {DeleteMasterCount,7} |");
            table.AppendLine($"Conflicted   | {RenameLocalCount,7} |         |");
            table.AppendLine($"No Change    | {NoActionCount,7} |         |");

            return table.ToString();
        }

        public void Add(FileActionsCount syncResult)
        {
            CopyToLocalCount += syncResult.CopyToLocalCount;
            CopyToSharedCount += syncResult.CopyToSharedCount;
            DeleteLocalCount += syncResult.DeleteLocalCount;
            DeleteMasterCount += syncResult.DeleteMasterCount;
            RenameLocalCount += syncResult.RenameLocalCount;
            NoActionCount += syncResult.NoActionCount;
        }
    }
}
