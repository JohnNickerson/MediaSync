using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AssimilationSoftware.MediaSync.Core;

namespace UnitTests
{
    [TestFixture]
    public class QueuedDiskManagerTest
    {
        [Test]
        public void CreateIndex()
        {
            QueuedDiskCopier q = new QueuedDiskCopier(null, null, null);
            var f = q.CreateIndex();
        }
    }
}
