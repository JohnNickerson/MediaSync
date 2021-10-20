using System;
using System.Collections.Generic;
using System.Diagnostics;
using AssimilationSoftware.MediaSync.Core.Interfaces;
using System.Security.Cryptography;
using System.IO;

namespace AssimilationSoftware.MediaSync.Core.FileManagement.Hashing
{
    public class Sha1Calculator : IFileHashProvider
    {
        private Dictionary<string, string> _cache = new Dictionary<string, string>();

        public string ComputeHash(string filename)
        {
            if (_cache.ContainsKey(filename)) return _cache[filename];
            BufferedStream stream = null;
            try
            {
                using (var cryptoProvider = new SHA1CryptoServiceProvider())
                {
                    stream = new BufferedStream(File.OpenRead(filename), 1200000);
                    var hash = BitConverter.ToString(cryptoProvider.ComputeHash(stream));
                    stream.Close();
                    _cache[filename] = hash;
                    return hash;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return null;
            }
            finally
            {
                try
                {
                    stream?.Close();
                }
                catch
                {
                    // ignored
                }
            }
        }

        public void ClearCache(string filename)
        {
            if (_cache.ContainsKey(filename))
            {
                _cache.Remove(filename);
            }
        }
    }
}
