using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Configuration;
using System.Data;
using System.IO;

namespace Client.Indexing
{
    class DbIndexer : IIndexService
    {
        private List<string> contents = new List<string>();
        private SyncOptions _options;

        public DbIndexer(SyncOptions options)
        {
            this._options = options;
        }

        void IIndexService.Add(string trunc_file)
        {
            contents.Add(trunc_file);
        }

        void IIndexService.WriteIndex()
        {
            // Also write index to the database.
            SqlCeConnection connection = new SqlCeConnection(ConfigurationManager.ConnectionStrings["database"].ConnectionString);
            SqlCeDataAdapter adapter = new SqlCeDataAdapter("select * from Indexes", connection);
            adapter.InsertCommand = new SqlCeCommand("Insert Into Indexes (Timestamp, Machine, Profile, RelPath, Size, Hash) Values (@Timestamp, @Machine, @Profile, @RelPath, @Size, @Hash)", connection);
            adapter.InsertCommand.Parameters.Add("@Timestamp", SqlDbType.DateTime);
            adapter.InsertCommand.Parameters.Add("@Machine", SqlDbType.NVarChar);
            adapter.InsertCommand.Parameters.Add("@Profile", SqlDbType.NVarChar);
            adapter.InsertCommand.Parameters.Add("@RelPath", SqlDbType.NVarChar);
            adapter.InsertCommand.Parameters.Add("@Size", SqlDbType.BigInt);
            adapter.InsertCommand.Parameters.Add("@Hash", SqlDbType.NVarChar);
            // A static timestamp to share among all records.
            DateTime indextime = DateTime.Now;
            connection.Open();
            foreach (string filename in contents)
            {
                adapter.InsertCommand.Parameters["@Timestamp"] = new SqlCeParameter("@Timestamp", indextime);
                adapter.InsertCommand.Parameters["@Machine"] = new SqlCeParameter("@Machine", Environment.MachineName);
                adapter.InsertCommand.Parameters["@Profile"] = new SqlCeParameter("@Profile", _options.ProfileName);
                adapter.InsertCommand.Parameters["@RelPath"] = new SqlCeParameter("@RelPath", filename);
                adapter.InsertCommand.Parameters["@Size"] = new SqlCeParameter("@Size", new FileInfo(Path.Combine(_options.SourcePath, filename)).Length);
                // TODO: Include file hash value. Will require reading the entire file.
                adapter.InsertCommand.Parameters["@Hash"] = new SqlCeParameter("@Hash", DBNull.Value);
                adapter.InsertCommand.ExecuteNonQuery();
            }
            connection.Close();
        }


        int IIndexService.PeerCount
        {
            get { throw new NotImplementedException(); }
        }

        Dictionary<string, int> IIndexService.CompareCounts()
        {
            throw new NotImplementedException();
        }
    }
}
