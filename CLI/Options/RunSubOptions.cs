﻿using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssimilationSoftware.MediaSync.CLI.Options
{
    class RunSubOptions
    {
        [Option('p', "preview", HelpText = "Just preview changes, don't do anything.")]
        public bool IndexOnly { get; set; }

        [Option('l', "log", HelpText = "Sets the level of verbosity of output (0=silent, 1=minimal, 2=conflicts, 3=detailed, 4=verbose)", DefaultValue = 2)]
        public int LogLevel { get; set; }

        [Option('q', "quick", HelpText = "Quick mode. Skips calculating hashes where possible.")]
        public bool QuickMode { get; set; }
    }
}
