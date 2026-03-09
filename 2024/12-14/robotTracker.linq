<Query Kind="Statements">
  <Namespace>System.Drawing</Namespace>
</Query>

//var file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-14\example.txt");
//var maxX = 11;
//var maxY = 7;

var file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-14\robots.txt");
var maxX = 101;
var maxY = 103;

var robots = new List<Robot>();

var pattern = @"[-+]?\d+";
foreach (var line in file)
{
	var matches = Regex.Matches(line, pattern);
	robots.Add(new Robot(
		new Position(int.Parse(matches[0].Value), int.Parse(matches[1].Value)),
		new Velocity(int.Parse(matches[2].Value), int.Parse(matches[3].Value)), maxX, maxY));
}

RobotsByQuadrant(100).Dump();
DrawRobots(8258, 8258);

void DrawRobots(int startTime, int endTime)
{
	for (var t = startTime; t <= endTime; t++)
	{
		var bitmap = new Bitmap(maxX, maxY);
		var locations = new HashSet<Position>();
		robots.ForEach(r => locations.Add(r.Move(t)));
		for (var y = 0; y < maxY; y++)
		{
			for (var x = 0; x < maxX; x++)
			{
				var currentPos = new Position(x, y);
				if (locations.Contains(currentPos))
				{
					bitmap.SetPixel(x,y, Color.White);
				}
			}
		}
		bitmap.Save(@"D:\Paylocity\advent2024\12-14\" + $"{t.ToString("D4")}.bmp");
	}
}

int RobotsByQuadrant(int time)
{
	var quad1 = 0;
	var quad2 = 0;
	var quad3 = 0;
	var quad4 = 0;

	foreach (var robot in robots)
	{
		var pos = robot.Move(time);
		if (pos.X < maxX / 2)
		{
			if (pos.Y < maxY / 2)
			{
				quad1++;
			}
			else if (pos.Y > maxY / 2)
			{
				quad2++;
			}
		}
		else if (pos.X > maxX / 2)
		{
			if (pos.Y < maxY / 2)
			{
				quad3++;
			}
			else if (pos.Y > maxY / 2)
			{
				quad4++;
			}
		}
	}

	return quad1 * quad2 * quad3 * quad4;
}

public class Robot(Position start, Velocity velocity, int maxX, int maxY)
{
	public Position Start = start;
	public Velocity Velocity = velocity;
	private int maxX = maxX;
	private int maxY = maxY;

	public Position Move(int time)
	{
		var shiftX = Velocity.X * time;
		var shiftY = Velocity.Y * time;
		var newX = (Start.X + shiftX) % maxX;
		var newY = (Start.Y + shiftY) % maxY;
		newX = newX < 0 ? maxX + newX : newX;
		newY = newY < 0 ? maxY + newY : newY;
		return new Position(newX, newY);
	}
}

public class Position(int x, int y)
{
	public int X = x;
	public int Y = y;

	public override bool Equals(object? other)
	{
		var pos = other as Position;
		return other is null
			? false :
			pos.X == X
		   	&& pos.Y == Y;
	}

	public override int GetHashCode()
	{
		return (X.ToString("D3") + Y.ToString("D3")).GetHashCode();
	}

	public static bool operator ==(Position left, Position right)
	{
		return left is null ? right is null : left.Equals(right);
	}

	public static bool operator !=(Position left, Position right)
	{
		return !(left is null ? right is null : left == right);
	}
}

public class Velocity(int x, int y)
{
	public int X = x;
	public int Y = y;
}