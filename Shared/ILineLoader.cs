using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public interface ILineLoader
    {
        public List<string> GetLines(string filePath);
    }
}
