<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//4:00 to run

var maxRows = 130;
var maxCols = 130;

var map = new char[maxRows, maxCols];

var sr = new StreamReader(@"D:\Paylocity\advent2024\12-06\map.txt");
var line = sr.ReadLine();

Position startPosition = null;
var row = 0;
while (line != null)
{
	var chars = line.ToCharArray();
	var col = 0;
	foreach (var c in chars)
	{
		map[row, col] = c;
		if (map[row, col] == '^')
		{
			startPosition = new Position(col, row);
		}

		col++;
	}
	line = sr.ReadLine();
	row++;
}
sr.Close();

var startDirection = new Direction(0, -1);

var guard = new Guard(map, startDirection, startPosition);
var uniquePositions = guard.GetRoute();
var loopingRoutes = 0;
var counter = 0;
Parallel.ForEach (uniquePositions, p => 
{
	var newMap = (char[,])map.Clone();
	newMap[p.Position.Y, p.Position.X] = '#';
	var newGuard = new Guard(newMap, startDirection, startPosition);
	newGuard.GetRoute();
	if (newGuard.IsRouteLoop) loopingRoutes++;
});

Console.WriteLine($"{uniquePositions.Count()} positions in original route");
Console.WriteLine($"There are {loopingRoutes} object positions that result in loops.");

public class Guard
{
	public Direction Direction;
	public Direction StartDirection;
	public Position Position;
	public Position StartPosition;
	public char[,] Map;
	public bool IsRouteLoop = false;

	public Guard(char[,] map, Direction direction, Position startPosition)
	{
		Direction = direction.Copy();
		StartDirection = direction.Copy();
		Position = startPosition.Copy();
		StartPosition = startPosition.Copy();
		Map = (char[,])map.Clone();
	}

	public List<RouteStep> GetRoute()
	{
		var uniquePositions = new List<RouteStep>();
		while (Position.X < Map.GetLength(1) && Position.X >= 0 && Position.Y < Map.GetLength(0) && Position.Y >= 0)
		{
			if (uniquePositions.Any(rs => rs.Position.Equals(Position) && rs.Direction.Equals(Direction)))
			{
				IsRouteLoop = true;
				break;
			}
			else if(!uniquePositions.Any(rs => rs.Position.Equals(Position)))
			{
				uniquePositions.Add(new RouteStep(Position, Direction));
			}
			Step();
		}
		return uniquePositions;
	}

	public Position Step()
	{
		var nextPosition = new Position(Position.X + Direction.X, Position.Y + Direction.Y);
		if (nextPosition.X < Map.GetLength(0)
			&& nextPosition.X >= 0
			&& nextPosition.Y < Map.GetLength(1)
			&& nextPosition.Y >= 0
			&& Map[nextPosition.Y, nextPosition.X] == '#')
		{
			if (Direction.X == 0 && Direction.Y == -1)
			{
				Direction.X = 1;
				Direction.Y = 0;
			}
			else if (Direction.X == 1 && Direction.Y == 0)
			{
				Direction.X = 0;
				Direction.Y = 1;
			}
			else if (Direction.X == 0 && Direction.Y == 1)
			{
				Direction.X = -1;
				Direction.Y = 0;
			}
			else if (Direction.X == -1 && Direction.Y == 0)
			{
				Direction.X = 0;
				Direction.Y = -1;
			}
			Step();
		}
		else
		{
			if (nextPosition.X < Map.GetLength(1)
			&& nextPosition.X >= 0
			&& nextPosition.Y < Map.GetLength(0)
			&& nextPosition.Y >= 0)
			{ Map[nextPosition.Y, nextPosition.X] = 'X'; }
			Position = nextPosition;
		}
		return Position;
	}
}

public class RouteStep
{
	public Position Position;
	public Direction Direction;

	public RouteStep(Position position, Direction direction)
	{
		Position = position.Copy();
		Direction = direction.Copy();
	}
}

public class Position
{
	public int X;
	public int Y;

	public Position(int x, int y)
	{
		X = x;
		Y = y;
	}

	public bool Equals(Position other)
	{
		return this.X == other.X && this.Y == other.Y;
	}

	public Position Copy()
	{
		return new Position(X, Y);
	}
}

public class Direction
{
	public int X;
	public int Y;

	public Direction(int x, int y)
	{
		X = x;
		Y = y;
	}

	public bool Equals(Direction other)
	{
		return this.X == other.X && this.Y == other.Y;
	}

	public Direction Copy()
	{
		return new Direction(X, Y);
	}
}