using AssimilationSoftware.MediaSync.Core.Interfaces;

namespace UnitTesting.Mocks
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
