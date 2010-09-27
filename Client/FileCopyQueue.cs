using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Client
{
	/// <summary>
	/// A class to handle asynchronous file copying.
	/// </summary>
	public class FileCopyQueue
	{
		#region Types
		/// <summary>
		/// A delegate type for performing asynchronous file copies.
		/// </summary>
		/// <param name="source">The file to be copied.</param>
		/// <param name="dest">The location to copy to.</param>
		delegate void CopyFileDelegate(string source, string dest);
		#endregion

		#region Variables
		/// <summary>
		/// A list of asynchronous file copy results.
		/// </summary>
		private List<IAsyncResult> CopyActions;

		/// <summary>
		/// A list of errors that occurred during copies.
		/// </summary>
		public List<Exception> Errors;
		#endregion

		#region Constructors
		/// <summary>
		/// Constructs a new asynchronous file copy service.
		/// </summary>
		public FileCopyQueue()
		{
			CopyActions = new List<IAsyncResult>();
			Errors = new List<Exception>();
		}
		#endregion

		#region Methods
		/// <summary>
		/// Starts an asynchronous file copy operation.
		/// </summary>
		/// <param name="source">The source file to copy.</param>
		/// <param name="target">The destination where the file will be copied to.</param>
		public void CopyFile(string source, string target)
		{
			lock (CopyActions)
			{
				CopyFileDelegate cf = new CopyFileDelegate(File.Copy);
				CopyActions.Add(cf.BeginInvoke(source, target, FinishCopy, cf));
			}
		}
		/// <summary>
		/// Tidies up after a copy operation is complete.
		/// </summary>
		/// <param name="result">The asynchronous details of the copy operation.</param>
		public void FinishCopy(IAsyncResult result)
		{
			lock (CopyActions)
			{
				CopyActions.Remove(result);
			}
			try
			{
				CopyFileDelegate cf = (CopyFileDelegate)result.AsyncState;
				cf.EndInvoke(result);
			}
			catch (Exception e)
			{
				Errors.Add(e);
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the number of pending copy operations.
		/// </summary>
		public int Count
		{
			get
			{
				return CopyActions.Count;
			}
		}
		#endregion
	}
}
