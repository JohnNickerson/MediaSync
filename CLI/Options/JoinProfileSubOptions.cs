using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class JoinProfileSubOptions
    {
        [Option('p', "profile", HelpText = "The name of the profile to join")]
        public string ProfileName { get; set; }

        [Option('l', "local", HelpText = "The path of the local file collection")]
        public string LocalPath { get; set; }

        [Option('s', "shared", HelpText = "The path (on this machine) to the shared file space")]
        public string SharedPath { get; set; }
    }
}
