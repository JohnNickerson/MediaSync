using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("move-replica", HelpText = "Changes the location of a replica on a machine.")]
    class MoveReplicaOptions
    {
        [Option('i', "id", HelpText = "The ID of the replica to move.", Required = true)]
        public string Id { get; set; }

        [Option('p', "path", HelpText = "The new location for the replica.", Required = true)]
        public string Path { get; set; }

        [Option('m', "move", HelpText = "Physically move existing files, if the replica is on the current machine. Otherwise abandon the existing files and start a new copy in a new location.", Default = false)]
        public bool MoveFiles { get; set; }
    }
}
