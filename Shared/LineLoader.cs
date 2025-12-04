using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class LineLoader: ILineLoader
    {
        public List<string> GetLines(string filePath)
        {
            var loadedLines = new List<string>();
            try
            {
                loadedLines.AddRange(System.IO.File.ReadAllLines(filePath));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading lines from {filePath}: {ex.Message}");
            }
            return loadedLines;
        }
    }
}
