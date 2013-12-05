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
        /// <summary>
        /// Compares this index to all the other indices on record.
        /// </summary>
        /// <returns>A dictionary of file names to index membership counts.</returns>
        public Dictionary<string, int> CompareCounts(SyncProfile options)
        {
            var FileCounts = new Dictionary<string, int>();

            var indexes = Load(options);
            // For each other most recent index...
            foreach (var p in options.Participants)
            {
                var f = LoadLatest(p.MachineName, options.ProfileName);
                if (f != null)
                {
                    foreach (var idxfile in f.Files)
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
