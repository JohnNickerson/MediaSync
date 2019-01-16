using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class ListProfilesSubOptions
    {
        [Option('v', "verbose", HelpText = "Turns on verbose/detailed output")]
        public bool Verbose { get; set; }
    }
}
