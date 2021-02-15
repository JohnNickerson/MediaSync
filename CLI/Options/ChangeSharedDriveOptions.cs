using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    public class ChangeSharedDriveOptions
    {
        [Option('d', "drive", HelpText = "The new drive letter of the shared drive on this machine", Required = true)]
        public string NewDriveLetter { get; set; }
    }
}
