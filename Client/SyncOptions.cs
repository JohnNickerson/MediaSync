using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Client
{
    [Serializable]
    public class SyncOptions
    {
        #region Fields
        public string SourcePath;
        public string SharedPath;
        public bool Simulate;
		public bool Contributor;
		public bool Consumer;
        public ulong ReserveSpace;
        public string[] ExcludePatterns;
        #endregion

        #region Methods
        /// <summary>
        /// Deserialises sync options from a named file.
        /// </summary>
        /// <param name="filename">The file name to load.</param>
        /// <returns>The sync options stored in the named file, if any.</returns>
        [Obsolete("Sync options are now stored in a database.")]
		public static SyncOptions Load(string filename)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(SyncOptions));
            Stream stream = new FileStream(filename,
                FileMode.Open,
                FileAccess.Read, FileShare.Read);
            SyncOptions s = (SyncOptions)formatter.Deserialize(stream);
            stream.Close();

            return s;
        }

        /// <summary>
        /// Serialises a SyncOptions object to a named file.
        /// </summary>
        /// <param name="filename">The name of the file to save to.</param>
        /// <param name="saveobject">The sync options to save.</param>
		[Obsolete("Sync options are now stored in a database.")]
		public static void Save(string filename, SyncOptions saveobject)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(SyncOptions));
            Stream stream = new FileStream(filename,
                                     FileMode.Create,
                                     FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, saveobject);
            stream.Close();
        }

		//TODO: public static SyncOptions[] Load(string machineName)
		//TODO: public static SyncOptions Load(string machineName, string profile)
		//TODO: public static void Save(SyncOptions saveObject)
        #endregion
    }
}
