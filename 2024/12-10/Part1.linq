<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

string[] file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-10\heightMap.txt");

var maxRows = file.Length;
var maxCols = file[0].Length;

var map = new int[maxRows, maxCols];
for (var r = 0; r < maxRows; r++)
{
	var row = file[r].ToArray();
	for (var c = 0; c < maxCols; c++)
	{
		map[r, c] = int.Parse(row[c].ToString());
	}
}

var trailHeads = new List<PathStep>();
for (var r = 0; r < maxRows; r++)
{
	for (var c = 0; c < maxCols; c++)
	{
		if (map[r, c] == 0) trailHeads.Add(new PathStep(r, c));
	}
}

foreach (var trailHead in trailHeads)
{
	trailHead.FindPath(map);
	//PrintPaths(trailHead);
}
//Parallel.For(0, trailHeads.Count, i =>
//{
//	trailHeads[i].FindPath(map);
//});

var score = 0;
var rating = 0;
foreach (var trailHead in trailHeads)
{
	score += trailHead.Score;
	rating += trailHead.Rating;
}
//Parallel.For(0, trailHeads.Count, i =>
//{
//	score += trailHeads[i].Score();
//});

Console.WriteLine($"All trails scores sum to: {score} all ratings sum to {rating}");

void PrintPaths(PathStep trailHead)
{
	var ends = new List<PathStep>();
	trailHead.GetAllEnds(ends);

	foreach (var end in ends)
	{
		Console.WriteLine(GetPathString(end));
	}

}

string GetPathString(PathStep step)
{
	var parentVal = step.Parent != null ? GetPathString(step.Parent) : "";
	return $"{parentVal} - {map[step.Row, step.Col]} ({step.Row}, {step.Col})";
}

public class PathStep(int r, int c, PathStep parent = null)
{
	public int Row = r;
	public int Col = c;

	public PathStep Parent = parent;
	public List<PathStep> Children = new List<PathStep>();

	private void FindNextSteps(int[,] map)
	{
		var maxRows = map.GetLength(0);
		var maxCols = map.GetLength(1);

		if (Row - 1 >= 0 && map[Row - 1, Col] == map[Row, Col] + 1)
			Children.Add(new PathStep(Row - 1, Col, this));
		if (Row + 1 < maxRows && map[Row + 1, Col] == map[Row, Col] + 1)
			Children.Add(new PathStep(Row + 1, Col, this));
		if (Col - 1 >= 0 && map[Row, Col - 1] == map[Row, Col] + 1)
			Children.Add(new PathStep(Row, Col - 1, this));
		if (Col + 1 < maxCols && map[Row, Col + 1] == map[Row, Col] + 1)
			Children.Add(new PathStep(Row, Col + 1, this));
	}

	private bool PrunePaths(int[,] map)
	{
		var childrenToRemove = new List<PathStep>();
		foreach (var child in Children)
		{
			if (!child.PrunePaths(map)) childrenToRemove.Add(child);
		}
		//Parallel.For(0, Children.Count, i =>
		//{
		//	if (!Children[i].PrunePaths(map)) Children.Remove(Children[i]);
		//});
		childrenToRemove.ForEach(c => Children.Remove(c));
		if (Children.Count == 0) return map[Row, Col] == 9;
		return true;
	}

	public void FindPath(int[,] map)
	{
		FindNextSteps(map);
		foreach (var child in Children)
		{
			child.FindPath(map);
		}
		//Parallel.For(0, Children.Count, i =>
		//{
		//	Children[i].FindPath(map);
		//});
		PrunePaths(map);
	}

	public int Score
	{
		get
		{
			var uniqueNines = new HashSet<PathStep>();
			var ends = new List<PathStep>();
			GetAllEnds(ends);
			ends.ForEach(e => uniqueNines.Add(e));
			return uniqueNines.Count;
		}
	}

	public int Rating
	{
		get
		{
			var ends = new List<PathStep>();
			GetAllEnds(ends);
			return ends.Count;
		}
	}

	public void GetAllEnds(List<PathStep> ends)
	{
		var start = this;
		if (start.Children.Count == 0)
		{
			ends.Add(start);
		}
		foreach (var child in start.Children)
		{
			child.GetAllEnds(ends);
		}
		return;
	}

	public override bool Equals(object other) =>
		other is PathStep ps
		&& ps.Col == this.Col
		&& ps.Row == this.Row;

	public override int GetHashCode() =>
		(Col.ToString("D3") + Row.ToString("D3")).GetHashCode();
}