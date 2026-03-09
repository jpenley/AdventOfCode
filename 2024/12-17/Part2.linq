<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Collections.Concurrent</Namespace>
</Query>

//Register A: 729
//Register B: 0
//Register C: 0
//
//Program: 0,1,5,4,3,0

//0,3,5,4,3,0
//2, 4, 1, 3, 7, 5, 4, 0, 1, 3, 0, 3, 5, 5, 3, 0

long regA = 117440;
long regB = 0;
long regC = 0;

int[] program = { 0,3,5,4,3,0 };

int processorCount = Environment.ProcessorCount;
var maxSearch = 100_000_000;
int chunkSize = maxSearch/processorCount; // Adjust based on performance needs

var solutions = new ConcurrentBag<long>();

Parallel.For(0, processorCount, threadIndex =>
{
	long start = (threadIndex * chunkSize + 1);
	long end = ((threadIndex + 1) * chunkSize);

	for (long a = start; a <= end; a++)
	{
		{
			regA = a;
			var output = RunProgram(program, regA, regB, regC);
			if (String.Join(',',output)==String.Join(',',program))
			{
				solutions.Add(a);
				break;
			}
		}
	}
});

regA = solutions.Any() ? solutions.Min() : -1;
Console.WriteLine($"Program is: {String.Join(',', program)} and Register A is: {regA}");
Console.WriteLine($"Program output is: {String.Join(',', RunProgram(program, regA, regB, regC))}");

List<long> RunProgram(int[] program, long regA, long regB, long regC)
{
	var outputs = new List<long>();
	for (var i = 0; i < program.Length; i += 2)
	{
		var instruction = program[i];
		var comboOp = GetValue(program[i + 1], regA, regB, regC);
		var literalOp = program[i + 1];

		switch (instruction)
		{
			case 0:
				regA = (int)(regA / Math.Pow(2, comboOp));
				break;
			case 1:
				regB = regB ^ literalOp;
				break;
			case 2:
				regB = comboOp % 8;
				break;
			case 3:
				i = regA > 0 ? literalOp - 2 : i;
				break;
			case 4:
				regB = regB ^ regC;
				break;
			case 5:
				outputs.Add(comboOp % 8);
				break;
			case 6:
				regB = (int)(regA / Math.Pow(2, comboOp));
				break;
			case 7:
				regC = (int)(regA / Math.Pow(2, comboOp));
				break;
		}
	}

	return outputs;
}

long GetValue(int comboOp, long regA, long regB, long regC)
{
	switch (comboOp)
	{
		case 0:
		case 1:
		case 2:
		case 3:
			return comboOp;
		case 4:
			return regA;
		case 5:
			return regB;
		case 6:
			return regC;
		default:
			return -1;
	}
}