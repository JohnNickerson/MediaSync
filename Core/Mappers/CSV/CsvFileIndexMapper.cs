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
    class CsvFileIndexMapper : CsvDiskMapper<FileIndex>
    {
        public override FileIndex FromTokens(string[] tokens)
        {
            return new FileIndex
            {
                ID = Guid.Parse(tokens[0]),
                LibraryId = Guid.Parse(tokens[1]),
                ReplicaId = string.IsNullOrEmpty(tokens[2]) ? (Guid?)null : Guid.Parse(tokens[2]),
                TimeStamp = DateTime.Parse(tokens[3]),
                LastModified = DateTime.Parse(tokens[4]),
                IsDeleted = bool.Parse(tokens[5]),
                RevisionGuid = Guid.Parse(tokens[6]),
                PrevRevision = string.IsNullOrEmpty(tokens[7]) ? (Guid?) null : Guid.Parse(tokens[7]),
                ImportHash = tokens[8]
            };
        }

        public override string ToCsv(FileIndex obj)
        {
            return
                $"{obj.ID},{obj.LibraryId},{obj.ReplicaId},{obj.TimeStamp},{obj.LastModified:O},{obj.IsDeleted},{obj.RevisionGuid},{obj.PrevRevision},{obj.ImportHash}";
        }

        public override string FieldsHeader =>
            "ID,LibraryId,ReplicaId,TimeStamp,LastModified,IsDeleted,RevisionGuid,PrevRevision,ImportHash";
    }
}
