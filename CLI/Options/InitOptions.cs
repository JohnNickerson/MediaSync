using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("init", HelpText = "Initialises general settings")]
    class InitOptions
    {
        [Option('f', "folder", HelpText = "The folder location to store metadata files", Default = ".")]
        public string MetadataFolder { get; set; }

        [Option('m', "machine", HelpText = "The name of the current machine.", Required = true)]
        public string MachineName { get; set; }

        [Option('s', "shared", HelpText = "The path (on this machine) to the shared file space", Required = true)]
        public string SharedPath { get; set; }
    }
}
