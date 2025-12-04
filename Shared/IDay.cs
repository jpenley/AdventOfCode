using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Shared
{
    public interface IDay
    {
        string Year { get; }
        string Name { get; }

        string Part1()
        {
            return "Not Implemented";
        }

        string Part2()
        {
            return "Not Implemented";
        }
    }
}
