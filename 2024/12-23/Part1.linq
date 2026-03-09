<Query Kind="Program" />

public class LanPartyFinder
{
	public static void Main()
	{
		// Example list of connections from the prompt
		// Each connection is a line with two computer names separated by '-'
		var connections = File.ReadAllLines(@"e:\source\advent2024\12-23\network.txt").ToList();

		// 1. Build an adjacency list for the network graph.
		Dictionary<string, HashSet<string>> adjacencyList = new Dictionary<string, HashSet<string>>();
		foreach (var connection in connections)
		{
			var parts = connection.Split('-');
			var a = parts[0];
			var b = parts[1];

			if (!adjacencyList.ContainsKey(a))
				adjacencyList[a] = new HashSet<string>();
			if (!adjacencyList.ContainsKey(b))
				adjacencyList[b] = new HashSet<string>();

			adjacencyList[a].Add(b);
			adjacencyList[b].Add(a);
		}

		// 2. Find all triplets (a,b,c) such that each is connected to the other two
		//    (i.e., a clique of size 3).
		var triplets = new HashSet<string>();
		var allNodes = adjacencyList.Keys.ToList();

		// Sort allNodes to ensure consistent ordering, so we don’t pick duplicates (e.g. a,b,c and b,a,c).
		allNodes.Sort();

		for (int i = 0; i < allNodes.Count; i++)
		{
			for (int j = i + 1; j < allNodes.Count; j++)
			{
				for (int k = j + 1; k < allNodes.Count; k++)
				{
					string a = allNodes[i];
					string b = allNodes[j];
					string c = allNodes[k];

					if (adjacencyList[a].Contains(b)
						&& adjacencyList[b].Contains(c)
						&& adjacencyList[c].Contains(a))
					{
						// Construct a sorted identifier for the triplet to avoid duplicates
						var tripletNodes = new List<string> { a, b, c };
						tripletNodes.Sort();
						string tripletKey = string.Join(",", tripletNodes);
						triplets.Add(tripletKey);
					}
				}
			}
		}

		// 3. Filter triplets that have at least one computer with a name starting with 't'.
		int countWithT = triplets.Count(tripletKey =>
		{
			var nodes = tripletKey.Split(',');
			return nodes.Any(node => node.StartsWith("t"));
		});

		// 4. Output the result
		Console.WriteLine($"Number of 3-computer sets containing at least one starting with 't': {countWithT}");
	}
}