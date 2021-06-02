using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("add-library", HelpText = "Adds a new sync library")]
    class AddLibraryOptions
    {
        [Option('p', "library", HelpText = "The name of the library to create", Required = true)]
        public string LibraryName { get; set; }

        [Option('l', "local", HelpText = "The local path of the library replica", Required = true)]
        public string LocalPath { get; set; }

        [Option('r', "reserve", HelpText = "Maximum megabytes to use for this library in the shared space", Default = 500u)]
        public ulong ReserveSpaceMb { get; set; }
    }
}
