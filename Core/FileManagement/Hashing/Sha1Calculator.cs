using System;
using System.Diagnostics;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using System.Security.Cryptography;
using System.IO;

namespace AssimilationSoftware.MediaSync.Core.FileManagement.Hashing
{
    public class Sha1Calculator : IFileHashProvider
    {
        public string ComputeHash(string filename)
        {
            try
            {
                using (var cryptoProvider = new SHA1CryptoServiceProvider())
                {
                    var stream = new FileInfo(filename).OpenRead();
                    var hash = BitConverter.ToString(cryptoProvider.ComputeHash(stream));
                    stream.Close();
                    return hash;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
    }
}
