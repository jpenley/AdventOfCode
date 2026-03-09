<Query Kind="Statements" />

var map = File.ReadAllLines(@"D:\Paylocity\advent2024\12-16\example1.txt").Select(l => l.ToArray()).ToArray();// Output: 7036
//var map = File.ReadAllLines(@"D:\Paylocity\advent2024\12-16\example2.txt").Select(l=>l.ToArray()).ToArray();// Output: 11048
//var map = File.ReadAllLines(@"D:\Paylocity\advent2024\12-16\maze.txt").Select(l=>l.ToArray()).ToArray();

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

var (score, tiles) = SolveMaze(map);
Console.WriteLine($"Maze score: {score} and {tiles} tiles on best paths."); 

(int,int) SolveMaze(char[][] map)
{
	// Directions: 0: East, 1: South, 2: West, 3: North
	int[] dr = { 0, 1, 0, -1 };
	int[] dc = { 1, 0, -1, 0 };

	var dist = new Dictionary<(int r, int c, int dir), int>();
    var bestPathTiles = new HashSet<(int, int)>();
	var pq = new PriorityQueue<(int r, int c, int dir), int>();

	dist[(startRow, startCol, 0)] = 0;
	pq.Enqueue((startRow, startCol, 0), 0);
	
	while (pq.Count > 0)
	{
		pq.TryDequeue(out var current, out var priority);
		int r = current.r;
		int c = current.c;
		int dir = current.dir;
		int score = dist[(r, c, dir)];
		bestPathTiles.Add((r,c));

		if (r == endRow && c == endCol)
		{
			if(pq.TryPeek(out var next, out var nextPriority) && nextPriority != priority)
			return (score, bestPathTiles.Count);
		}

		// Move forward
		int nr = r + dr[dir];
		int nc = c + dc[dir];
		if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && map[nr][nc] != '#')
		{
			int newScore = score + 1;
			if (!dist.ContainsKey((nr, nc, dir)) || newScore < dist[(nr, nc, dir)])
			{
				dist[(nr, nc, dir)] = newScore;
				pq.Enqueue((nr, nc, dir), newScore);
			}
		}

		// Rotate clockwise
		int newDirCW = (dir + 1) % 4;
		int scoreCW = score + 1000;
		if (!dist.ContainsKey((r, c, newDirCW)) || scoreCW < dist[(r, c, newDirCW)])
		{
			dist[(r, c, newDirCW)] = scoreCW;
			pq.Enqueue((r, c, newDirCW), scoreCW);
		}

		// Rotate counterclockwise
		int newDirCCW = (dir + 3) % 4;
		int scoreCCW = score + 1000;
		if (!dist.ContainsKey((r, c, newDirCCW)) || scoreCCW < dist[(r, c, newDirCCW)])
		{
			dist[(r, c, newDirCCW)] = scoreCCW;
			pq.Enqueue((r, c, newDirCCW), scoreCCW);
		}
	}

	return (-1,-1); // No path found
}