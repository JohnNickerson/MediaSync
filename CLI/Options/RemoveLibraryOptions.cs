using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class RemoveLibraryOptions
    {
        [Option('p', "profile", HelpText = "The name of the profile to remove", Required = true)]
        public string ProfileName { get; set; }
    }
}
