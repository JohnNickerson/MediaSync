using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("purge", HelpText = "Clean up data storage, removing orphaned records.")]
    public class PurgeDataOptions
    {
    }
}
