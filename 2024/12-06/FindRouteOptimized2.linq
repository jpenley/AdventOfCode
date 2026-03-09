<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

//This Version to 4.997 seconds

string[] file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-06\map.txt");

var map = new char[file.Length, file[0].Length];
var maxCol = map.GetLength(1);
var maxRow = map.GetLength(0);

for (int r = 0; r < maxRow; r++)
{
	var chars = file[r].ToArray();
	for (int c = 0; c < maxCol; c++)
	{
		map[r, c] = chars[c];
	}
}

var guard = new Guard(map);
var uniquePositions = guard.GetRoute();
var loopingRoutes = 0;
Parallel.ForEach(uniquePositions, p =>
{
	if (!p.Equals(uniquePositions.First()))
	{
		var newMap = (char[,])map.Clone();
		newMap[p.R, p.C] = '#';
		var newGuard = new Guard(newMap);
		newGuard.GetRoute();
		if (newGuard.IsRouteLoop) loopingRoutes++;
	}
});

Console.WriteLine($"{uniquePositions.Count()} positions in original route");
Console.WriteLine($"There are {loopingRoutes} object positions that result in loops.");

public class Guard
{
	public Direction StartDirection;
	public Position StartPosition;
	public char[,] Map;
	public bool IsRouteLoop = false;

	private int maxC;
	private int maxR;

	public Guard(char[,] map)
	{
		Map = map;
		maxC = Map.GetLength(1);
		maxR = Map.GetLength(0);
		GetStart();
	}

	private void GetStart()
	{
		for (var r = 0; r < maxC; r++)
		{
			for (var c = 0; c < maxR; c++)
			{
				if (Map[r, c] == '^')
				{
					StartPosition = new Position(c, r);
					StartDirection = new Direction(0, -1);
				}
			}
		}
	}

	public List<Position> GetRoute()
	{
		var currentStep = new RouteStep(StartPosition, StartDirection);
		var uniquePositions = new HashSet<Position>();
		var uniqueRouteSteps = new HashSet<RouteStep>();
		while (currentStep.Position.C < maxC
			&& currentStep.Position.C >= 0
			&& currentStep.Position.R < maxR
			&& currentStep.Position.R >= 0)
		{
			if (uniqueRouteSteps.Contains(currentStep))
			{
				IsRouteLoop = true;
				break;
			}
			if (!uniqueRouteSteps.Contains(currentStep))
			{
				uniqueRouteSteps.Add(currentStep);
			}
			if (!uniquePositions.Contains(currentStep.Position))
			{
				uniquePositions.Add(currentStep.Position);
			}
			currentStep = Step(currentStep);
		}
		return uniquePositions.ToList();
	}

	public RouteStep Step(RouteStep step)
	{
		var nextStep = new RouteStep(new Position(step.Position.C + step.Direction.C, step.Position.R + step.Direction.R), step.Direction);
		var nextPosition = nextStep.Position;
		if (nextPosition.C < maxC
			&& nextPosition.C >= 0
			&& nextPosition.R < maxR
			&& nextPosition.R >= 0
			&& Map[nextPosition.R, nextPosition.C] == '#')
		{
			if (step.Direction.C == 0 && step.Direction.R == -1)
			{
				step.Direction.C = 1;
				step.Direction.R = 0;
			}
			else if (step.Direction.C == 1 && step.Direction.R == 0)
			{
				step.Direction.C = 0;
				step.Direction.R = 1;
			}
			else if (step.Direction.C == 0 && step.Direction.R == 1)
			{
				step.Direction.C = -1;
				step.Direction.R = 0;
			}
			else if (step.Direction.C == -1 && step.Direction.R == 0)
			{
				step.Direction.C = 0;
				step.Direction.R = -1;
			}
			nextStep = Step(step);
		}
		return nextStep;
	}
}

public struct RouteStep
{
	public Position Position;
	public Direction Direction;

	public RouteStep(Position position, Direction direction)
	{
		Position = position;
		Direction = direction;
	}
	
	public override bool Equals(object other) =>
		other is RouteStep rs
		&& rs.Position.Equals(this.Position)
		&& rs.Direction.Equals(this.Direction);

	public override int GetHashCode() =>
		HashCode.Combine(Position.GetHashCode(),Direction.GetHashCode());
}

public struct Position
{
	public int C;
	public int R;

	public Position(int c, int r)
	{
		C = c;
		R = r;
	}

	public override bool Equals(object other) =>
		other is Position pos
		&& pos.C == this.C
		&& pos.R == this.R;

	public override int GetHashCode() =>
		(C.ToString("D3") + R.ToString("D3")).GetHashCode();
}

public struct Direction
{
	public int C;
	public int R;

	public Direction(int c, int r)
	{
		C = c;
		R = r;
	}

	public override bool Equals(object other) =>
		other is Direction dir
		&& dir.C == this.C
		&& dir.R == this.R;

	public override int GetHashCode() =>
		(C.ToString("D3") + R.ToString("D3")).GetHashCode();
}