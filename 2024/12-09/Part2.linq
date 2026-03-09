<Query Kind="Statements" />

string[] file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-09\diskMap.txt");

var diskMap = file[0].ToArray().Select(d => int.Parse(d.ToString())).ToArray();

var diskSize = diskMap.Sum();

var disk = new char[diskSize];

var fileId = 0;
var iDisk = 0;
for (var i = 0; i < diskMap.Length; i++)
{
	if (i % 2 == 0)
	{
		Enumerable.Repeat(Convert.ToChar(fileId + Convert.ToInt32('0')), diskMap[i]).ToArray().CopyTo(disk, iDisk);
		iDisk += diskMap[i];
		fileId++;
	}
	else
	{
		Enumerable.Repeat('.', diskMap[i]).ToArray().CopyTo(disk, iDisk);
		iDisk += diskMap[i];
	}
}
//String.Join(null, disk).Dump();

Int64 checksum = 0;
for (var i = disk.Length - 1; i > -1; i--)
{
	if (disk[i] == '.') continue;
	fileId = disk[i] - Convert.ToInt32('0');
	var fileEnd = i;
	while (i > 1 && disk[i - 1] == Convert.ToChar(fileId + Convert.ToInt32('0')))
	{
		i--;
	}
	var spaceStart = FindSpace(fileEnd - i + 1, i);
	if (spaceStart < i) MoveFile(fileId, spaceStart, fileEnd - i + 1, i);
}

for (var i = 0; i < disk.Length; i++)
{
	if(disk[i]!='.') checksum += (disk[i] - Convert.ToInt32('0')) * i;
}

	Console.WriteLine($"The file checksum is {checksum}");

int FindSpace(int length, int end)
{
	for (var i = 0; i < end; i++)
	{
		var spaceStart = 0;
		if (disk[i] == '.')
		{
			spaceStart = i;
			while (disk[i] == '.')
			{
				i++;
			}
			if (i - spaceStart >= length) return spaceStart;
		}
	}
	return end;
}

bool MoveFile(int fileId, int spaceStart, int length, int fileStart)
{
	Enumerable.Repeat(Convert.ToChar(fileId + Convert.ToInt32('0')), length).ToArray().CopyTo(disk, spaceStart);
	Enumerable.Repeat('.', length).ToArray().CopyTo(disk, fileStart);
	//String.Join(null, disk).Dump();
	return true;
}