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
        private string _filename;
        private List<FileIndex> _indexes;
        #endregion

        #region Constructors
        public XmlIndexMapper(string filename)
        {
            serialiser = new SharpSerializer(false);
            _filename = filename;
            if (File.Exists(_filename))
            {
                _indexes = LoadAll();
            }
            else
            {
                _indexes = new List<FileIndex>();
            }
        }
        #endregion

        #region Methods
        public void Save(FileIndex index)
        {
            _indexes.Add(index);
            serialiser.Serialize(_indexes, _filename);
        }

        public FileIndex LoadLatest(string machine, string profile)
        {
            var c = (from i in LoadAll() where i.MachineName == machine && i.ProfileName == profile orderby i.TimeStamp select i);
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
            return (List<FileIndex>)serialiser.Deserialize(_filename);
        }

        public List<FileIndex> Load(SyncProfile profile)
        {
            return (from i in LoadAll() where i.ProfileName == profile.ProfileName select i).ToList();
        }

        public List<FileIndex> Load(string machine)
        {
            return (from i in LoadAll() where i.MachineName == machine select i).ToList();
        }

        public List<FileIndex> Load(string machine, SyncProfile profile)
        {
            return (from i in LoadAll() where i.MachineName == machine && i.ProfileName == profile.ProfileName select i).ToList();
        }
        #endregion
    }
}
