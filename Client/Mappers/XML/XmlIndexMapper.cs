using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Polenter.Serialization;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;

namespace AssimilationSoftware.MediaSync.Mappers.Xml
{
    public class XmlIndexMapper : IIndexMapper
    {
        #region Fields
        private SharpSerializer serialiser;
        private SyncProfile _profile;
        #endregion

        #region Constructors
        public XmlIndexMapper(SyncProfile options)
        {
            serialiser = new SharpSerializer(false);
            _profile = options;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Adds a file to the index.
        /// </summary>
        /// <param name="trunc_file">The file name to add to the index.</param>
        public void Add(string trunc_file)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Persists the index in storage.
        /// </summary>
        public void WriteIndex()
        {
            throw new NotImplementedException();
        }

        public void CreateIndex(IFileManager file_manager)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Compares this index to all the other indices on record.
        /// </summary>
        /// <returns>A dictionary of file names to index membership counts.</returns>
        public Dictionary<string, int> CompareCounts()
        {
            throw new NotImplementedException();
        }

        public void Save(FileIndex index)
        {
            throw new NotImplementedException();
        }

        public FileIndex LoadLatest(string machine, string profile)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of peers participating in this sync profile.
        /// </summary>
        public int PeerCount
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
