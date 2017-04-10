using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class RemoveProfileOptions
    {
        [Option('p', "profile", HelpText = "The name of the profile to remove", Required = true)]
        public string ProfileName { get; set; }
    }
}
