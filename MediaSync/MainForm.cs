using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Client;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
using AssimilationSoftware.MediaSync.Core;
using AssimilationSoftware.MediaSync.Core.Views;

namespace AssimilationSoftware.MediaSync.WinForms
{
    public partial class MainForm : Form, IOutputView
    {
        #region Fields
        private string _filename;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructs a new form instance.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            // Look for default settings.
            if (File.Exists("default.xml"))
            {
                _filename = "default.xml";
                LoadOptions();
            }
            else
            {
                // SourceBox.Text = User folder + Pictures
                SourceBox.Text = string.Format(@"C:\Users\{0}\Pictures", Environment.UserName);
                // SharedBox.Text = User folder + Dropbox + Pictures
                SharedBox.Text = string.Format(@"C:\Users\{0}\Documents\My Dropbox\Photos", Environment.UserName);
            }
        }
        #endregion

        #region Methods
        private void BrowseSourceButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == folderBrowserDialog1.ShowDialog(this))
            {
                SourceBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void BrowseSharedButton_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == folderBrowserDialog1.ShowDialog(this))
            {
                SharedBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        #region IOutputView Members

        public void WriteLine(string format, params object[] args)
        {
            OutputBox.AppendText(string.Format(format, args));
            OutputBox.AppendText(Environment.NewLine);
        }

        public void Report(SyncOperation op)
        {
            switch (op.Action)
            {
                case SyncOperation.SyncAction.Copy:
                    OutputBox.AppendText(string.Format("Copying:{2}\t{0}{2}\t->{2}\t{1}", op.SourceFile, op.TargetFile, Environment.NewLine));
                    break;
                case SyncOperation.SyncAction.Delete:
                    OutputBox.AppendText(string.Format("Deleting {0}", op.TargetFile));
                    break;
                default:
                    OutputBox.AppendText(string.Format("Unknown sync action: {0}", op.Action));
                    break;
            }
            OutputBox.AppendText(Environment.NewLine);
        }

        #endregion

        private void RunButton_Click(object sender, EventArgs e)
        {
            OutputBox.Clear();
            WriteLine("Run {0} at {1}", SimCheckBox.Checked ? "(simulated)" : string.Empty, DateTime.Now);
            ulong sharesize = ReserveSize;
            Service syncer = new Service(SourceBox.Text, SharedBox.Text, sharesize, SimCheckBox.Checked, this);
            syncer.Sync();
            WriteLine("Done.");
        }

        private ulong ReserveSize
        {
            get
            {
                ulong sharesize = ulong.Parse(SpaceBox.Text);
                switch (SpaceUnitSelect.Text)
                {
                    case "TB":
                        sharesize *= (ulong)Math.Pow(10, 12);
                        break;
                    case "GB":
                        sharesize *= (ulong)Math.Pow(10, 9);
                        break;
                    case "MB":
                        sharesize *= (ulong)Math.Pow(10, 6);
                        break;
                    case "KB":
                        sharesize *= 1000;
                        break;
                    case "B":
                    default:
                        break;
                }
                return sharesize;
            }
            set
            {
                SpaceBox.Text = value.ToString();
                int zeroes = 0;
                for (int x = SpaceBox.Text.Length - 1; x > 0 && SpaceBox.Text[x] == '0'; x--)
                {
                    zeroes++;
                }
                if (zeroes >= 12)
                {
                    SpaceUnitSelect.Text = "TB";
                    SpaceBox.Text = SpaceBox.Text.Substring(0, SpaceBox.Text.Length - 12);
                }
                else if (zeroes >= 9)
                {
                    SpaceUnitSelect.Text = "GB";
                    SpaceBox.Text = SpaceBox.Text.Substring(0, SpaceBox.Text.Length - 9);
                }
                else if (zeroes >= 6)
                {
                    SpaceUnitSelect.Text = "MB";
                    SpaceBox.Text = SpaceBox.Text.Substring(0, SpaceBox.Text.Length - 6);
                }
                else if (zeroes >= 3)
                {
                    SpaceUnitSelect.Text = "KB";
                    SpaceBox.Text = SpaceBox.Text.Substring(0, SpaceBox.Text.Length - 3);
                }
                else
                {
                    SpaceUnitSelect.Text = "B";
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Save details if working with the default file.
            if (_filename == "default.xml")
            {
                SaveOptions();
            }
            Application.Exit();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_filename == null)
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
            else
            {
                SaveOptions();
            }
        }

        private void SaveOptions()
        {
            SyncOptions s = new SyncOptions();
            s.SourcePath = SourceBox.Text;
            s.SharedPath = SharedBox.Text;
            s.Simulate = SimCheckBox.Checked;
            s.ReserveSpace = ReserveSize;

            SyncOptions.Save(_filename, s);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == saveFileDialog1.ShowDialog(this))
            {
                _filename = saveFileDialog1.FileName;
            }
            SaveOptions();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog(this))
            {
                _filename = openFileDialog1.FileName;
                LoadOptions();
            }
            else
            {
                _filename = null;
            }
        }

        private void LoadOptions()
        {
			//SyncOptions s = SyncOptions.Load(_filename);

			//SourceBox.Text = s.SourcePath;
			//SharedBox.Text = s.SharedPath;
			//ReserveSize = s.ReserveSpace;
			//SimCheckBox.Checked = s.Simulate;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _filename = null;
            SourceBox.Text = string.Empty;
            SharedBox.Text = string.Empty;
            ReserveSize = 0;
            SimCheckBox.Checked = true;
        }
        #endregion
    }
}
