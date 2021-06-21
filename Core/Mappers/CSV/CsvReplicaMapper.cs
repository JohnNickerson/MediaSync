using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AssimilationSoftware.Maroon.Interfaces;
using AssimilationSoftware.Maroon.Mappers.Csv;
using AssimilationSoftware.MediaSync.Core.Model;

namespace AssimilationSoftware.MediaSync.Core.Mappers.CSV
{
    class CsvReplicaMapper : CsvDiskMapper<Replica>
    {
        public override Replica FromTokens(string[] tokens)
        {
            return new Replica
            {
                ID = Guid.Parse(tokens[0]),
                MachineId = Guid.Parse(tokens[1]),
                IndexId = Guid.Parse(tokens[2]),
                LocalPath = tokens[3],
                LibraryId = Guid.Parse(tokens[4]),
                IsDeleted = bool.Parse(tokens[5]),
                RevisionGuid = Guid.Parse(tokens[6]),
                PrevRevision = string.IsNullOrEmpty(tokens[7]) ? (Guid?) null : Guid.Parse(tokens[7])
            };
        }

        public override string ToCsv(Replica obj)
        {
            return
                $"{obj.ID},{obj.MachineId},{obj.IndexId},\"{obj.LocalPath}\",{obj.LibraryId},{obj.IsDeleted},{obj.RevisionGuid},{obj.PrevRevision},{obj.ImportHash}";
        }

        public override string FieldsHeader => "ID,MachineId,IndexId,LocalPath,LibraryId,IsDeleted,RevisionGuid,PrevRevision,ImportHash";
    }
}
