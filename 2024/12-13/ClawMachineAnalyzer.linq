<Query Kind="Statements">
  <NuGetReference>MathNet.Numerics</NuGetReference>
  <Namespace>MathNet.Numerics</Namespace>
  <Namespace>MathNet.Numerics.LinearAlgebra</Namespace>
  <Namespace>MathNet.Numerics.LinearAlgebra.Double</Namespace>
</Query>

//var file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-13\example.txt");
var file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-13\machines.txt");

var machines = new List<ClawMachine>();
for (var line = 0; line < file.Length; line = line + 4)
{
	var match1 = Regex.Match(file[line], @"(?<label>\w+)\s*:\s*X(?<xOp>[\+\-])(?<xValue>\d+),\s*Y(?<yOp>[\+\-])(?<yValue>\d+)");
	var ax = int.Parse(match1.Groups["xValue"].Value);
	var ay = int.Parse(match1.Groups["yValue"].Value);
	var match2 = Regex.Match(file[line + 1], @"(?<label>\w+)\s*:\s*X(?<xOp>[\+\-])(?<xValue>\d+),\s*Y(?<yOp>[\+\-])(?<yValue>\d+)");
	var bx = int.Parse(match2.Groups["xValue"].Value);
	var by = int.Parse(match2.Groups["yValue"].Value);
	var match3 = Regex.Match(file[line + 2], @"(?<label>\w+)\s*:\s*X=(?<xValue>\d+),\s*Y=(?<yValue>\d+)");
	int px = int.Parse(match3.Groups["xValue"].Value);
	int py = int.Parse(match3.Groups["yValue"].Value);

	machines.Add(new ClawMachine { ButtonA = (ax, ay), ButtonB = (bx, by), Prize = (px + 10000000000000, py + 10000000000000) });
}

long totalCost = 0;
int winnableMachines = 0;

foreach (var machine in machines)
{
	var result = machine.FindCheapestWin();
	if (result.HasValue)
	{
		(long cost, long aPresses, long bPresses) = result.Value;
		totalCost += cost;
		winnableMachines++;
		Console.WriteLine($"Machine winnable with {aPresses} A presses and {bPresses} B presses. Cost: {cost}");
	}
	else
	{
		Console.WriteLine("Machine not winnable.");
	}
}

Console.WriteLine($"\nTotal prizes winnable: {winnableMachines}");
Console.WriteLine($"Total token cost: {totalCost}");


public class ClawMachine
{
	public (int X, int Y) ButtonA { get; set; }
	public (int X, int Y) ButtonB { get; set; }
	public (long X, long Y) Prize { get; set; }

	public (long, long, long)? FindCheapestWin()
	{

		long minCost = long.MaxValue;
		long bestA = 0;
		long bestB = 0;

		long prizeX = Prize.X;
		long prizeY = Prize.Y;
		int buttonAX = ButtonA.X;
		int buttonAY = ButtonA.Y;
		int buttonBX = ButtonB.X;
		int buttonBY = ButtonB.Y;

		int gcdX = GCD(buttonAX, buttonBX);
		int gcdY = GCD(buttonAY, buttonBY);

		if (prizeX % gcdX != 0 || prizeY % gcdY != 0)
		{
			return null;
		}

		for (long a = 0; a <= prizeX / buttonAX; a++)
		{
			long remainingX = prizeX - a * buttonAX;
			long remainingY = prizeY - a * buttonAY;

			if (remainingX % buttonBX == 0 && remainingY % buttonBY == 0)
			{
				long b = remainingX / buttonBX;
				long cost = 3 * a + b;

				if (cost >= minCost)
					break;

				if (cost < minCost)
				{
					minCost = cost;
					bestA = a;
					bestB = b;

					long maxAffordableA = (minCost - b) / 3;
					if (maxAffordableA < prizeX / buttonAX)
					{
						a = maxAffordableA - 1;
					}
				}
			}
		}

		if (minCost != long.MaxValue)
		{
			return (minCost, bestA, bestB);
		}
		else
		{
			return null;
		}
	}

	private static int GCD(int a, int b)
	{
		while (b != 0)
		{
			int temp = b;
			b = a % b;
			a = temp;
		}
		return a;
	}
}