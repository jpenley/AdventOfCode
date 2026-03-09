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

	public bool Safe => CheckSafety();

	private bool CheckSafety()
	{
		var differences = new List<int>();
		for (var i = 0; i < _values.Count - 1; i++)
		{
			differences.Add(_values[i]-_values[i+1]);
		}
		var sum = differences.Sum();
		var absSum = differences.Sum(d => Math.Abs(d));
		return Math.Abs(sum) == absSum && !differences.Any(d => d == 0 || Math.Abs(d) > 3);
	}
}