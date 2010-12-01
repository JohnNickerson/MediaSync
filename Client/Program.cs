﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Data.SqlServerCe;
using System.Data;
using System.IO;
using AssimilationSoftware.MediaSync.Core.Indexing;
using AssimilationSoftware.MediaSync.Core.Views;

namespace AssimilationSoftware.MediaSync.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                foreach (SyncOptions opts in SyncOptions.Load(Environment.MachineName))
                {
                    Service s = new Service(opts, new ConsoleView(), new TextIndexer(opts));
                    s.Sync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not sync.");
                Console.WriteLine(e.Message);
                File.WriteAllText("error.log", e.StackTrace);
            }

            Console.WriteLine("Finished. Press a key to exit.");
            Console.ReadKey();
        }

		public void ConnectListAndSaveSQLCompactExample()
		{
			// Create a connection to the file datafile.sdf in the program folder
			string dbfile = new System.IO.FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName + "\\index.sdf";
			SqlCeConnection connection = new SqlCeConnection("datasource=" + dbfile);

			// Read all rows from the table test_table into a dataset (note, the adapter automatically opens the connection)
			SqlCeDataAdapter adapter = new SqlCeDataAdapter("select * from Profiles", connection);
			DataSet data = new DataSet();
			adapter.Fill(data);

			// Add a row to the test_table (assume that table consists of a text column)
			DataRow r = data.Tables[0].NewRow();
			r["ID"] = 1;
			r["Machine"] = Environment.MachineName;
			r["Profile"] = "Music";
			r["MediaPath"] = Environment.SpecialFolder.MyMusic;
			r["SharedPath"] = "J:\\Music";
			r["SharedSpace"] = 4000000000;
			r["Contributor"] = true;
			r["Consumer"] = true;
			data.Tables[0].Rows.Add(r);
			//data.Tables[0].Rows.Add(new object[] { "New row added by code" });

			// Save data back to the database
			//adapter.Update(data);
			adapter.InsertCommand = new SqlCeCommand("Insert Into Profiles (Machine, Profile, MediaPath, SharedPath, SharedSpace, Contributor, Consumer) Values (@Machine, @Profile, @MediaPath, @SharedPath, @SharedSpace, @Contributor, @Consumer)", connection);
			foreach (DataColumn col in r.Table.Columns)
			{
				adapter.InsertCommand.Parameters.AddWithValue(string.Format("@{0}", col.ColumnName), r[col]);
			}
			connection.Open();
			adapter.InsertCommand.ExecuteNonQuery();

			// Close 
			connection.Close();
		}

    }
}
