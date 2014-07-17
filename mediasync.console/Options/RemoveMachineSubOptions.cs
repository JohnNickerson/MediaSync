using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class RemoveMachineSubOptions
    {
        [Option('m', "machine", HelpText = "The name of the current machine.")]
        public string MachineName { get; set; }
    }
}
