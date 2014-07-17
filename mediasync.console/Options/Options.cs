using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        [VerbOption("addprofile", HelpText = "Adds a new sync profile")]
        public AddProfileSubOptions AddVerb { get; set; }

        [VerbOption("init", HelpText = "Initialises general settings")]
        public InitSubOptions InitVerb { get; set; }

        [VerbOption("joinprofile", HelpText = "Join a sync profile")]
        public JoinProfileSubOptions JoinVerb { get; set; }

        [VerbOption("leaveprofile", HelpText = "Leave a sync profile")]
        public LeaveProfileSubOptions LeaveVerb { get; set; }

        [VerbOption("list", HelpText = "List all profiles")]
        public ListProfilesSubOptions ListProfilesVerb { get; set; }

        [VerbOption("listmachines", HelpText = "List all machines")]
        public ListMachinesSubOptions ListMachinesVerb { get; set; }

        [VerbOption("removemachine", HelpText = "Removes a machine from a profile")]
        public RemoveMachineSubOptions RemoveMachineVerb { get; set; }
        #endregion
    }
}
