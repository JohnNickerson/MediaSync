using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Mappers.Xml;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Core;
using System.IO;

namespace UnitTests
{
    class Program
    {
        static void Main(string[] args)
        {
            // For different types of profiles,
            // Test the sync operations with mock indexing and mock file moving.

            // Test the text indexer.
            // Test the database indexer.
            
            // Test the hash calculation.
            var starttime = DateTime.Now;
            foreach (string file in Directory.GetFiles(@"J:\Public\TV\Doctor Who", "*.*", SearchOption.AllDirectories))
            {
                var f = new FileHeader(file, string.Empty, false);
            }
            var endtime = DateTime.Now;
            Console.WriteLine(endtime - starttime);
            Console.ReadKey();
        }
    }
}
