using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class FileAction
    {
        public int Id { get; set; }

        public DateTime TimeStamp { get; set; }

        public Machine Source { get; set; }

        public FileHeader File { get; set; }
    }
}
