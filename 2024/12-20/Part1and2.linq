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

var (normalScore, normalPath) = SolveMaze(map, false);
Console.WriteLine($"Normal path score: {normalScore}");

var wallSkipPaths = FindWallSkipPaths(map, normalScore);
Console.WriteLine($"Wall-skip path score: {wallSkipPaths}");

(int score, HashSet<(int, int)> path) SolveMaze(char[][] map, bool allowWallSkip, (int skipR, int skipC)? wallSkip = null)
{
	// Directions: 0: East, 1: South, 2: West, 3: North
	int[] dr = { 0, 1, 0, -1 };
	int[] dc = { 1, 0, -1, 0 };

	var dist = new Dictionary<(int r, int c, int dir), int>();
	var pathTiles = new HashSet<(int, int)>();
	var pq = new PriorityQueue<(int r, int c, int dir), int>();
	var prev = new Dictionary<(int r, int c, int dir), (int r, int c, int dir)?>();

	// Start from end point now
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

		for (int d = 0; d < 4; d++)
		{
			int nr = r + dr[d];
			int nc = c + dc[d];

			if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
			{
				bool isValidMove = map[nr][nc] != '#' ||
					(allowWallSkip && wallSkip.HasValue && nr == wallSkip.Value.skipR && nc == wallSkip.Value.skipC);

				if (isValidMove)
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

int FindWallSkipPaths(char[][] map, int normalScore)
{
	var result = 0;

	// Try skipping through each wall
	for (int r = 0; r < rows; r++)
	{
		for (int c = 0; c < cols; c++)
		{
			if (map[r][c] == '#')
			{
				var (score, path) = SolveMaze(map, true, (r, c));
				if (score != -1 && normalScore - score >= 100)
				{
					result++;
				}
			}
		}
	}

	return result;
}