using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Polenter.Serialization;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;
using AssimilationSoftware.MediaSync.Core.Properties;
using System.IO;

namespace AssimilationSoftware.MediaSync.Mappers.Xml
{
    public class XmlIndexMapper : IIndexMapper
    {
        #region Fields
        private SharpSerializer serialiser;
        private SyncProfile _profile;
        private FileIndex _index;
        #endregion

        #region Constructors
        public XmlIndexMapper(SyncProfile options)
        {
            serialiser = new SharpSerializer(false);
            _profile = options;
            _index = new FileIndex();
            _index.MachineName = Settings.Default.MachineName;
            _index.ProfileName = _profile.ProfileName;
            _index.LocalBasePath = _profile.GetParticipant(Settings.Default.MachineName).LocalPath;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Persists the index in storage.
        /// </summary>
        public void WriteIndex()
        {
            Save(_index);
        }

        /// <summary>
        /// Compares this index to all the other indices on record.
        /// </summary>
        /// <returns>A dictionary of file names to index membership counts.</returns>
        public Dictionary<string, int> CompareCounts()
        {
            var FileCounts = new Dictionary<string, int>();
            string basepath = _profile.GetParticipant(Settings.Default.MachineName).SharedPath;
            foreach (var participant in _profile.Participants)
            {
                string otherindex = Path.Combine(basepath, string.Format("{0}_index.xml", participant.MachineName));
                if (File.Exists(otherindex))
                {
                    FileIndex idx = (FileIndex)serialiser.Deserialize(otherindex);
                    foreach (var idxfile in idx.Files)
                    {
                        var relfile = Path.Combine(idxfile.RelativePath, idxfile.FileName);
                        if (FileCounts.ContainsKey(relfile))
                        {
                            FileCounts[relfile]++;
                        }
                        else
                        {
                            FileCounts[relfile] = 1;
                        }
                    }
                }
            }
            return FileCounts;
        }

        public void Save(FileIndex index)
        {
            serialiser.Serialize(index, Path.Combine(_profile.GetParticipant(Settings.Default.MachineName).SharedPath, string.Format("{0}_index.xml", Settings.Default.MachineName)));
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
        /// Gets the number of peers participating in this sync profile.
        /// </summary>
        public int PeerCount
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

    }
}
