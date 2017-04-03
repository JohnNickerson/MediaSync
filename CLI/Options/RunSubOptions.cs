using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class RunSubOptions
    {
        [Option('i', "indexonly", HelpText = "True to just perform indexing operations - no copies")]
        public bool IndexOnly { get; set; }

        [Option('l', "log", HelpText = "Sets the level of verbosity of output (0=silent, 1=minimal, 2=conflicts, 3=detailed, 4=verbose)", DefaultValue = 2)]
        public int LogLevel { get; set; }
    }
}
