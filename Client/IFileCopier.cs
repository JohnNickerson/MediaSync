using System;
namespace Client
{
	/// <summary>
	/// An interface for components that handle file copying.
	/// </summary>
	interface IFileCopier
	{
		#region Methods
		/// <summary>
		/// Requests a file copy.
		/// </summary>
		/// <param name="source">The file to copy.</param>
		/// <param name="target">The new location to copy to.</param>
		void CopyFile(string source, string target);
		#endregion

		#region Properties
		/// <summary>
		/// Gets the number of file copies still pending.
		/// </summary>
		int Count { get; }
		#endregion
	}
}
