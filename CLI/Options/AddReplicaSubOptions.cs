using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class AddReplicaSubOptions
    {
        [Option('p', "library", HelpText = "The name of the library to join or modify")]
        public string ProfileName { get; set; }

        [Option('l', "local", HelpText = "The local path of the replica")]
        public string LocalPath { get; set; }

        [Option('s', "shared", HelpText = "The path (on this machine) to the shared file space")]
        public string SharedPath { get; set; }
    }
}
