using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class LeaveProfileSubOptions
    {
        [Option('p', "profile", HelpText = "The name of the profile to leave")]
        public string ProfileName { get; set; }
    }
}
