using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.MediaSync.Core.Model;

namespace AssimilationSoftware.MediaSync.Core.Mappers.CSV
{
    public class CsvMachineMapper : AssimilationSoftware.Maroon.Mappers.Csv.CsvDiskMapper<Machine>
    {
        public override string FieldsHeader => "ID,Name,SharedPath,LastModified,IsDeleted,RevisionGuid,PrevRevision,ImportHash";

        public override Machine FromTokens(string[] tokens)
        {
            return new Machine
            {
                ID = Guid.Parse(tokens[0]),
                Name = tokens[1],
                SharedPath = tokens[2],
                LastModified = DateTime.Parse(tokens[3]),
                IsDeleted = bool.Parse(tokens[4]),
                RevisionGuid = Guid.Parse(tokens[5]),
                PrevRevision = string.IsNullOrEmpty(tokens[6]) ? (Guid?) null : Guid.Parse(tokens[6]),
                ImportHash = tokens[7]
            };
        }

        public override string ToCsv(Machine obj)
        {
            return
                $"{obj.ID},{obj.Name},{obj.SharedPath},{obj.LastModified:O},{obj.IsDeleted},{obj.RevisionGuid},{obj.PrevRevision},{obj.ImportHash}";
        }
    }
}
