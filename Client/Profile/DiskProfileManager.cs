using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using Polenter.Serialization;

namespace AssimilationSoftware.MediaSync.Core.Profile
{
    public class DiskProfileManager : IProfileManager
    {
        public string Filename;

        public DiskProfileManager(string profileListFilename)
        {
            Filename = profileListFilename;
        }

        void IProfileManager.Save(string machineName, SyncProfile saveobject)
        {
            string filename = String.Format("{0}_{1}.xml", machineName, saveobject.ProfileName);
            XmlSerializer formatter = new XmlSerializer(typeof(SyncProfile));
            Stream stream = new FileStream(filename,
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, saveobject);
            stream.Close();
        }

        SyncProfile IProfileManager.Load(string machineName, string profile)
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

        SyncProfile[] IProfileManager.Load(string machineName)
        {
            SharpSerializer s = new SharpSerializer(false);
            List<SyncProfile> results = (List<SyncProfile>)s.Deserialize(Filename);
            return results.ToArray();
        }


        List<SyncProfile> IProfileManager.Load()
        {
            SharpSerializer s = new SharpSerializer(false);
            if (!File.Exists(Filename))
            {
                s.Serialize(new List<SyncProfile>(), Filename);
            }
            List<SyncProfile> p = (List<SyncProfile>)s.Deserialize(Filename);
            return p;
        }

        void IProfileManager.Save(List<SyncProfile> profiles)
        {
            SharpSerializer s = new SharpSerializer(false);
            s.Serialize(profiles, Filename);
        }
    }
}
