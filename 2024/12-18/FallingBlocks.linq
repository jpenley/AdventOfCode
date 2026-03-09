<Query Kind="Statements" />

var coords = new HashSet<(int, int)>(
			File.ReadAllLines(@"E:\source\advent2024\12-18\coords.txt")
			.Take(1024)
				.SelectMany(l => Regex.Matches(l, @"(\d+),(\d+)")
					.Cast<Match>()
					.Select(m => (int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value))))
		);

var nextCoords = new HashSet<(int, int)>(
			File.ReadAllLines(@"E:\source\advent2024\12-18\coords.txt")
			.Skip(1024)
				.SelectMany(l => Regex.Matches(l, @"(\d+),(\d+)")
					.Cast<Match>()
					.Select(m => (int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value))))
		);


int startRow = -1, startCol = -1;
int endRow = -1, endCol = -1;

var map = new char[71][];
for (int x = 0; x < 71; x++)
{
	map[x] = new char[71];
	for (int y = 0; y < 71; y++)
	{
		if (coords.Contains((x, y))) map[x][y] = '#';
		else map[x][y] = '.';
	}
}

startRow = 0;
startCol = 0;
endRow = 70;
endCol = 70;

if (startRow == -1 || endRow == -1)
{
	throw new ArgumentException("Map must contain start and end points.");
}

Console.WriteLine($"Part 1: It takes {SolveMaze(map)} steps to escape.");

foreach(var coord in nextCoords){
	map[coord.Item1][coord.Item2] = '#';
	if (SolveMaze(map) == -1)
	{
		Console.WriteLine($"Part 2: Block at {coord.Item1},{coord.Item2} blocks path.");
		break;
	}
}

int SolveMaze(char[][] map)
{
	// Directions: 0: East, 1: South, 2: West, 3: North
	int[] dr = { 0, 1, 0, -1 };
	int[] dc = { 1, 0, -1, 0 };

	var dist = new Dictionary<(int r, int c, int dir), int>();
	var pq = new PriorityQueue<(int r, int c, int dir), int>();

	dist[(startRow, startCol, 0)] = 0;
	pq.Enqueue((startRow, startCol, 0), 0);

	while (pq.Count > 0)
	{
		var current = pq.Dequeue();
		int r = current.r;
		int c = current.c;
		int dir = current.dir;
		int score = dist[(r, c, dir)];

		if (r == endRow && c == endCol)
		{
			return score;
		}

		// Move forward
		int nr = r + dr[dir];
		int nc = c + dc[dir];
		if (nr >= 0 && nr < 71 && nc >= 0 && nc < 71 && map[nr][nc] != '#')
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
		int scoreCW = score;
		if (!dist.ContainsKey((r, c, newDirCW)) || scoreCW < dist[(r, c, newDirCW)])
		{
			dist[(r, c, newDirCW)] = scoreCW;
			pq.Enqueue((r, c, newDirCW), scoreCW);
		}

		// Rotate counterclockwise
		int newDirCCW = (dir + 3) % 4;
		int scoreCCW = score;
		if (!dist.ContainsKey((r, c, newDirCCW)) || scoreCCW < dist[(r, c, newDirCCW)])
		{
			dist[(r, c, newDirCCW)] = scoreCCW;
			pq.Enqueue((r, c, newDirCCW), scoreCCW);
		}
	}

	return -1; // No path found
}