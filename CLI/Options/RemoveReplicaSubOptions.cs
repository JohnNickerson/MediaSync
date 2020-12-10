using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class RemoveReplicaSubOptions
    {
        [Option('p', "profile", HelpText = "The name of the profile to leave", Required = true)]
        public string ProfileName { get; set; }

        [Option('m', "machine", HelpText = "The name of the machine to remove", DefaultValue = "this")]
        public string MachineName { get; set; }
    }
}
