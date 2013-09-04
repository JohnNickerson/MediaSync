using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace AssimilationSoftware.MediaSync.Core.Profile
{
    public class DiskProfileManager : IProfileManager
    {
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
            List<SyncProfile> results = new List<SyncProfile>();
            foreach (string filename in Directory.GetFiles(".", string.Format("{0}_*.xml", machineName)))
            {
                string profilename = new FileInfo(filename).Name.Remove(0, machineName.Length + 1).Replace(".xml", string.Empty);
                results.Add(((IProfileManager)this).Load(machineName, profilename));
            }
            return results.ToArray();
        }
    }
}
