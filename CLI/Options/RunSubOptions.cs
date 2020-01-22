using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class RunSubOptions
    {
        [Option('p', "preview", HelpText = "Just preview changes, don't do anything.")]
        public bool IndexOnly { get; set; }

        [Option('l', "log", HelpText = "Sets the level of verbosity of output (0=silent, 1=minimal, 2=conflicts, 3=detailed, 4=verbose)", DefaultValue = 2)]
        public int LogLevel { get; set; }

        [Option("profile", HelpText = "The name of the profile to run (omit to run all).")]
        public string ProfileSearch { get; set; }
    }
}
