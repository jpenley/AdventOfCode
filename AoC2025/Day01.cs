using Shared;

namespace AoC2025
{
    public class Day01(ILineLoader loader) : IDay
    {
        private ILineLoader loader = loader;

        public string Year => "2025";

        public string Name => "Day 1: Secret Entrance";

        public string Part1()
        {
            var inputs = Lines;
            var currentPosition = 50;
            var numberOfZeros = 0;
            foreach (var input in inputs)
            {
                var (direction, steps) = (input[..1], int.Parse(input[1..]));
                currentPosition = (direction == "R" ? currentPosition + steps : currentPosition - steps)%100;
                if(currentPosition == 0)
                {
                    numberOfZeros++;
                }
            }
            return $"The number of times we reach position 0 is {numberOfZeros}.";
        }

        public string Part2()
        {
            var inputs = Lines;
            var currentPosition = 50;
            var numberOfZeros = 0;
            foreach (var input in inputs)
            {
                var (direction, steps) = (input[..1], int.Parse(input[1..]));
                for (var i = 0; i < steps; i++)
                {
                    currentPosition = (direction == "R" ? currentPosition + 1 : currentPosition - 1) % 100;
                    if (currentPosition == 0)
                    {
                        numberOfZeros++;
                    }
                }
            }
            return $"The number of times zero was passed is {numberOfZeros}";
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
