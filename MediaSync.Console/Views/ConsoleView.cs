using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssimilationSoftware.MediaSync.Model;
using AssimilationSoftware.MediaSync.Interfaces;
using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.Console.Properties;

namespace AssimilationSoftware.MediaSync.Console
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
        /// Prompts to configure a path based on an existing value.
        /// </summary>
        /// <param name="path">The path as it exists. May include "{MyDocs}" as a placeholder.</param>
        /// <param name="prompt">The human-friendly name of the folder to be used as a cue.</param>
        /// <returns>The correct path as provided by the user.</returns>
        public string ConfigurePath(string path, string prompt)
        {
            // Special folder replacements.
            path = path.Replace("{MyDocs}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            path = path.Replace("{MyPictures}", Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
            path = path.Replace("{MachineName}", Settings.Default.MachineName);

            System.Console.WriteLine("Configure path to {0}:", prompt);
            System.Console.WriteLine("Type correct value or [Enter] to accept default.");
            System.Console.WriteLine(path);
            var response = System.Console.ReadLine();
            if (response.Trim().Length > 0)
            {
                path = response;
                System.Console.WriteLine();
            }
            return path;
        }

        /// <summary>
        /// Prompts to configure a string value, or accept a default.
        /// </summary>
        /// <param name="value">The initial default value.</param>
        /// <param name="prompt">A prompt for the user.</param>
        /// <returns>The configured value as entered or accepted by the user.</returns>
        public string ConfigureString(string value, string prompt)
        {
            value = value.Replace("{MachineName}", Environment.MachineName);

            System.Console.WriteLine("Configure string value for {0}:", prompt);
            System.Console.WriteLine("Type correct value or [Enter] to accept default.");
            System.Console.WriteLine(value);
            var response = System.Console.ReadLine();
            if (response.Trim().Length > 0)
            {
                value = response;
                System.Console.WriteLine();
            }
            return value;
        }

        /// <summary>
        /// Prompts to configure a numeric value (unsigned long) or accept a default.
        /// </summary>
        /// <param name="value">The default value to present.</param>
        /// <param name="prompt">The description to provide to the user.</param>
        /// <returns>The configured value, whether default or overridden.</returns>
        public ulong ConfigureULong(ulong value, string prompt)
        {
            ulong configval = value;
            System.Console.WriteLine("Configure value for {0}:", prompt);
            System.Console.WriteLine("Type correct value or [Enter] to accept default.");
            System.Console.WriteLine(value);
            var response = System.Console.ReadLine();
            if (response.Trim().Length > 0)
            {
                if (ulong.TryParse(response, out configval))
                {
                    // Everything is fine.
                }
                else
                {
                    System.Console.WriteLine("Could not parse value. Using default.");
                }
            }
            return configval;
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
