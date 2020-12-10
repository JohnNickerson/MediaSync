using CommandLine;
using CommandLine.Text;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class Options
    {
        [HelpVerbOption()]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }

        #region Commands
        [VerbOption("add-profile", HelpText = "Adds a new sync profile")]
        public AddLibrarySubOptions AddVerb { get; set; }

        [VerbOption("init", HelpText = "Initialises general settings")]
        public InitSubOptions InitVerb { get; set; }

        [VerbOption("join-profile", HelpText = "Join (or retarget) a sync profile")]
        public AddReplicaSubOptions JoinVerb { get; set; }

        [VerbOption("leave-profile", HelpText = "Leave a sync profile")]
        public RemoveReplicaSubOptions LeaveVerb { get; set; }

        [VerbOption("update-profile", HelpText = "Modify the size reserved for a profile")]
        public UpdateProfileSubOptions UpdateVerb { get; set; }

        [VerbOption("list", HelpText = "List all profiles")]
        public ListProfilesSubOptions ListProfilesVerb { get; set; }

        [VerbOption("list-machines", HelpText = "List all machines")]
        public ListMachinesSubOptions ListMachinesVerb { get; set; }

        [VerbOption("remove-machine", HelpText = "Removes a machine from a profile")]
        public RemoveMachineSubOptions RemoveMachineVerb { get; set; }

        [VerbOption("remove-profile", HelpText = "Removes an entire profile from the configuration")]
        public RemoveLibraryOptions RemoveLibraryVerb { get; set; }

        [VerbOption("run", HelpText = "Runs all profiles")]
        public RunSubOptions RunVerb { get; set; }

        [VerbOption("version", HelpText = "Displays version information")]
        public VersionSubOptions VersionVerb { get; set; }
        #endregion
    }
}
