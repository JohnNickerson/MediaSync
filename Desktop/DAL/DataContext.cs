using System.Collections.Generic;
using System.Linq;
using AssimilationSoftware.Maroon.Interfaces;
using AssimilationSoftware.MediaSync.Desktop.Models;

namespace AssimilationSoftware.MediaSync.Desktop.DAL
{
    public class DataContext
    {
        private IMergeRepository<FileHeader> _fileHeaders;
        private IMergeRepository<FileIndex> _fileIndexes;
        private IMergeRepository<FlashDrive> _flashDrives;
        private IMergeRepository<Library> _libraries;
        private IMergeRepository<Replica> _replicas;

        public IEnumerable<FileHeader> FileHeaders => _fileHeaders.Items.ToList().AsReadOnly();
        public IEnumerable<FileIndex> FileIndexes => _fileIndexes.Items.ToList().AsReadOnly();
        public IEnumerable<FlashDrive> FlashDrives => _flashDrives.Items.ToList().AsReadOnly();
        public IEnumerable<Library> Libraries => _libraries.Items.ToList().AsReadOnly();
        public IEnumerable<Replica> Replicas => _replicas.Items.ToList().AsReadOnly();
    }
}
