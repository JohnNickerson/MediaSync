using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class AddProfileSubOptions
    {
        [Option('p', "profile", HelpText = "The name of the profile to create", Required = true)]
        public string ProfileName { get; set; }

        [Option('l', "local", HelpText = "The path of the local file collection", Required = true)]
        public string LocalPath { get; set; }

        [Option('s', "shared", HelpText = "The path (on this machine) to the shared file space", Required = true)]
        public string SharedPath { get; set; }

        [Option('r', "reserve", HelpText = "Maximum megabytes to use for this profile in the shared space", DefaultValue = 500u)]
        public ulong ReserveSpaceMb { get; set; }

        [Option('i', "ignore", HelpText = "A list of file search patterns to exclude.", DefaultValue = new string[] { })]
        public string[] IgnorePatterns { get; set; }
    }
}
