<Query Kind="Statements" />

string[] file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-07\equations.txt");

var goodEquations = new List<Int64>();
for (int i = 0; i < file.Length; i++)
{
	Int64 answer = Int64.Parse(file[i].Split(':')[0]);
	var numbers = file[i].Split(':')[1].Trim().Split(' ').Select(n => int.Parse(n)).ToArray();
	if (FindEquation(answer, numbers)) goodEquations.Add(answer);
}

Console.WriteLine($"Sum of all possible equations: {goodEquations.Sum()}");

bool FindEquation(Int64 answer, int[] numbers)
{
	var iter = 0;
	while (iter < (int)Math.Pow(2, numbers.Length - 1))
	{
		var multiply = new bool[numbers.Length - 1];
		BuildMultiply(iter++, multiply);
		var result = Calculate(numbers, multiply);
		if (answer == result) return true;
	}
	return false;
}

Int64 Calculate(int[] numbers, bool[] multiply)
{
	Int64 value = numbers[0];
	for (var i = 0; i < multiply.Length; i++)
	{
		if (multiply[i])
		{
			value *= numbers[i + 1];
		}
		else
		{
			value += numbers[i + 1];
		}
	}
	return value;
}

void BuildMultiply(int iter, bool[] multiply)
{
	for (var i = 0; i < multiply.Length; i++)
	{
		multiply[i] = (((int)Math.Pow(2, i)) & iter) > 0;
	}
}