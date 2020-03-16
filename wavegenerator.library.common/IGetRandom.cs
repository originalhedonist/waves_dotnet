using System;
using System.Collections.Generic;
using System.Text;

namespace wavegenerator.library.common
{
    public interface IGetRandom
    {
        double GetRandom(double defaultValue = 1);
    }
}
