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

        #endregion
    }
}
