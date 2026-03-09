<Query Kind="Statements">
  <Namespace>System.Numerics</Namespace>
</Query>

var file = File.ReadAllLines(@"e:\source\advent2024\12-24\logicFixed.txt");

var wires = new SortedDictionary<string, bool>();
var rules = new List<Logic>();

foreach (var line in file)
{
	var lineParts = line.Split(':');
	if (lineParts.Length == 2) wires[lineParts[0]] = lineParts[0].StartsWith("x");
	lineParts = line.Split(' ');
	if (lineParts.Length == 5) rules.Add(new Logic { Wire1 = lineParts[0], Wire2 = lineParts[2], Op = lineParts[1], Out = lineParts[4] });
}


var startRules = new List<Logic>();
var intermediateRules = new List<Logic>();
var finalRules = new List<Logic>();
var outXRef = new SortedDictionary<string, string>();

foreach (var rule in rules)
{
	if (rule.Wire1.StartsWith("x") && rule.Wire2.StartsWith("y") && !rule.Out.StartsWith("z"))
	{
		var newRule = new Logic
		{
			Wire1 = rule.Wire1,
			Wire2 = rule.Wire2,
			Op = rule.Op,
			Out = $"({rule.Wire1}{rule.Op}{rule.Wire2})"
		};
		startRules.Add(newRule);
		outXRef[rule.Out] = newRule.Out;
		continue;
	}
	if (rule.Wire2.StartsWith("x") && rule.Wire1.StartsWith("y") && !rule.Out.StartsWith("z"))
	{
		var newRule = new Logic
		{
			Wire1 = rule.Wire2,
			Wire2 = rule.Wire1,
			Op = rule.Op,
			Out = $"({rule.Wire2}{rule.Op}{rule.Wire1})"
		};
		startRules.Add(newRule);
		outXRef[rule.Out] = newRule.Out;
		continue;
	}
	if (rule.Out.StartsWith("z"))
	{
		finalRules.Add(rule);
		continue;
	}
	intermediateRules.Add(new Logic
	{
		Wire1 = rule.Wire1,
		Wire2 = rule.Wire2,
		Op = rule.Op,
		Out = rule.Out
	});
}

while (intermediateRules.Any(r => !r.Wire1.StartsWith("(") || !r.Wire2.StartsWith("("))){
	var newIntermediateRules = new List<Logic>();
	foreach (var rule in intermediateRules)
	{
		Logic newRule;
		if (rule.Wire1.StartsWith("(") && rule.Wire2.StartsWith("("))
		{
			newRule = (new Logic
			{
				Wire1 = rule.Wire1,
				Wire2 = rule.Wire2,
				Op = rule.Op,
				Out = rule.Out
			});
		}
		else
		{
			newRule = (new Logic
			{
				Wire1 = outXRef.ContainsKey(rule.Wire1) ? outXRef[rule.Wire1] : rule.Wire1,
				Wire2 = outXRef.ContainsKey(rule.Wire2) ? outXRef[rule.Wire2] : rule.Wire2,
				Op = rule.Op,
				Out = rule.Out
			});
			if (newRule.Wire1.StartsWith("(") && newRule.Wire2.StartsWith("("))
			{
				newRule.Out = $"({newRule.Wire1}{newRule.Op}{newRule.Wire2})";
				outXRef[rule.Out] = newRule.Out;
			}
		}
		newIntermediateRules.Add(newRule);
	}
	intermediateRules = newIntermediateRules;
}

var newFinalRules = new List<Logic>();
foreach (var rule in finalRules)
{
	newFinalRules.Add(new Logic
	{
		Wire1 = outXRef.ContainsKey(rule.Wire1) ? outXRef[rule.Wire1] : rule.Wire1,
		Wire2 = outXRef.ContainsKey(rule.Wire2) ? outXRef[rule.Wire2] : rule.Wire2,
		Op = rule.Op,
		Out = rule.Out
	});
}
finalRules = newFinalRules;

startRules.Sort((i,j)=>i.Out.CompareTo(j.Out));
intermediateRules.Sort((i,j)=>i.Out.CompareTo(j.Out));
finalRules.Sort((i,j)=>i.Out.CompareTo(j.Out));
finalRules.ForEach(r=>Console.WriteLine($"{r.Wire1} | {r.Wire2} | {r.Op} | {r.Out}\n"));




var i = 0;
while (rules.Count > 0)
{
	var r = rules[i];
	if (wires.ContainsKey(r.Wire1) && wires.ContainsKey(r.Wire2))
	{
		wires[r.Out] = r.Op switch
		{
			"AND" => wires[r.Wire1] && wires[r.Wire2],
			"OR" => wires[r.Wire1] || wires[r.Wire2],
			"XOR" => (wires[r.Wire1] && !wires[r.Wire2]) || (!wires[r.Wire1] && wires[r.Wire2])
		};
		rules.Remove(r);
		i = 0;
	}
	i++;
	if (i == rules.Count) i = 0;
}

var zWires = wires.Where(i => i.Key.StartsWith("z")).Select(z => z.Value).ToArray();

var sOut = new string(zWires.Select(z => z ? '1' : '0').Reverse().ToArray());

BigInteger zOut = 0;
for (var b = 0; b < zWires.Length; b++)
{
	if (zWires[b])
		zOut += (BigInteger)Math.Pow(2, b);
}

sOut.Dump();
zOut.Dump();

//Hand debugged

var s = new string[] { "z21", "gds", "jrs", "wrk", "fph", "z15", "z34", "cqk" };
Array.Sort(s);
var js = String.Join(',', s);
js.Dump();


struct Logic
{
	public string Wire1;
	public string Wire2;
	public string Op;
	public string Out;
}