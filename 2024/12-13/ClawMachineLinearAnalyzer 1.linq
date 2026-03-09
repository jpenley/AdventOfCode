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

	machines.Add(new ClawMachine { ButtonA = (ax, ay), ButtonB = (bx, by), Prize = (px, py) });
}

long totalCost = 0;
foreach (var machine in machines)
{
	var result = machine.FindCheapestWin();
	if (result.HasValue && result.Value > 0)
	{
		var cost = result.Value;
		totalCost += (long)cost;
	}
}

Console.WriteLine($"Total token cost part 1: {totalCost}");

totalCost = 0;
foreach (var machine in machines)
{
	machine.Prize = (machine.Prize.X + 10000000000000, machine.Prize.Y + 10000000000000);
	var result = machine.FindCheapestWin();
	if (result.HasValue && result.Value > 0)
	{
		var cost = result.Value;
		totalCost += (long)cost;
	}
}

Console.WriteLine($"Total token cost part 2: {totalCost}");


public class ClawMachine
{
	public (int X, int Y) ButtonA { get; set; }
	public (int X, int Y) ButtonB { get; set; }
	public (long X, long Y) Prize { get; set; }

	public double? FindCheapestWin()
	{
		var detInverse = CalculateDeterminantInverse();
		var vectors = SolveEquations(detInverse);
		var cost = VerifyAndCalculateResult(vectors);
		return cost;
	}
	private double CalculateDeterminantInverse()
	{
		double det = ButtonA.X * ButtonB.Y - ButtonB.X * ButtonA.Y;
		return 1 / det;
	}

	private (double, double) SolveEquations(double detInverse)
	{
		double A = Math.Round((ButtonB.Y * Prize.X - ButtonB.X * Prize.Y) * detInverse);
		double B = Math.Round((ButtonA.X * Prize.Y - ButtonA.Y * Prize.X) * detInverse);
		return (A, B);
	}

	private double VerifyAndCalculateResult((double A, double B) v)
	{
		return v.A * ButtonA.X + v.B * ButtonB.X == Prize.X
			   && v.A * ButtonA.Y + v.B * ButtonB.Y == Prize.Y
			? 3 * v.A + v.B
			: 0;
	}
}