using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class RunSubOptions
    {
        [Option('v', "verbose", HelpText = "Turns on verbose/detailed output")]
        public bool Verbose { get; set; }

        [Option('i', "indexonly", HelpText = "True to just perform indexing operations - no copies")]
        public bool IndexOnly { get; set; }
    }
}
