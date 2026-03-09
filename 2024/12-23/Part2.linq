<Query Kind="Program" />

public class LanPartyFinder
{
	public static void Main()
	{
		// Example list of connections from the prompt
		var connections = File.ReadAllLines(@"e:\source\advent2024\12-23\network.txt").ToList();

		// 1. Build an adjacency list
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

		// 2. Generate all subsets of the node list (naive approach)
		var allNodes = adjacencyList.Keys.ToList();
		List<string> largestClique = new List<string>();

		// There are up to 2^N subsets; this is only practical for small N
		int totalSubsets = 1 << allNodes.Count;

		for (int subsetMask = 1; subsetMask < totalSubsets; subsetMask++)
		{
			List<string> subset = new List<string>();
			for (int i = 0; i < allNodes.Count; i++)
			{
				if ((subsetMask & (1 << i)) != 0)
					subset.Add(allNodes[i]);
			}

			if (IsClique(subset, adjacencyList) && subset.Count > largestClique.Count)
			{
				largestClique = subset;
			}
		}

		// 3. Sort the largest clique and form the password
		largestClique.Sort();
		string password = string.Join(",", largestClique);

		Console.WriteLine($"The largest set of connected computers is: {string.Join(", ", largestClique)}");
		Console.WriteLine($"The password to get into the LAN party is: {password}");
	}

	// Helper method to check if a subset of nodes forms a clique
	private static bool IsClique(List<string> nodes, Dictionary<string, HashSet<string>> adjacencyList)
	{
		// A subset is a clique if every pair in the subset is connected
		for (int i = 0; i < nodes.Count; i++)
		{
			for (int j = i + 1; j < nodes.Count; j++)
			{
				string a = nodes[i];
				string b = nodes[j];
				if (!adjacencyList[a].Contains(b))
					return false;
			}
		}
		return true;
	}
}