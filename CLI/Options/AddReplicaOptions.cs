using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("add-replica", HelpText = "Creates a local replica of a synchronised library")]
    class AddReplicaOptions
    {
        [Option('n', "library", HelpText = "The name of the library to join or modify")]
        public string LibraryName { get; set; }

        [Option('l', "local", HelpText = "The local path of the replica")]
        public string LocalPath { get; set; }
    }
}
