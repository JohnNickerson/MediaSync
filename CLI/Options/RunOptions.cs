using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("sync", HelpText = "Runs synchronisation on one or all replicas")]
    class RunOptions
    {
        [Option('p', "preview", HelpText = "Just preview changes, don't do anything.")]
        public bool IndexOnly { get; set; }

        [Option('l', "log", HelpText = "Sets the level of verbosity of output (0=silent, 1=minimal, 2=conflicts, 3=detailed, 4=verbose)", Default = 2)]
        public int LogLevel { get; set; }

        [Option("library", HelpText = "The name of the library to synchronise (omit to run all).")]
        public string LibraryName { get; set; }
    }
}
