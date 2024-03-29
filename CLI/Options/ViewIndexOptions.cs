﻿using AssimilationSoftware.MediaSync.Core.Model;
using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("dir", HelpText = "Inspect the index data")]
    class ViewIndexOptions
    {
        [Option('m', "machine", HelpText = "The name of the machine to inspect, if any.")]
        public string MachineName { get; set; }
        [Option('l', "library", HelpText = "The name of the library to inspect, if any.")]
        public string LibraryName { get; set; }
        [Option('r', "replica", HelpText = "The ID of the replica to inspect, if any.")]
        public string ReplicaId { get; set; }
        [Option('p', "path", HelpText = "The path to look at, if any.")]
        public string LocalPath { get; set; }
        [Option('s', "subfolders", HelpText = "True to show data for all subfolders, false to show only the target path.", Default = false)]
        public bool ShowSubFolders { get; set; }
        [Option("state", HelpText = "Sync state to search for (Synchronised, Transit, Expiring, Destroyed)")]
        public string StateString { get; set; }

        public FileSyncState? State
        {
            get
            {
                if (string.IsNullOrEmpty(StateString)) return null;
                switch (StateString.ToLower())
                {
                    case "synchronised":
                    case "synchronized": // Ew, American spelling.
                        return FileSyncState.Synchronised;
                    case "transit":
                        return FileSyncState.Transit;
                    case "expiring":
                        return FileSyncState.Expiring;
                    case "destroyed":
                        return FileSyncState.Destroyed;
                    default:
                        return null;
                }
            }
        }
    }
}
