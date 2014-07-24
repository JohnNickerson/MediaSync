﻿using System;
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
        private ProfileParticipant _localSettings;
        private string _connString = ConfigurationManager.ConnectionStrings["database"].ConnectionString;

        public DbIndexMapper(SyncProfile options, string machine)
        {
            this._options = options;
            this._localSettings = options.GetParticipant(machine);
        }

        public void Save(FileIndex index)
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
            foreach (FileHeader file in index.Files)
            {
                adapter.InsertCommand.Parameters["@Timestamp"] = new SqlCeParameter("@Timestamp", indextime);
                adapter.InsertCommand.Parameters["@Machine"] = new SqlCeParameter("@Machine", _localSettings.MachineName);
                adapter.InsertCommand.Parameters["@Profile"] = new SqlCeParameter("@Profile", _options.Name);
                adapter.InsertCommand.Parameters["@RelPath"] = new SqlCeParameter("@RelPath", Path.Combine(file.RelativePath, file.FileName));
                adapter.InsertCommand.Parameters["@Size"] = new SqlCeParameter("@Size", file.FileSize);
                adapter.InsertCommand.Parameters["@Hash"] = new SqlCeParameter("@Hash", file.ContentsHash);
                adapter.InsertCommand.ExecuteNonQuery();
            }

            // TODO: Find any indexes older than the last two. Remove them.
            adapter.DeleteCommand = new SqlCeCommand("Delete From Indexes Where Timestamp Not In (Select Top 2 Timestamp From Indexes Where Machine = @Machine And Profile = @Profile) And Machine = @Machine And Profile = @Profile", connection);
            adapter.DeleteCommand.Parameters.Add("@Machine", SqlDbType.NVarChar);
            adapter.DeleteCommand.Parameters.Add("@Profile", SqlDbType.NVarChar);
            adapter.DeleteCommand.Parameters["@Machine"] = new SqlCeParameter("@Machine", _localSettings.MachineName);
            adapter.DeleteCommand.Parameters["@Profile"] = new SqlCeParameter("@Profile", _options.Name);
            adapter.DeleteCommand.ExecuteNonQuery();
            connection.Close();
        }

        public FileIndex LoadLatest(string machine, string profile)
        {
            throw new NotImplementedException();
        }


        public List<FileIndex> LoadAll()
        {
            throw new NotImplementedException();
        }

        public List<FileIndex> Load(SyncProfile profile)
        {
            throw new NotImplementedException();
        }

        public List<FileIndex> Load(string machine)
        {
            throw new NotImplementedException();
        }

        public List<FileIndex> Load(string machine, SyncProfile profile)
        {
            throw new NotImplementedException();
        }
    }
}
