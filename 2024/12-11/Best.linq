<Query Kind="Program">
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

class Program
{
	private static ConcurrentDictionary<(long stone, int i), long> stoneCache = [];
	
	static void Main(string[] args)
	{
		Stopwatch sw;
		var stones = "8793800 1629 65 5 960 0 138983 85629".Split(' ').Select(long.Parse).ToArray();
		
		sw = Stopwatch.StartNew();
		Console.WriteLine($"Number of stones after 25 blinks: {StoneCounter(stones, 25)}");
		Console.WriteLine(sw.ElapsedMilliseconds);
		sw = Stopwatch.StartNew();
		Console.WriteLine($"Number of stones after 75 blinks: {StoneCounter(stones, 75)}");
		Console.WriteLine(sw.ElapsedMilliseconds);
	}

	private static long StoneCounter(long[] stones, int blinks)
	{
		var Tasks = new List<Task<long>>();
		foreach (var stone in stones)
		{
			Tasks.Add(StoneWrapper(stone, blinks));
		};
		var result = Task.WhenAll(Tasks).Result.Sum();
		return result;
	}

	private static async Task<long> StoneWrapper(long stone, int blinks)
	{
		if (stoneCache.ContainsKey((stone, blinks)))
		{
			return stoneCache[(stone, blinks)];
		}
		stoneCache[(stone, blinks)] = await StoneEvaluator(stone, blinks);
		return stoneCache[(stone, blinks)];
	}

	private static async Task<long> StoneEvaluator(long stone, int blinks)
	{
		//No more blinks this is the last stone of its branch.
		if (blinks == 0)
		{
			return 1;
		}
		else
		{
			if (stone == 0)
			{
				return await StoneWrapper(1, blinks - 1);
			}
			else if (stone.ToString().Length % 2 == 0)
			{
				var str = stone.ToString();
				return (await Task.WhenAll(new string[] { str[..(str.Length / 2)], str[(str.Length / 2)..] }.Select(long.Parse).Select(s => StoneWrapper(s, blinks - 1)))).Sum();
			}
			else
			{
				return await StoneWrapper(stone * 2024, blinks - 1);
			}
		}
	}
}
