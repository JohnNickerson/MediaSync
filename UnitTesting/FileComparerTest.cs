using AssimilationSoftware.MediaSync.Core.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTesting.Mocks;

namespace UnitTesting
{
    [TestClass]
    public class FileComparerTest
    {
        /// <summary>
        /// Local file does not match the local or current master index, but does match an old master index revision.
        /// </summary>
        [TestMethod]
        public void OldLocalVersionShouldBeOverwritten()
        {
            // Set up a master index (revisions 0 to 2)
            // Set up a local index (revision 0)
            // Set up a mock file manager (revision 1)
            IFileManager mockfilesystem = new MockFileManager();

            // Get the action queue from the index comparer.

            // Check that the action queue says the local file should be updated.
        }
    }
}
