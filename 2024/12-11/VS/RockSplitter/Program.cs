using ComputeSharp;

class Program
{
    static void Main(string[] args)
    {
        // Initial arrangement of stones (input data)
        List<string> stones = "8793800 1629 65 5 960 0 138983 85629".Split(' ').ToList();
        int totalBlinks = 75;

        for (int blink = 1; blink <= totalBlinks; blink++)
        {
            Console.WriteLine($"Blink: {blink}");

            // Convert stones to an array of integers 
            int[] stonesArray = stones.Select(int.Parse).ToArray();
            int[] nextStonesArray = new int[stonesArray.Length * 2]; // Allocate maximum possible size

            // Create a GPU buffer for the stones
            using ReadOnlyBuffer<int> stonesBuffer = GraphicsDevice.GetDefault().AllocateReadOnlyBuffer(stonesArray);
            // Create a GPU buffer for the next generation of stones
            using ReadWriteBuffer<int> nextStonesBuffer = GraphicsDevice.GetDefault().AllocateReadWriteBuffer(nextStonesArray);

            // Run the shader
            GraphicsDevice.GetDefault().For(stonesArray.Length, new BlinkShader(stonesBuffer, nextStonesBuffer));

            // Get the results back from the GPU
            nextStonesBuffer.CopyTo(nextStonesArray);

            // Convert the results back to a list of strings, filtering out zeros
            stones = nextStonesArray
                .Where(x => x != 0)
                .Select(x => x.ToString())
                .ToList();

            // Uncomment the following line to observe the arrangement at each blink
            // Console.WriteLine($"After {blink} blink(s): {string.Join(" ", stones)}");
        }

        Console.WriteLine($"Number of stones after {totalBlinks} blinks: {stones.Count}");
    }

    [ThreadGroupSize(DefaultThreadGroupSizes.X)]
    [GeneratedComputeShaderDescriptor]
    public readonly partial struct BlinkShader(ReadWriteBuffer<int> stones, ReadWriteBuffer<int> nextStones) : IComputeShader
    {
        public readonly ReadOnlyBuffer<int> stones;
        public readonly ReadWriteBuffer<int> nextStones;

        public int ThreadIdX => ThreadIds.X;

        public BlinkShader(ReadOnlyBuffer<int> stones, ReadWriteBuffer<int> nextStones) : this()
        {
            this.stones = stones;
            this.nextStones = nextStones;
        }

        public void Execute()
        {
            int stone = stones[ThreadIdX];
            if (stone == 0)
            {
                nextStones[ThreadIdX * 2] = 1;
            }
            else if (stone.ToString().Length % 2 == 0) // Even number of digits
            {
                string str = stone.ToString();
                int half = str.Length / 2;
                string left = str.Substring(0, half).TrimStart('0');
                string right = str.Substring(half).TrimStart('0');

                nextStones[ThreadIdX * 2] = string.IsNullOrEmpty(left) ? 0 : int.Parse(left);
                nextStones[ThreadIdX * 2 + 1] = string.IsNullOrEmpty(right) ? 0 : int.Parse(right);
            }
            else // Multiply by 2024
            {
                nextStones[ThreadIdX * 2] = stone * 2024;
            }
        }
    }
}