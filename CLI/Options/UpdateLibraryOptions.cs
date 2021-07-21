using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("update-library", HelpText = "Modify the size reserved for a library")]
    public class UpdateLibraryOptions
    {
        [Option('n', "library", HelpText = "The name of the library to modify", Required = true)]
        public string LibraryName { get; set; }

        [Option('r', "reserve", HelpText = "Maximum megabytes to use for this library in the shared space", Default = 500u)]
        public ulong ReserveSpaceMb { get; set; }
    }
}
