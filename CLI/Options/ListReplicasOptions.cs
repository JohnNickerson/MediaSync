using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("list", HelpText = "List all replicas")]
    class ListReplicasOptions
    {
        [Option('v', "verbose", HelpText = "Turns on verbose/detailed output")]
        public bool Verbose { get; set; }
    }
}
