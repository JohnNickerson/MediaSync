using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class RemoveMachineSubOptions
    {
        [Option('m', "machine", HelpText = "The name of the current machine.")]
        public string MachineName { get; set; }
    }
}
