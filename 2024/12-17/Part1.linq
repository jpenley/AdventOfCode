<Query Kind="Statements" />

//Register A: 729
//Register B: 0
//Register C: 0
//
//Program: 0,1,5,4,3,0

var regA = 51342988;
var regB = 0;
var regC = 0;

int[] program = { 2, 4, 1, 3, 7, 5, 4, 0, 1, 3, 0, 3, 5, 5, 3, 0 };

var outputs = new List<int>();
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

Console.WriteLine($"Program output is: {String.Join(',', outputs)}");

int GetValue(int comboOp, int regA, int regB, int regC)
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