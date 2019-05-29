using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    public class UpdateProfileSubOptions
    {
        [Option('p', "profile", HelpText = "The name of the profile to modify", Required = true)]
        public string ProfileName { get; set; }

        [Option('r', "reserve", HelpText = "Maximum megabytes to use for this profile in the shared space", DefaultValue = 500u)]
        public ulong ReserveSpaceMb { get; set; }
    }
}
