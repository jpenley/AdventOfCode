using Shared;
using System;
using System.Collections.Generic;
using System.Text;

namespace AoC2025
{
    public class Day03(ILineLoader loader) : IDay
    {
        private ILineLoader loader = loader;

        public string Year => "2025";
        public string Name => "Day 3: Lobby";

        //17109
        public string Part1()
        {
            var total = 0;
            var inputs = Lines;
            foreach (var input in inputs)
            {
                var joltage = int.Parse(FindJoltage(2, input));
                total += joltage;
            }

            return $"The total output joltage is {total.ToString()}";
        }

        public string Part2()
        {
            long total = 0;
            var inputs = Lines;
            foreach (var input in inputs)
            {
                var joltage = long.Parse(FindJoltage(12, input));
                total += joltage;
            }

            return $"The total output joltage is {total.ToString()}";
        }


        private string FindJoltage(int digits, string input)
        {
            if (digits <= 0) return "";
            for (var i = 9; i >= 0; i--)
            {
                var index = input.IndexOf(i.ToString());
                if (index > -1 && index <= input.Length - digits)
                {
                    var nextDigits = FindJoltage(digits - 1, input[(index + 1)..]);
                    return string.Concat(i, nextDigits);
                }
            }

            return "error";
        }

        private List<string> Lines
        {
            get
            {
                var filePath = $"Inputs/{Year}{this.GetType().Name}.txt";
                return field ?? (field = loader.GetLines(filePath));
            }
        }
    }
}
