using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Configuration;
using System.Data;
using System.IO;

namespace AssimilationSoftware.MediaSync.Core.Indexing
{
    /// <summary>
    /// Stores file indexes in a database.
    /// </summary>
    class DbIndexer : IIndexService
    {
        private List<string> contents = new List<string>();
        private SyncProfile _options;

        public DbIndexer(SyncProfile options)
        {
            this._options = options;
        }

        void IIndexService.Add(string trunc_file)
        {
            contents.Add(trunc_file);
        }

        /// <summary>
        /// Writes a file index to the database.
        /// </summary>
        void IIndexService.WriteIndex()
        {
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
                adapter.InsertCommand.Parameters["@Size"] = new SqlCeParameter("@Size", new FileInfo(Path.Combine(_options.LocalPath, filename)).Length);
                // TODO: Include file hash value. Will require reading the entire file.
                adapter.InsertCommand.Parameters["@Hash"] = new SqlCeParameter("@Hash", DBNull.Value);
                adapter.InsertCommand.ExecuteNonQuery();
            }

            // TODO: Find any indexes older than the last two. Remove them.
            adapter.DeleteCommand = new SqlCeCommand("Delete From Indexes Where Timestamp Not In (Select Top 2 Timestamp From Indexes Where Machine = @Machine And Profile = @Profile) And Machine = @Machine And Profile = @Profile", connection);
            adapter.DeleteCommand.Parameters.Add("@Machine", SqlDbType.NVarChar);
            adapter.DeleteCommand.Parameters.Add("@Profile", SqlDbType.NVarChar);
            adapter.DeleteCommand.Parameters["@Machine"] = new SqlCeParameter("@Machine", Environment.MachineName);
            adapter.DeleteCommand.Parameters["@Profile"] = new SqlCeParameter("@Profile", _options.ProfileName);
            adapter.DeleteCommand.ExecuteNonQuery();
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
