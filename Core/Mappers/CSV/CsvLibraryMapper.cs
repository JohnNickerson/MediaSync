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
    class CsvLibraryMapper : CsvDiskMapper<Library>
    {
        public override Library FromTokens(string[] tokens)
        {
            return new Library
            {
                ID = Guid.Parse(tokens[0]),
                Name = tokens[1],
                PrimaryIndexId = Guid.Parse(tokens[2]),
                MaxSharedSize = ulong.Parse(tokens[3]),
                IsDeleted = bool.Parse(tokens[4]),
                RevisionGuid = Guid.Parse(tokens[5]),
                PrevRevision = string.IsNullOrEmpty(tokens[6]) ? (Guid?) null : Guid.Parse(tokens[6]),
                ImportHash = tokens[7]
            };
        }

        public override string ToCsv(Library lib)
        {
            return
                $"{lib.ID},{lib.Name},{lib.PrimaryIndexId},{lib.MaxSharedSize},{lib.IsDeleted},{lib.RevisionGuid},{lib.PrevRevision},{lib.ImportHash}";
        }

        public override string FieldsHeader => "ID,Name,PrimaryIndexId,MaxSharedSize,IsDeleted,RevisionGuid,PrevRevision,ImportHash";
    }
}
