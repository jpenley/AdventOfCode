<Query Kind="Statements" />

var file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-05\manualUpdates.txt");

var rules = new Dictionary<int, List<int>>();
var updates = new List<List<int>>();

foreach (var line in file)
{
	if (line.IndexOf('|') > -1)
	{
		var rule = line.Split('|');
		var key = int.Parse(rule[0]);
		var value = int.Parse(rule[1]);
		if (!rules.ContainsKey(key))
		{
			rules[key] = new List<int>();
		}
		rules[key].Add(value);
	}
	if (line.IndexOf(',') > -1)
	{
		var parts = line.Split(',');
		var update = new List<int>();
		foreach (var part in parts)
		{
			update.Add(int.Parse(part));
		}
		updates.Add(update);
	}
}

var correctSum = 0;
var incorrectSum = 0;
foreach (var update in updates)
{
	if (IsCorrectOrder(update))
	{
		correctSum += GetMiddlePage(update);
	}
	else
	{
		while (!IsCorrectOrder(update))
		{
			FixUpdate(update);
		}
		incorrectSum += GetMiddlePage(update);
	}
}

Console.WriteLine($"Correctly ordered updates sum:{correctSum}");
Console.WriteLine($"Incorrectly ordered updates sum:{incorrectSum}");

int GetMiddlePage(List<int> update)
{
	var middle = update[update.Count / 2];
	return middle;
}

List<int> FixUpdate(List<int> update)
{
	for (var i = 0; i < update.Count; i++)
	{
		List<int> rule;
		if (rules.TryGetValue(update[i], out rule))
		{
			for (var x = 0; x < i; x++)
			{
				if (rule.Contains(update[x]))
				{
					var page = update[i];
					update.RemoveAt(i);
					update.Insert(x, page);
				}
			}
		}
	}
	return update;
}

bool IsCorrectOrder(List<int> update)
{
	for (var i = 0; i < update.Count; i++)
	{
		List<int> rule;
		if (rules.TryGetValue(update[i], out rule))
		{
			for (var x = i - 1; x > -1; x--)
			{
				if (rule.Contains(update[x])) return false;
			}
		}
	}
	return true;
}