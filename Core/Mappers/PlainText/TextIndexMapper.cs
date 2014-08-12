using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using AssimilationSoftware.MediaSync.Core.Properties;
using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.Core.Mappers.PlainText
{
    /// <summary>
    /// An index class that writes to a text file.
    /// </summary>
    public class TextIndexMapper : IIndexMapper
    {
        #region Fields
        /// <summary>
        /// The sync profile to work from.
        /// </summary>
        private SyncProfile _options;

        private ProfileParticipant _localSettings;

        /// <summary>
        /// The current list of files.
        /// </summary>
        private List<string> _fileList;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new text indexer for a given sync profile.
        /// </summary>
        /// <param name="_options">The sync profile to work from.</param>
        public TextIndexMapper(SyncProfile _options, string machine)
        {
            this._options = _options;
            _localSettings = _options.GetParticipant(machine);
            _fileList = new List<string>();
        }
        #endregion

        #region Methods
        public void Save(FileIndex index)
        {
            var files = from f in index.Files select Path.Combine(f.RelativePath, f.FileName);
            File.WriteAllLines(IndexFileName, files.ToArray());
        }

        public FileIndex LoadLatest(string machine, string profile)
        {
            throw new NotImplementedException();
        }

        public List<FileIndex> LoadAll()
        {
            throw new NotImplementedException();
        }

        public List<FileIndex> Load(SyncProfile profile)
        {
            throw new NotImplementedException();
        }

        public List<FileIndex> Load(string machine)
        {
            throw new NotImplementedException();
        }

        public List<FileIndex> Load(string machine, SyncProfile profile)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the file to which the index will be written.
        /// </summary>
        private string IndexFileName
        {
            get
            {
                string indexfile = Path.Combine(_localSettings.SharedPath, string.Format("{0}_index.txt", _localSettings.MachineName));
                return indexfile;
            }
        }
        #endregion

    }
}
