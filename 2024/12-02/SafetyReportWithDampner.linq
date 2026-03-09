<Query Kind="Statements" />

var reports = new List<Report>();

var sr = new StreamReader(@"D:\Paylocity\advent2024\12-02\reports.txt");
var line = sr.ReadLine();
while (line != null)
{
	var valList = line.Split(' ').ToList();
	reports.Add(new Report(valList.Select(item => int.Parse(item)).ToList()));
	line = sr.ReadLine();
}
sr.Close();

var safe = reports.Where(r => r.Safe).ToList();
var unSafe = reports.Where(r => !r.Safe).ToList();

safe.Dump();

public class Report
{
	private List<int> _values;

	public Report(List<int> values)
	{
		_values = values;
	}

	public bool Safe => CheckSafety(_values);

	private bool CheckSafety(List<int> values, bool dampnerUsed = false)
	{
		var differences = new List<int>();

		for (var i = 1; i < values.Count; i++)
		{
			var difference = values[i] - values[i - 1];
			differences.Add(difference);
		}

		var rising = differences.Where(d => d > 0).Count() > differences.Where(d => d < 0).Count();

		for (var i = 0; i < differences.Count; i++)
		{
			if (!dampnerUsed)
			{
				if (rising && differences[i] < 0)
				{
					var newValues = new List<int>(values);
					var lastValues = new List<int>(values);
					newValues.RemoveAt(i+1);
					lastValues.RemoveAt(0);
					var result = CheckSafety(newValues, true) || CheckSafety(lastValues, true);
					return result;
				}
				if (!rising && differences[i] > 0)
				{
					var newValues = new List<int>(values);
					var lastValues = new List<int>(values);
					newValues.RemoveAt(i + 1);
					lastValues.RemoveAt(0);
					var result = CheckSafety(newValues, true) || CheckSafety(lastValues, true);
					return result;
				}
				if (differences[i] == 0)
				{
					var newValues = new List<int>(values);
					var lastValues = new List<int>(values);
					newValues.RemoveAt(i + 1);
					lastValues.RemoveAt(0);
					var result = CheckSafety(newValues, true) || CheckSafety(lastValues, true);
					return result;
				}
				if (Math.Abs(differences[i]) > 3)
				{
					var newValues = new List<int>(values);
					var lastValues = new List<int>(values);
					newValues.RemoveAt(i + 1);
					lastValues.RemoveAt(0);
					var result = CheckSafety(newValues, true) || CheckSafety(lastValues, true);
					return result;
				}
			}
			else if ((rising && differences[i] < 0)
			|| (!rising && differences[i] > 0)
			|| (differences[i] == 0)
			|| (Math.Abs(differences[i]) > 3))
			{
				return false;
			}
		}

		return true;
	}
}