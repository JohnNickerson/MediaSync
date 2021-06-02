using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    [Verb("delete-file", HelpText = "Marks a file as deleted in a library's index, to be deleted in all replicas.")]
    class DeleteFileOptions
    {
        [Option('l', "library", HelpText = "The name of the library that contains the file.", Required = true)]
        public string LibraryName { get; set; }

        [Option('p', "path", HelpText = "The path of the file to delete.", Required = true)]
        public string Path { get; set; }
    }
}
