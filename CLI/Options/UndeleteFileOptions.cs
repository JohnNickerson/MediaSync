using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("undelete-file", HelpText = "Marks a deleted file as not deleted in a library's index. Stops a file from being deleted in replicas.")]
    class UndeleteFileOptions
    {
        [Option('l', "library", HelpText = "The name of the library that contains the file.", Required = true)]
        public string LibraryName { get; set; }

        [Option('p', "path", HelpText = "The path of the file to restore.", Required = true)]
        public string Path { get; set; }
    }
}
