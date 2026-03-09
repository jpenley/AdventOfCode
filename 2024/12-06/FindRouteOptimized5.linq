<Query Kind="Program">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	//This Version to 1.825 seconds

	string[] file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-06\map.txt");

	var map = new HashSet<Position>();
	var maxCol = file[0].Length;
	var maxRow = file.Length;
	Position startingPosition = new Position();

	for (int r = 0; r < maxRow; r++)
	{
		var chars = file[r].ToArray();
		for (int c = 0; c < maxCol; c++)
		{
			if (chars[c] == '#')
			{
				map.Add(new Position(c, r));
			}
			if (chars[c] == '^')
			{
				startingPosition = (new Position(c, r));
			}
		}
	}

	var guard = new Guard(map, maxCol, maxRow, startingPosition);
	var uniquePositions = guard.GetRoute();
	var loopingRoutes = 0;
	Parallel.For(1, uniquePositions.Count, i =>
	{
		var p = uniquePositions[i];
		var newGuard = new Guard(map, maxCol, maxRow, startingPosition, p);
		newGuard.GetRoute();
		if (newGuard.IsRouteLoop)
		{
			Interlocked.Increment(ref loopingRoutes);
		}
	});

	Console.WriteLine($"{uniquePositions.Count()} positions in original route");
	Console.WriteLine($"There are {loopingRoutes} object positions that result in loops.");
}

public class Guard
{
	public Direction StartDirection = new Direction(0, -1);
	public static Position StartPosition;
	public HashSet<Position> Map;
	public bool IsRouteLoop = false;

	private int maxC;
	private int maxR;
	private Position? blockingObject;

	public Guard(HashSet<Position> map, int maxC, int maxR, Position startingPosition, Position? p = null)
	{
		Map = map;
		this.maxC = maxC;
		this.maxR = maxR;
		blockingObject = p;
		StartPosition = startingPosition;
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
			if (!uniqueRouteSteps.Add(currentStep))
			{
				IsRouteLoop = true;
				break;
			}

			uniquePositions.Add(currentStep.Position);
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
			&& (Map.Contains(nextPosition) || nextPosition.Equals(blockingObject)))
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
		HashCode.Combine(Position.GetHashCode(), Direction.GetHashCode());
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
