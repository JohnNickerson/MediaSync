using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Client.Indexing
{
    public class TextIndexer : IIndexService
    {
        private SyncOptions _options;
        private List<string> _fileList;

        public TextIndexer(SyncOptions _options)
        {
            this._options = _options;
            _fileList = new List<string>();
        }

        void IIndexService.Add(string trunc_file)
        {
            _fileList.Add(trunc_file);
        }

        void IIndexService.WriteIndex()
        {
            File.WriteAllLines(IndexFileName, _fileList.ToArray());
        }


        int IIndexService.PeerCount
        {
            get
            {
                return Directory.GetFiles(_options.SourcePath, "*_index.txt").Length;
            }
        }

        private string IndexFileName
        {
            get
            {
                string indexfile = Path.Combine(_options.SourcePath, string.Format("{0}_index.txt", Environment.MachineName));
                return indexfile;
            }
        }

        Dictionary<string, int> IIndexService.CompareCounts()
        {
            var FileCounts = new Dictionary<string, int>();
            foreach (string otherindex in Directory.GetFiles(_options.SourcePath, "*_index.txt"))
            {
                if (!otherindex.Equals(IndexFileName))
                {
                    foreach (string idxfilename in File.ReadAllLines(otherindex))
                    {
                        if (FileCounts.ContainsKey(idxfilename))
                        {
                            FileCounts[idxfilename]++;
                        }
                        else
                        {
                            FileCounts[idxfilename] = 1;
                        }
                    }
                }
            }
            return FileCounts;
        }
    }
}
