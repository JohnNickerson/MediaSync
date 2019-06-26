﻿using CommandLine;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class InitSubOptions
    {
        [Option('f', "folder", HelpText = "The folder location to store metadata files", DefaultValue = ".")]
        public string MetadataFolder { get; set; }

        [Option('m', "machine", HelpText = "The name of the current machine.", Required = true)]
        public string MachineName { get; set; }

        [Option("format", HelpText = "Specify the data file format: 'litedb' or 'xml'", DefaultValue = "xml")]
        public string DataFileFormat { get; set; }
    }
}
