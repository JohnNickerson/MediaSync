using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("remove-replica", HelpText = "Removes a replica of a library. Does not delete files.")]
    class RemoveReplicaOptions
    {
        [Option('i', "id", HelpText = "The unique ID of the replica to remove.", Required = true)]
        public string Id { get; set; }
    }
}
