<Query Kind="Statements">
  <Namespace>System.Numerics</Namespace>
</Query>

var file = File.ReadAllLines(@"e:\source\advent2024\12-24\logic.txt");

var wires = new SortedDictionary<string, bool>();
var rules = new List<Logic>();

foreach (var line in file)
{
	var lineParts = line.Split(':');
	if (lineParts.Length == 2) wires[lineParts[0]] = lineParts[1].Trim() == "1";
	lineParts = line.Split(' ');
	if (lineParts.Length == 5) rules.Add(new Logic { Wire1 = lineParts[0], Wire2 = lineParts[2], Op = lineParts[1], Out = lineParts[4] });
}


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

var zWires = wires.Where(i => i.Key.StartsWith("z")).Select(z=>z.Value).ToArray();

var sOut = new string(zWires.Select(z=>z?'1':'0').ToArray());

BigInteger zOut = 0;
for (var b = 0; b < zWires.Length; b++)
{
	if(zWires[b])
	zOut +=(BigInteger) Math.Pow(2,b);
}

sOut.Dump();
zOut.Dump();

struct Logic
{
	public string Wire1;
	public string Wire2;
	public string Op;
	public string Out;
}