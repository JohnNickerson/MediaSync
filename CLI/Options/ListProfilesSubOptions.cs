using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class ListProfilesSubOptions
    {
        [Option('v', "verbose", HelpText = "Turns on verbose/detailed output")]
        public bool Verbose { get; set; }
    }
}
