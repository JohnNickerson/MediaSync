using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    class ConsoleView : IOutputView
    {
        #region IOutputView Members

        public void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(format, args);
        }

        public void Report(SyncOperation op)
        {
            switch (op.Action)
            {
                case SyncOperation.SyncAction.Copy:
                    Console.WriteLine("Copying:{2}\t{0}{2}\t->{2}\t{1}", op.SourceFile, op.TargetFile, Environment.NewLine);
                    break;
                case SyncOperation.SyncAction.Delete:
                    Console.WriteLine("Deleting {0}", op.TargetFile);
                    break;
                default:
                    Console.WriteLine("Unknown sync action: {0}", op.Action);
                    break;
            }
        }
        #endregion
    }
}
