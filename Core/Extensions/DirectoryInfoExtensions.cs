using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssimilationSoftware.MediaSync.Core.Extensions
{
    public static class DirectoryInfoExtensions
    {
        public static bool IsSubPathOf(this DirectoryInfo subPath, DirectoryInfo basePath)
        {
            // One arg is null.
            if (subPath == null || basePath == null) return false;
            // Paths are equal.
            if (subPath.FullName.Equals(basePath.FullName, StringComparison.CurrentCultureIgnoreCase)) return true;
            // Different roots.
            if (!basePath.Root.FullName.Equals(subPath.Root.FullName, StringComparison.CurrentCultureIgnoreCase)) return false;
            // subPath is root and paths are not equal.
            if (subPath.Parent == null) return false;
            // subPath is direct child of basePath.
            if (basePath.FullName.Equals(subPath.Parent.FullName, StringComparison.CurrentCultureIgnoreCase)) return true;
            // Check subPath.Parent.
            return subPath.Parent.IsSubPathOf(basePath);
        }
    }
}
