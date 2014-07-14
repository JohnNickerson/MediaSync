using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class InitSubOptions
    {
        [Option('f', "folder", HelpText = "The folder location to store metadata files")]
        public string MetadataFolder { get; set; }

        [Option('m', "machine", HelpText = "The name of the current machine.")]
        public string MachineName { get; set; }
    }
}
