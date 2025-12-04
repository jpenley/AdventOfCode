using System;
using System.Collections.Generic;
using System.Text;
using Shared;

namespace AoC2025
{
    public class Day02(ILineLoader loader) : IDay
    {
        private ILineLoader loader = loader;

        public string Year => "2025";
        public string Name => "Day 2: Gift Shop";

        public string Part1()
        {
            var inputs = Lines[0].Split(',');
            long total = 0;
            foreach (var input in inputs)
            {
                var range = input.Split("-");
                var start = long.Parse(range[0]);
                var end = long.Parse(range[1]);
                for (var i = start; i <= end; i++)
                {
                    var stringified = i.ToString();
                    if (stringified.Length % 2 == 0 && stringified[..(stringified.Length/2)]== stringified[(stringified.Length/2)..])
                    {
                        total += i;
                    }
                }
            }
            return total.ToString();
        }

        public string Part2()
        {
            var inputs = Lines[0].Split(',');
            long total = 0;
            foreach (var input in inputs)
            {
                var range = input.Split("-");
                var start = long.Parse(range[0]);
                var end = long.Parse(range[1]);
                for (var i = start; i <= end; i++)
                {
                    var stringified = i.ToString();
                    for(var l = 1; l <= stringified.Length/2; l++)
                    {
                        var matched = false;
                        var pattern = stringified[..l];
                        for (var p = l; p <= stringified.Length - l; p += l)
                        {
                            if (stringified[p..(p+l)] != pattern)
                            {
                                break;
                            }
                            if (p== stringified.Length - l)
                            {
                                total = total + i;
                                matched = true;
                            }
                        }
                        if(matched)
                        {
                            break;
                        }
                    }

                }
            }
            return total.ToString();
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
