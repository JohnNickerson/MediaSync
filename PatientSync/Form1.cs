using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Client;

namespace PatientSync
{
    public partial class Form1 : Form, IOutputView
    {
        #region Constructors
        /// <summary>
        /// Constructs a new form instance.
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            // SourceBox.Text = User folder + Pictures
            SourceBox.Text = string.Format(@"C:\Users\{0}\Pictures", Environment.UserName);
            // SharedBox.Text = User folder + Dropbox + Pictures
            SharedBox.Text = string.Format(@"C:\Users\{0}\Documents\My Dropbox\Photos", Environment.UserName);
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

        #endregion

        private void RunButton_Click(object sender, EventArgs e)
        {
            OutputBox.Clear();
            WriteLine("Run {0} at {1}", SimCheckBox.Checked ? "(simulated)" : string.Empty, DateTime.Now);
            ulong sharesize = ulong.Parse(SpaceBox.Text);
            switch (SpaceUnitSelect.Text)
            {
                case "TB":
                    sharesize *= 10 ^ 12;
                    break;
                case "GB":
                    sharesize *= 10 ^ 9;
                    break;
                case "MB":
                    sharesize *= 10 ^ 6;
                    break;
                case "KB":
                    sharesize *= 1000;
                    break;
                case "B":
                default:
                    break;
            }
            Service syncer = new Service(SourceBox.Text, SharedBox.Text, sharesize, SimCheckBox.Checked, this);
            syncer.Sync();
            WriteLine("Done.");
        }
        #endregion
    }
}
