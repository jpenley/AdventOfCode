<Query Kind="Statements" />

var sr = new StreamReader(@"D:\Paylocity\advent2024\12-03\corruptdata.txt");
var file = sr.ReadToEnd();
sr.Close();

var patternMul = @"mul\((\d+),(\d+)\)";
var patternDo = @"do(?:n't)?";

var mulMatches = Regex.Matches(file, patternMul);
var doMatches = Regex.Matches(file, patternDo);

var dontFound = false;
var lastIndex = 0;
var ranges = new List<DoRange>();
int sum = 0;

foreach (Match doMatch in doMatches)
{
	if (doMatch.Value == "don't" && !dontFound)
	{
		ranges.Add(new DoRange(lastIndex, doMatch.Index));
		dontFound = true;
	}
	else if (doMatch.Value == "do" && dontFound)
	{
		lastIndex = doMatch.Index;
		dontFound = false;
	}
}


foreach (DoRange range in ranges)
{
	foreach (Match mulMatch in mulMatches)
	{
		if (range.InRange(mulMatch.Index))
		{
			var num1 = int.Parse(mulMatch.Groups[1].Value);
			var num2 = int.Parse(mulMatch.Groups[2].Value);
			sum += num1 * num2;
		}
	}
}

sum.Dump();

public class DoRange
{
	public int _start, _end;

	public DoRange(int start, int end)
	{
		_start = start;
		_end = end;
	}

	public bool InRange(int index)
	{
		return index > _start && index < _end;
	}
}