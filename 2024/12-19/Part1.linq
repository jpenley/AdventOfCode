<Query Kind="Statements" />

//var map = File.ReadAllLines(@"D:\Paylocity\advent2024\12-16\example1.txt").Select(l => l.ToArray()).ToArray();
var file = File.ReadAllLines(@"e:\source\advent2024\12-19\towels.txt");

var towelPatterns = file[0].Split(',').Select(t => t.Trim()).OrderBy(t => t.Length).ToArray();
var desiredPatterns = file.Skip(2).ToArray();

var matchedPatterns = 0;
for (var dp = 0; dp < desiredPatterns.Length; dp++)
{
	var pq = new PriorityQueue<(string, List<string>), int>();
	var currentPattern = desiredPatterns[dp];
	var matches = new List<string>();
	pq.Enqueue((currentPattern, matches), 0);
	var match = false;
	while (pq.Count > 0 && !match)
	{
		(string cp, List<string> m) = pq.Dequeue();
		for (var tp = 0; tp < towelPatterns.Length; tp++)
		{
			var towelPattern = towelPatterns[tp];
			if (cp.StartsWith(towelPattern))
			{
				if (cp.Length == towelPattern.Length) match = true;
				var np = cp.Substring(towelPattern.Length);
				m.Add(towelPattern);
				pq.Enqueue((np, m.ToList()), np.Length);
			}
			if (towelPattern.Length > cp.Length)
			{
				break;
			}
		}
		if (match)
		{
			matchedPatterns++;
			match = false;
		}
	}
}

matchedPatterns.Dump();