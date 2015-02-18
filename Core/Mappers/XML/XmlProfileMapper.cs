using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Polenter.Serialization;
using AssimilationSoftware.MediaSync.Core.Model;
using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.Core.Mappers.Xml
{
    [Obsolete("To be removed in version 1.2")]
    public class XmlProfileMapper : IProfileMapper
    {
        public string Filename;

        public XmlProfileMapper(string profileListFilename)
        {
            Filename = profileListFilename;
        }

        void IProfileMapper.Save(string machineName, SyncProfile saveobject)
        {
            string filename = String.Format("{0}_{1}.xml", machineName, saveobject.Name);
            XmlSerializer formatter = new XmlSerializer(typeof(SyncProfile));
            Stream stream = new FileStream(filename,
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, saveobject);
            stream.Close();
        }

        SyncProfile IProfileMapper.Load(string machineName, string profile)
        {
            string filename = String.Format("{0}_{1}.xml", machineName, profile);
            XmlSerializer formatter = new XmlSerializer(typeof(SyncProfile));
            Stream stream = new FileStream(filename,
                                     FileMode.Open,
                                     FileAccess.Read, FileShare.None);
            SyncProfile result = (SyncProfile)formatter.Deserialize(stream);
            stream.Close();
            return result;
        }

        SyncProfile[] IProfileMapper.Load(string machineName)
        {
            SharpSerializer s = new SharpSerializer(false);
            List<SyncProfile> results = (List<SyncProfile>)s.Deserialize(Filename);
            return results.ToArray();
        }


        public List<SyncProfile> Load()
        {
            List<SyncProfile> p;
            if (File.Exists(Filename))
            {
                SharpSerializer s = new SharpSerializer();
                p = (List<SyncProfile>)s.Deserialize(Filename);
            }
            else
            {
                p = new List<SyncProfile>();
            }
            return p;
        }

        public void Save(List<SyncProfile> profiles)
        {
            SharpSerializer s = new SharpSerializer();
            s.Serialize(profiles, Filename);
        }

        public void Save(SyncProfile profile)
        {
            var allprofiles = Load();
            if (allprofiles.Select(p => p.Id).Contains(profile.Id))
            {
                allprofiles.Remove(allprofiles.Where(p => p.Id == profile.Id).First());
            }
            allprofiles.Add(profile);
            Save(allprofiles);
        }

        public SyncProfile Load(int id)
        {
            var search = Load().Where(s => s.Id == id);

            if (search.Count() > 0)
            {
                return search.First();
            }
            else
            {
                return null;
            }
        }


        public void Delete(SyncProfile p)
        {
            throw new NotImplementedException();
        }
    }
}
