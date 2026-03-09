<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
</Query>

var day = new StoneCounter();
var output = day.SolvePart2("8793800 1629 65 5 960 0 138983 85629");
output.Dump();

public class StoneCounter
{

	private static ConcurrentDictionary<(long stone, int i), long> memo = [];

	private static async Task<long> StoneSorter(long stone, int i)
	{
		//No more blinks this is the last stone of its branch.
		if (i == 0)
		{
			return 1;
		}
		else if (memo.ContainsKey((stone, i)))
		{
			return memo[(stone, i)];
		}
		else
		{
			if (stone == 0)
			{
				memo[(stone, i)] = await StoneSorter(1, i - 1);
			}
			else if (stone.ToString().Length % 2 == 0)
			{
				var str = stone.ToString();
				memo[(stone, i)] = (await Task.WhenAll(new string[] { str[..(str.Length / 2)], str[(str.Length / 2)..] }.Select(long.Parse).Select(s => StoneSorter(s, i - 1)))).Sum();
			}
			else
			{
				memo[(stone, i)] = await StoneSorter(stone * 2024, i - 1);
			}
			return memo[(stone, i)];
		}
	}

	private static string Solve(string input, int blinks)
	{
		memo = [];
		return $"{Task.WhenAll(
				input.Split(' ').Select(long.Parse)
					.Select(stone => StoneSorter(stone, blinks))
					.ToArray())
			.Result.Sum()}";

	}

	public string SolvePart1(string input)
		=> Solve(input, 25);

	public string SolvePart2(string input)
		=> Solve(input, 75);
}

public static class Counter{
	public static int Count;
	
	public static void Increment(){
		Interlocked.Increment(ref Count);
	}
}
