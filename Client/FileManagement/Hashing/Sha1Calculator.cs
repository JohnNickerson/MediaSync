using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using System.Security.Cryptography;
using System.IO;

namespace AssimilationSoftware.MediaSync.Core.FileManagement.Hashing
{
    class Sha1Calculator : IFileHashProvider
    {
        public string ComputeHash(string filename)
        {
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                return BitConverter.ToString(cryptoProvider.ComputeHash(new FileInfo(filename).OpenRead()));

                // Buffered and cleaned version. Apparently slower.
                //var stream = new BufferedStream(File.OpenRead(fileinfo.FullName), 1200000);
                //this.ContentsHash = BitConverter.ToString(cryptoProvider.ComputeHash(fileinfo.OpenRead()));
                //stream.Close();
                //stream.Dispose();
            }
        }
    }
}
