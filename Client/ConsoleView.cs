using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    /// <summary>
    /// A view that writes to the console.
    /// </summary>
    class ConsoleView : IOutputView
    {
        #region IOutputView Members
        /// <summary>
        /// Writes a line of text (optionally including string format parameters) to the console.
        /// </summary>
        /// <param name="format">A string to output, optionally including format parameters.</param>
        /// <param name="args">Format parameters to fill in to the string.</param>
        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }
        /// <summary>
        /// Reports a single file sync operation via console output.
        /// </summary>
        /// <param name="op">The sync operation to report.</param>
        public void Report(SyncOperation op)
        {
            switch (op.Action)
            {
                case SyncOperation.SyncAction.Copy:
                    Console.WriteLine("Copying:{2}\t{0}{2}\t->{2}\t{1}", op.SourceFile, op.TargetFile, Environment.NewLine);
                    break;
                case SyncOperation.SyncAction.Delete:
                    Console.WriteLine("Deleting {0}", op.TargetFile);
                    break;
                default:
                    Console.WriteLine("Unknown sync action: {0}", op.Action);
                    break;
            }
        }
        #endregion
    }
}
