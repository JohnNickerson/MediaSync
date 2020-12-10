using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class AddLibrarySubOptions
    {
        [Option('p', "library", HelpText = "The name of the library to create", Required = true)]
        public string LibraryName { get; set; }

        [Option('l', "local", HelpText = "The local path of the library replica", Required = true)]
        public string LocalPath { get; set; }

        [Option('s', "shared", HelpText = "The path (on this machine) to the shared file space", Required = true)]
        public string SharedPath { get; set; }

        [Option('r', "reserve", HelpText = "Maximum megabytes to use for this library in the shared space", DefaultValue = 500u)]
        public ulong ReserveSpaceMb { get; set; }
    }
}
