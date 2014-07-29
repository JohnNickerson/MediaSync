﻿using CommandLine;
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
        [VerbOption("add-profile", HelpText = "Adds a new sync profile")]
        public AddProfileSubOptions AddVerb { get; set; }

        [VerbOption("init", HelpText = "Initialises general settings")]
        public InitSubOptions InitVerb { get; set; }

        [VerbOption("join-profile", HelpText = "Join a sync profile")]
        public JoinProfileSubOptions JoinVerb { get; set; }

        [VerbOption("leave-profile", HelpText = "Leave a sync profile")]
        public LeaveProfileSubOptions LeaveVerb { get; set; }

        [VerbOption("list", HelpText = "List all profiles")]
        public ListProfilesSubOptions ListProfilesVerb { get; set; }

        [VerbOption("list-machines", HelpText = "List all machines")]
        public ListMachinesSubOptions ListMachinesVerb { get; set; }

        [VerbOption("remove-machine", HelpText = "Removes a machine from a profile")]
        public RemoveMachineSubOptions RemoveMachineVerb { get; set; }

        [VerbOption("run", HelpText = "Runs all profiles")]
        public RunSubOptions RunVerb { get; set; }

        [VerbOption("version", HelpText = "Displays version information")]
        public VersionSubOptions VersionVerb { get; set; }
        #endregion
    }
}
