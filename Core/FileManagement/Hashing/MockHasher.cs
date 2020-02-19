using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace AssimilationSoftware.MediaSync.Core.FileManagement.Hashing
{
    public class MockHasher : IFileHashProvider
    {
        public string ComputeHash(string filename)
        {
            return string.Empty;
        }

        public void ClearCache(string filename)
        {
            // ignored.
        }
    }
}
