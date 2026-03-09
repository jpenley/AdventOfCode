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
		if(grid[r,c] == 'X'){
			matches += FindXmas(r,c);
		}
	}
}

matches.Dump();

int FindXmas(int row, int col){
	var words = new List<string>();
	//up left
	if (row - 3 >= 0 && col -3 >= 0)
	{
		char[] t = { grid[row, col], grid[row - 1, col - 1], grid[row - 2, col - 2], grid[row - 3, col - 3] };
		words.Add(new string(t));
	}
	//up
	if (row - 3 >= 0)
	{
		char[] t = { grid[row, col], grid[row - 1, col], grid[row - 2, col], grid[row - 3, col] };
		words.Add(new string(t));
	}
	//up right
	if (row - 3 >= 0 && col + 3 < maxCols)
	{
		char[] t = { grid[row, col], grid[row - 1, col + 1], grid[row - 2, col + 2], grid[row - 3, col + 3] };
		words.Add(new string(t));
	}
	//left
	if (col - 3 >= 0)
	{
		char[] t = { grid[row, col], grid[row, col - 1], grid[row, col - 2], grid[row, col - 3] };
		words.Add(new string(t));
	}
	//right
	if (col + 3 < maxCols)
	{
		char[] t = { grid[row, col], grid[row, col + 1], grid[row, col + 2], grid[row, col + 3] };
		words.Add(new string(t));
	}
	//down left
	if (row + 3 < maxRows && col - 3 >= 0)
	{
		char[] t = { grid[row, col], grid[row + 1, col - 1], grid[row + 2, col - 2], grid[row + 3, col - 3] };
		words.Add(new string(t));
	}
	//down
	if (row + 3 < maxRows)
	{
		char[] t = { grid[row, col], grid[row + 1, col], grid[row + 2, col], grid[row + 3, col] };
		words.Add(new string(t));
	}
	//down right
	if (row + 3 < maxRows && col + 3 <maxCols)
	{
		char[] t = { grid[row, col], grid[row + 1, col + 1], grid[row + 2, col + 2], grid[row + 3, col + 3] };
		words.Add(new string(t));
	}

	return words.Where(w => w == "XMAS").Count();
}