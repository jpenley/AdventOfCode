<Query Kind="Program" />

public class LanPartyFinder
{
	// Adjacency list: each computer key has a set of computers to which it is connected
	private Dictionary<string, HashSet<string>> graph = new Dictionary<string, HashSet<string>>();

	public void AddConnection(string a, string b)
	{
		if (!graph.ContainsKey(a))
			graph[a] = new HashSet<string>();
		if (!graph.ContainsKey(b))
			graph[b] = new HashSet<string>();

		graph[a].Add(b);
		graph[b].Add(a);
	}

	// Bron–Kerbosch algorithm to find the maximum clique in an undirected graph
	// Reference (simplified): Bron–Kerbosch pivot variant
	public List<string> FindMaximumClique()
	{
		var r = new HashSet<string>();
		var p = new HashSet<string>(graph.Keys);
		var x = new HashSet<string>();

		List<string> maxClique = new List<string>();

		BronKerbosch(r, p, x, ref maxClique);
		return maxClique;
	}

	private void BronKerbosch(HashSet<string> r, HashSet<string> p, HashSet<string> x, ref List<string> maxClique)
	{
		if (p.Count == 0 && x.Count == 0)
		{
			// We've found a clique, check if it's the largest
			if (r.Count > maxClique.Count)
			{
				maxClique = r.ToList();
			}
			return;
		}

		// Choose a pivot (just take one from p or x)
		string pivot = (p.Count > 0) ? p.First() : x.First();
		// All neighbors of pivot
		var neighborsOfPivot = graph[pivot];

		// Explore each vertex in p that is not a neighbor of pivot
		foreach (var v in p.Except(neighborsOfPivot).ToList())
		{
			// Intersection to keep only neighbors in the subproblem
			var rNext = new HashSet<string>(r) { v };
			var pNext = new HashSet<string>(p.Where(n => graph[v].Contains(n)));
			var xNext = new HashSet<string>(x.Where(n => graph[v].Contains(n)));

			BronKerbosch(rNext, pNext, xNext, ref maxClique);

			p.Remove(v);
			x.Add(v);
		}
	}

	public static void Main(string[] args)
	{
		// Example usage with the puzzle's sample connections
		// (You can replace these with your puzzle input lines.)
		var connections = File.ReadAllLines(@"e:\source\advent2024\12-23\network.txt").ToList();

		var finder = new LanPartyFinder();
		foreach (var c in connections)
		{
			// Each line "X-Y" means there's a connection between X and Y
			var parts = c.Split('-');
			finder.AddConnection(parts[0], parts[1]);
		}

		// Find the maximum clique
		var maxClique = finder.FindMaximumClique();

		// Sort it alphabetically to form the "password"
		maxClique.Sort(StringComparer.Ordinal);

		// Print result
		Console.WriteLine("Largest set of interconnected computers:");
		Console.WriteLine(string.Join(", ", maxClique));
		Console.WriteLine("LAN party password:");
		Console.WriteLine(string.Join(",", maxClique));
	}
}