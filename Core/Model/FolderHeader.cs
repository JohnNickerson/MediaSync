using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Model
{
    public class FolderHeader : FileSystemEntry
    {
        public override bool Matches(FileSystemEntry shareFileHead)
        {
            return shareFileHead is FolderHeader &&
                   (string.Equals(RelativePath, shareFileHead.RelativePath,
                       StringComparison.CurrentCultureIgnoreCase) && base.Matches(shareFileHead));
        }
    }
}
