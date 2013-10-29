using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace AssimilationSoftware.MediaSync.Model
{
    public class FileHeader
    {
        public FileHeader(string filename, string basepath, bool calculateHash)
        {
            var fileinfo = new FileInfo(Path.Combine(basepath, filename));

            this.FileName = fileinfo.Name;
            this.FileSize = fileinfo.Length;
            this.RelativePath = filename.Substring(0, filename.Length - fileinfo.Name.Length);
            if (calculateHash)
            {
                // // MurMurHash3. Needs a buffered reader to improve performance.
                //var hashmaker = new Murmur3();
                //this.ContentsHash = BitConverter.ToString(hashmaker.ComputeHash(fileinfo.OpenRead()));

                using (var cryptoProvider = new SHA1CryptoServiceProvider())
                {
                    // Basic version.
                    var stream = fileinfo.OpenRead();
                    this.ContentsHash = BitConverter.ToString(cryptoProvider.ComputeHash(stream));

                    // Buffered and cleaned version. Apparently slower.
                    //var stream = new BufferedStream(File.OpenRead(fileinfo.FullName), 1200000);
                    //this.ContentsHash = BitConverter.ToString(cryptoProvider.ComputeHash(fileinfo.OpenRead()));
                    //stream.Close();
                    //stream.Dispose();
                }
            }
        }

        public FileHeader()
        {
        }

        public string RelativePath { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string ContentsHash { get; set; }
    }
}
