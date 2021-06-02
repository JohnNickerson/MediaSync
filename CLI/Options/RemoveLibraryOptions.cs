using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("remove-library", HelpText = "Removes an entire library from the configuration. Does not delete files.")]
    class RemoveLibraryOptions
    {
        [Option('n', "name", HelpText = "The name of the library to remove", Required = true)]
        public string LibraryName { get; set; }
    }
}
