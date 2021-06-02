using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("delete-machine", HelpText = "Removes a machine from all replicas")]
    class DeleteMachineOptions
    {
        [Option('m', "machine", HelpText = "The name of the current machine.")]
        public string MachineName { get; set; }
    }
}
