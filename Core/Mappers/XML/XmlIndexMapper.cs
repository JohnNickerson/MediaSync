using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Polenter.Serialization;
using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using AssimilationSoftware.MediaSync.Core.Properties;
using System.IO;

namespace AssimilationSoftware.MediaSync.Core.Mappers.Xml
{
    [Obsolete("To be removed in version 1.2")]
    public class XmlIndexMapper : IIndexMapper
    {
        #region Fields
        private SharpSerializer serialiser;
        private string _filename;
        private List<FileIndex> _indexes;
        #endregion

        #region Constructors
        public XmlIndexMapper(string filename)
        {
            serialiser = new SharpSerializer(false);
            _filename = filename;
            LoadAll();
        }
        #endregion

        #region Methods
        public void Save(FileIndex index)
        {
            _indexes.Add(index);
            // TODO: Prune all but the most recent two indexes for the same machine and profile.
            serialiser.Serialize(_indexes, _filename);
        }

        public FileIndex LoadLatest(string machine, string profile)
        {
            var c = (from i in LoadAll() where i.Participant.MachineName.ToLower() == machine.ToLower() && i.ProfileName.ToLower() == profile.ToLower() orderby i.TimeStamp select i);
            if (c.Count() > 0)
            {
                return c.Last();
            }
            else
            {
                return null;
            }
        }

        public List<FileIndex> LoadAll()
        {
            if (File.Exists(_filename))
            {
                _indexes = (List<FileIndex>)serialiser.Deserialize(_filename);
            }
            else
            {
                _indexes = new List<FileIndex>();
            }
            return _indexes;
        }

        public List<FileIndex> Load(SyncProfile profile)
        {
            return (from i in LoadAll() where i.ProfileName.ToLower() == profile.Name.ToLower() select i).ToList();
        }

        public List<FileIndex> Load(string machine)
        {
            return (from i in LoadAll() where i.Participant.MachineName.ToLower() == machine.ToLower() select i).ToList();
        }

        public List<FileIndex> Load(string machine, SyncProfile profile)
        {
            return (from i in LoadAll() where i.Participant.MachineName.ToLower() == machine.ToLower() && i.ProfileName.ToLower() == profile.Name.ToLower() select i).ToList();
        }
        #endregion
    }
}
