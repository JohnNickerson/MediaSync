using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class ActionQueue
    {
        public int Id { get; set; }

        public List<FileAction> Actions { get; set; }

        public Repository Repository { get; set; }
    }
}
