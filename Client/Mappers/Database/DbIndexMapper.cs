using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlServerCe;
using System.Configuration;
using System.Data;
using System.IO;
using AssimilationSoftware.MediaSync.Core.Properties;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;

namespace AssimilationSoftware.MediaSync.Mappers.Database
{
    /// <summary>
    /// Stores file indexes in a database.
    /// </summary>
    class DbIndexMapper : IIndexMapper
    {
        private List<string> contents = new List<string>();
        private SyncProfile _options;
        private string _connString = ConfigurationManager.ConnectionStrings["database"].ConnectionString;

        public DbIndexMapper(SyncProfile options)
        {
            this._options = options;
        }

        void IIndexMapper.Add(string trunc_file)
        {
            contents.Add(trunc_file);
        }

        /// <summary>
        /// Writes a file index to the database.
        /// </summary>
        void IIndexMapper.WriteIndex()
        {
            SqlCeConnection connection = new SqlCeConnection(_connString);
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
                adapter.InsertCommand.Parameters["@Machine"] = new SqlCeParameter("@Machine", Settings.Default.MachineName);
                adapter.InsertCommand.Parameters["@Profile"] = new SqlCeParameter("@Profile", _options.ProfileName);
                adapter.InsertCommand.Parameters["@RelPath"] = new SqlCeParameter("@RelPath", filename);
                //adapter.InsertCommand.Parameters["@Size"] = new SqlCeParameter("@Size", new FileInfo(Path.Combine(_options.LocalPath, filename)).Length);
                // TODO: Include file hash value. Will require reading the entire file.
                // Steal Snowden SHA512 code? It probably uses something else anyway.
                adapter.InsertCommand.Parameters["@Hash"] = new SqlCeParameter("@Hash", DBNull.Value);
                adapter.InsertCommand.ExecuteNonQuery();
            }

            // TODO: Find any indexes older than the last two. Remove them.
            adapter.DeleteCommand = new SqlCeCommand("Delete From Indexes Where Timestamp Not In (Select Top 2 Timestamp From Indexes Where Machine = @Machine And Profile = @Profile) And Machine = @Machine And Profile = @Profile", connection);
            adapter.DeleteCommand.Parameters.Add("@Machine", SqlDbType.NVarChar);
            adapter.DeleteCommand.Parameters.Add("@Profile", SqlDbType.NVarChar);
            adapter.DeleteCommand.Parameters["@Machine"] = new SqlCeParameter("@Machine", Settings.Default.MachineName);
            adapter.DeleteCommand.Parameters["@Profile"] = new SqlCeParameter("@Profile", _options.ProfileName);
            adapter.DeleteCommand.ExecuteNonQuery();
            connection.Close();
        }

        void IIndexMapper.CreateIndex(IFileManager file_manager)
        {
            throw new NotImplementedException();
        }

        int IIndexMapper.PeerCount
        {
            get
            {
                // Select Count(Machine) From Profiles Where Profile = _options.ProfileName
                SqlCeConnection connection = new SqlCeConnection(_connString);
                SqlCeDataAdapter adapter = new SqlCeDataAdapter("select * from Indexes", connection);
                adapter.SelectCommand = new SqlCeCommand("Select Count(Machine) From Indexes Where Profile = @ProfileName", connection);
                adapter.SelectCommand.Parameters.AddWithValue("@ProfileName", _options.ProfileName);
                connection.Open();
                int result = (int)adapter.SelectCommand.ExecuteScalar();
                connection.Close();
                return result;
            }
        }

        Dictionary<string, int> IIndexMapper.CompareCounts()
        {
            throw new NotImplementedException();
        }

        public void Save(FileIndex index)
        {
            throw new NotImplementedException();
        }

        public FileIndex LoadLatest(string machine, string profile)
        {
            throw new NotImplementedException();
        }

        public int NumPeers(string profile)
        {
            throw new NotImplementedException();
        }
    }
}
