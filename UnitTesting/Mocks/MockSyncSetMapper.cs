using AssimilationSoftware.MediaSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using AssimilationSoftware.MediaSync.Core.Model;

namespace UnitTesting.Mocks
{
    class MockSyncSetMapper : ISyncSetMapper
    {
        private SyncSet ss;

        public MockSyncSetMapper(SyncSet ss)
        {
            this.ss = ss;
        }

        public void Delete(string name)
        {
            throw new NotImplementedException();
        }

        public void Delete(SyncSet syncSet)
        {
            throw new NotImplementedException();
        }

        public SyncSet Read(string name)
        {
            throw new NotImplementedException();
        }

        public List<SyncSet> ReadAll()
        {
            return new List<SyncSet> { ss };
        }

        public void Update(SyncSet syncSet)
        {
            ss = syncSet;
        }

        public void UpdateAll(List<SyncSet> syncSets)
        {
            throw new NotImplementedException();
        }
    }
}
