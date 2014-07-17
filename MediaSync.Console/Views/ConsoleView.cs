using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;
using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.CLI.Properties;

namespace AssimilationSoftware.MediaSync.CLI
{
    /// <summary>
    /// A view that writes to the console.
    /// </summary>
    public class ConsoleView : IInputView
    {
        #region Methods
        /// <summary>
        /// Writes a line of text (optionally including string format parameters) to the console.
        /// </summary>
        /// <param name="format">A string to output, optionally including format parameters.</param>
        /// <param name="args">Format parameters to fill in to the string.</param>
        void WriteLine(string format, params object[] args)
        {
            System.Console.WriteLine(format, args);
        }

        void WriteLine()
        {
            System.Console.WriteLine();
        }

        /// <summary>
        /// Reports a single file sync operation via console output.
        /// </summary>
        /// <param name="op">The sync operation to report.</param>
        void Report(SyncOperation op)
        {
            switch (op.Action)
            {
                case SyncOperation.SyncAction.Copy:
                    System.Console.WriteLine("Copying:{2}\t{0}{2}\t->{2}\t{1}", op.SourceFile, op.TargetFile, Environment.NewLine);
                    break;
                case SyncOperation.SyncAction.Delete:
                    System.Console.WriteLine("Deleting {0}", op.TargetFile);
                    break;
                default:
                    System.Console.WriteLine("Unknown sync action: {0}", op.Action);
                    break;
            }
        }

        /// <summary>
        /// Waits for a keypress from the user.
        /// </summary>
        public void WaitForKey()
        {
            System.Console.ReadKey();
        }
        #endregion

        #region Properties
        string Status
        {
            set
            {
                System.Console.WriteLine(value);
            }
        }
        #endregion
    }
}
