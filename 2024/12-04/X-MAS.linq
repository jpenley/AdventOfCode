<Query Kind="Statements" />

var maxRows = 140;
var maxCols = 140;

var grid = new char[maxRows, maxCols];

var sr = new StreamReader(@"D:\Paylocity\advent2024\12-04\wordSearch.txt");
var line = sr.ReadLine();

var row = 0;
while (line != null)
{
	var chars = line.ToCharArray();
	var col = 0;
	foreach (var c in chars)
	{
		grid[row, col] = c;
		col++;
	}
	line = sr.ReadLine();
	row++;
}
sr.Close();

var matches = 0;
for (var r = 0; r < maxRows; r++)
{
	for (var c = 0; c < maxCols; c++)
	{
		//Console.WriteLine($"grid[{r},{c}]");
		if (grid[r, c] == 'A')
		{
			matches += FindXmas(r, c);
		}
	}
}

matches.Dump();

int FindXmas(int row, int col)
{
	if (row - 1 >= 0 && col - 1 >= 0 && row + 1 < maxRows && col + 1 < maxCols)
	{
		char[] a1 = { grid[row - 1, col - 1], grid[row, col], grid[row + 1, col + 1] };
		char[] a2 = { grid[row - 1, col + 1], grid[row, col], grid[row + 1, col - 1] };
		var w1 = new string(a1);
		var w2 = new string(a2);
		var w1r = new string(w1.Reverse().ToArray());
		var w2r = new string(w2.Reverse().ToArray());
		if((w1 == "MAS" || w1r == "MAS")&&(w2 == "MAS" || w2r == "MAS")) return 1;
	}
	return 0;
}