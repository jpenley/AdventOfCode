<Query Kind="Statements" />

string[] file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-07\equations.txt");

var goodEquations = new List<Int64>();
for (int i = 0; i < file.Length; i++)
{
	Int64 answer = Int64.Parse(file[i].Split(':')[0]);
	var numbers = file[i].Split(':')[1].Trim().Split(' ').ToArray();
	if (FindEquation(answer, numbers)) goodEquations.Add(answer);
}

Console.WriteLine($"Sum of all possible equations: {goodEquations.Sum()}");

bool FindEquation(Int64 answer, string[] numbers)
{
	var opsLists = GenerateArrays(numbers.Length - 1, new string[] { "+", "*", "|" });

	foreach (var opsList in opsLists)
	{
		var result = Calculate(numbers, opsList);
		if (answer == result) return true;
	}
	return false;
}

Int64 Calculate(string[] numbers, string[] ops)
{
	var sb = new StringBuilder();
	sb.Append(numbers[0]);
	for (var i = 0; i < ops.Length; i++)
	{
		Int64 left, right;
		switch (ops[i])
		{
			case "+":
				left = Int64.Parse(sb.ToString());
				right = Int64.Parse(numbers[i + 1]);
				sb.Clear();
				sb.Append(left + right);
				break;
			case "*":
				left = Int64.Parse(sb.ToString());
				right = Int64.Parse(numbers[i + 1]);
				sb.Clear();
				sb.Append(left * right);
				break;
			case "|":
				sb.Append(numbers[i + 1]);
				break;
		};
	}
	return Int64.Parse(sb.ToString());
}

static List<string[]> GenerateArrays(int length, string[] possibleValues)
{
	List<string[]> result = new List<string[]>();
	GenerateArraysRecursive(length, possibleValues, new string[length], 0, result);
	return result;
}

static void GenerateArraysRecursive(int length, string[] possibleValues, string[] currentArray, int index, List<string[]> result)
{
	if (index == length)
	{
		// Base case: Reached the end of the array, add a copy to the result
		result.Add((string[])currentArray.Clone());
		return;
	}

	// Recursive step: Try each possible value at the current index
	foreach (string value in possibleValues)
	{
		currentArray[index] = value;
		GenerateArraysRecursive(length, possibleValues, currentArray, index + 1, result);
	}
}