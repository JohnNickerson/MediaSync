using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Polenter.Serialization;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;

namespace AssimilationSoftware.MediaSync.Mappers.Xml
{
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


        List<SyncProfile> IProfileMapper.Load()
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

        void IProfileMapper.Save(List<SyncProfile> profiles)
        {
            SharpSerializer s = new SharpSerializer();
            s.Serialize(profiles, Filename);
        }
    }
}
