<Query Kind="Statements" />

var map = File.ReadAllLines(@"E:\source\advent2024\12-20\maze.txt").Select(l => l.ToArray()).ToArray();

int rows = map.Length;
int cols = map[0].Length;

int startRow = -1, startCol = -1;
int endRow = -1, endCol = -1;

for (int r = 0; r < rows; r++)
{
	for (int c = 0; c < cols; c++)
	{
		if (map[r][c] == 'S')
		{
			startRow = r;
			startCol = c;
		}
		else if (map[r][c] == 'E')
		{
			endRow = r;
			endCol = c;
		}
	}
}

if (startRow == -1 || endRow == -1)
{
	throw new ArgumentException("Map must contain start and end points.");
}

DrawMap(map);

// Main program modifications
var (normalScore, normalPath) = SolveMaze(map);
Console.WriteLine($"Normal path score: {normalScore}");

var cheatPathCount = FindCheatPaths(map, normalScore, 2, 1);
Console.WriteLine($"Cheat path count: {cheatPathCount}");


(int score, HashSet<(int, int)> path) SolveMaze(char[][] map, bool allowCheat = false, (int fromR, int fromC, int toR, int toC)? cheatPath = null)
{
	// Directions: 0: East, 1: South, 2: West, 3: North
	int[] dr = { 0, 1, 0, -1 };
	int[] dc = { 1, 0, -1, 0 };

	var dist = new Dictionary<(int r, int c, int dir), int>();
	var pathTiles = new HashSet<(int, int)>();
	var pq = new PriorityQueue<(int r, int c, int dir), int>();
	var prev = new Dictionary<(int r, int c, int dir), (int r, int c, int dir)?>();

	// Start from end point
	dist[(endRow, endCol, 0)] = 0;
	pq.Enqueue((endRow, endCol, 0), 0);

	int finalScore = -1;
	(int r, int c, int dir)? finalState = null;

	while (pq.Count > 0)
	{
		pq.TryDequeue(out var current, out var priority);
		int r = current.r;
		int c = current.c;
		int dir = current.dir;
		int score = dist[(r, c, dir)];

		if (r == startRow && c == startCol)
		{
			finalScore = score;
			finalState = current;
			break;
		}

		// Handle normal moves
		for (int d = 0; d < 4; d++)
		{
			int nr = r + dr[d];
			int nc = c + dc[d];

			if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && map[nr][nc] != '#')
			{
				int newScore = score + 1;
				if (!dist.ContainsKey((nr, nc, d)) || newScore < dist[(nr, nc, d)])
				{
					dist[(nr, nc, d)] = newScore;
					prev[(nr, nc, d)] = (r, c, dir);
					pq.Enqueue((nr, nc, d), newScore);
				}
			}
		}

		// Handle cheat path if applicable
		if (allowCheat && cheatPath.HasValue &&
			r == cheatPath.Value.fromR && c == cheatPath.Value.fromC)
		{
			int nr = cheatPath.Value.toR;
			int nc = cheatPath.Value.toC;

			for (int d = 0; d < 4; d++)
			{
				int newScore = score + 1; // Cost of using cheat path is 1
				if (!dist.ContainsKey((nr, nc, d)) || newScore < dist[(nr, nc, d)])
				{
					dist[(nr, nc, d)] = newScore;
					prev[(nr, nc, d)] = (r, c, dir);
					pq.Enqueue((nr, nc, d), newScore);
				}
			}
		}
	}

	if (finalState.HasValue)
	{
		// Reconstruct path
		var current = finalState.Value;
		while (prev.ContainsKey(current))
		{
			pathTiles.Add((current.r, current.c));
			current = prev[current].Value;
		}
		pathTiles.Add((current.r, current.c));

		return (finalScore, pathTiles);
	}

	return (-1, new HashSet<(int, int)>());
}

int FindCheatPaths(char[][] map, int normalScore, int cheatSpaces, int minTimeSaved)
{
	var result = 0;
	var seenCheatPaths = new HashSet<(int fromR, int fromC, int toR, int toC)>();

	for (int fromR = 0; fromR < rows; fromR++)
	{
		for (int fromC = 0; fromC < cols; fromC++)
		{
			// Only start from valid path tiles
			if (map[fromR][fromC] == '#') continue;

			// Check all possible end points within 20 spaces
			for (int toR = Math.Max(0, fromR - cheatSpaces); toR <= Math.Min(rows - 1, fromR + cheatSpaces); toR++)
			{
				for (int toC = Math.Max(0, fromC - cheatSpaces); toC <= Math.Min(cols - 1, fromC + cheatSpaces); toC++)
				{
					// Skip if same position or end point is a wall
					if ((fromR == toR && fromC == toC) || map[toR][toC] == '#') continue;

					// Calculate Manhattan distance
					int distance = Math.Abs(toR - fromR) + Math.Abs(toC - fromC) - 1;
					if (distance > cheatSpaces) continue;

					// Check if we've seen this cheat path (or its reverse)
					var cheatPath = (fromR, fromC, toR, toC);
					var reverseCheatPath = (toR, toC, fromR, fromC);
					if (seenCheatPaths.Contains(cheatPath) || seenCheatPaths.Contains(reverseCheatPath))
						continue;

					seenCheatPaths.Add(cheatPath);

					var (score, path) = SolveMaze(map, true, cheatPath);
					if (score != -1 && normalScore - score >= minTimeSaved)
					{
						result++;
					}
				}
			}
		}
	}

	return result;
}

void DrawMap(char[][] map){
	var sb = new StringBuilder();
	map.Select( i=> new string(i)).ToList().ForEach(s=>sb.AppendLine(s));
	Console.Write(sb.ToString());
}
