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
		delegate void CopyFileDelegate(string source, string dest);
		#endregion

		#region Variables
		private List<IAsyncResult> CopyActions;
		private List<Exception> Errors;
		#endregion

		#region Constructors
		public FileCopyQueue()
		{
			CopyActions = new List<IAsyncResult>();
			Errors = new List<Exception>();
		}
		#endregion

		#region Methods
		public void CopyFile(string source, string target)
		{
			lock (CopyActions)
			{
				CopyFileDelegate cf = new CopyFileDelegate(File.Copy);
				CopyActions.Add(cf.BeginInvoke(source, target, FinishCopy, cf));
			}
		}

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
