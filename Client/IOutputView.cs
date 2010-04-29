using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    public interface IOutputView
    {
        void WriteLine(string format, params object[] args);
    }
}
