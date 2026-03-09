class Program
{
	public static void Main(string[] args)
	{
		var file = File.ReadAllLines(@"e:\source\advent2024\12-19\towels.txt");
        var designs = file.Skip(2).ToArray();
        long patternWithWays = 0;
        long totalWays = 0;

		foreach (var design in designs)
		{
            var patterns = file[0].Split(',')
                .Select(t => t.Trim())
                .OrderBy(t => t.Length)
                .ToArray();

            Dictionary<string, long> memo = new Dictionary<string, long>();
            long ways = CountWays(design, patterns, memo);
            totalWays += ways;
			if (ways > 0) patternWithWays++;
		}
		Console.WriteLine($"Patterns with Ways: {patternWithWays}");
		Console.WriteLine($"Total Ways: {totalWays}");
	}

	public static long CountWays(string design, string[] patterns, Dictionary<string, long> memo)
	{
		if (design == "")
		{
			return 1;
		}

		if (memo.ContainsKey(design))
		{
			return memo[design];
		}

		long count = 0;
		foreach (var pattern in patterns)
		{
			if (design.StartsWith(pattern))
			{
				string remaining = design.Substring(pattern.Length);
				count += CountWays(remaining, patterns, memo);
			}
		}

		memo[design] = count;
		return count;
	}
}