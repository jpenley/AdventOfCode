<Query Kind="Program">
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

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
			
			var nextStones = new ConcurrentBag<string>(); // Use ConcurrentBag for unordered fast insertions

			int rangeSize = Math.Max(1, stones.Count / Environment.ProcessorCount); // Ensure range size is at least 1

			Parallel.ForEach(Partitioner.Create(0, stones.Count, rangeSize), range =>
			{
				var localQueue = new List<string>(); // Use local list to minimize contention

				for (int i = range.Item1; i < range.Item2; i++)
				{
					string stone = stones[i];
					if (stone == "0")
					{
						localQueue.Add("1");
					}
					else if (stone.Length % 2 == 0) // Even number of digits
					{
						int half = stone.Length / 2;
						ReadOnlySpan<char> span = stone.AsSpan();
						string left = span.Slice(0, half).TrimStart('0').ToString();
						string right = span.Slice(half).TrimStart('0').ToString();
						localQueue.Add(string.IsNullOrEmpty(left) ? "0" : left);
						localQueue.Add(string.IsNullOrEmpty(right) ? "0" : right);
					}
					else // Multiply by 2024
					{
						long number = long.Parse(stone);
						localQueue.Add((number * 2024).ToString());
					}
				}

				foreach (var item in localQueue)
				{
					nextStones.Add(item);
				}
			});

			stones = nextStones.ToList(); // Convert ConcurrentBag back to List

			// Uncomment the following line to observe the arrangement at each blink
			// Console.WriteLine($"After {blink} blink(s): {string.Join(" ", stones)}");
		}

		Console.WriteLine($"Number of stones after {totalBlinks} blinks: {stones.Count}");
	}
}
