using System;
using System.Collections.Generic;
using System.Text;

namespace wavegenerator
{
    public interface ISettingsProvider<T> where T : class
    {
        T GetSettings();
    }
}
