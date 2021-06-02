using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("update-profile", HelpText = "Modify the size reserved for a profile")]
    public class UpdateLibraryOptions
    {
        [Option('p', "profile", HelpText = "The name of the profile to modify", Required = true)]
        public string LibraryName { get; set; }

        [Option('r', "reserve", HelpText = "Maximum megabytes to use for this profile in the shared space", Default = 500u)]
        public ulong ReserveSpaceMb { get; set; }
    }
}
