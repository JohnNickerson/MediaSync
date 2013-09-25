﻿using System;
using System.Collections.Generic;
namespace AssimilationSoftware.MediaSync.Core
{
	/// <summary>
	/// An interface for components that handle file copying.
	/// </summary>
	public interface IFileManager
	{
		#region Methods
		/// <summary>
		/// Requests a file copy.
		/// </summary>
		/// <param name="source">The file to copy.</param>
		/// <param name="target">The new location to copy to.</param>
		void CopyFile(string source, string target);

        ulong SharedPathSize();

        bool ShouldCopy(string filename);

        void Delete(string dir);

        void EnsureFolder(string targetdir);

        void SetNormalAttributes();

        string[] ListLocalFiles();
		#endregion

		#region Properties
		/// <summary>
		/// Gets the number of file copies still pending.
		/// </summary>
		int Count { get; }

        List<Exception> Errors { get; }
		#endregion
    }
}
