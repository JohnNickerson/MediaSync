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
    class CsvFileSystemEntryMapper : CsvDiskMapper<FileSystemEntry>
    {
        public override FileSystemEntry FromTokens(string[] tokens)
        {
            switch (tokens[1])
            {
                case "FileHeader":
                    return new FileHeader
                    {
                        ID = Guid.Parse(tokens[0]),
                        State = (FileSyncState) Enum.Parse(typeof(FileSyncState), tokens[2]),
                        IndexId = Guid.Parse(tokens[3]),
                        Size = long.Parse(tokens[4]),
                        ContentsHash = tokens[5],
                        RelativePath = tokens[6],
                        LastModified = DateTime.Parse(tokens[7]),
                        IsDeleted = bool.Parse(tokens[8]),
                        RevisionGuid = Guid.Parse(tokens[9]),
                        PrevRevision = string.IsNullOrEmpty(tokens[10]) ? (Guid?) null : Guid.Parse(tokens[10]),
                        ImportHash = tokens[11],
                        BasePath = tokens[12],
                        //IsFolder = bool.Parse(tokens[13]),
                        LastWriteTime = DateTime.Parse(tokens[14])
                    };
                case "FolderHeader":
                    return new FolderHeader
                    {
                        ID = Guid.Parse(tokens[0]),
                        State = (FileSyncState) Enum.Parse(typeof(FileSyncState), tokens[2]),
                        IndexId = Guid.Parse(tokens[3]),
                        Size = long.Parse(tokens[4]),
                        ContentsHash = tokens[5],
                        RelativePath = tokens[6],
                        LastModified = DateTime.Parse(tokens[7]),
                        IsDeleted = bool.Parse(tokens[8]),
                        RevisionGuid = Guid.Parse(tokens[9]),
                        PrevRevision = string.IsNullOrEmpty(tokens[10]) ? (Guid?) null : Guid.Parse(tokens[10]),
                        ImportHash = tokens[11],
                    };
                default:
                    return null;
            }
        }

        public override string ToCsv(FileSystemEntry obj)
        {
            if (obj is FileHeader file)
            {
                return
                    $"{file.ID},FileHeader,{file.State},{file.IndexId},{file.Size},{file.ContentsHash},{file.RelativePath},{file.LastModified:O},{file.IsDeleted},{file.RevisionGuid},{file.PrevRevision},{file.ImportHash},{file.BasePath},false,{file.LastWriteTime:O}";
            }
            else if (obj is FolderHeader folder)
            {
                return
                    $"{folder.ID},FolderHeader,{folder.State},{folder.IndexId},{folder.Size},{folder.ContentsHash},{folder.RelativePath},{folder.LastModified:O},{folder.IsDeleted},{folder.RevisionGuid},{folder.PrevRevision},{folder.ImportHash}";
            }
            else return string.Empty;
        }

        public override string FieldsHeader => "ID,Type,State,IndexId,Size,ContentsHash,RelativePath,LastModified,IsDeleted,RevisionGuid,PrevRevision,ImportHash,BasePath,IsFolder,LastWriteTime";
    }
}
