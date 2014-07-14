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
        #endregion
    }
}
