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

var fileIndex = new HashSet<char>();
Int64 checksum = 0;
var iDiskReverseRead = disk.Length - 1;
for (var i = 0; i < disk.Length; i++)
{
	if (disk[i] == '.')
	{
		while (disk[iDiskReverseRead] == '.')
		{
			iDiskReverseRead--;
		}
		disk[i] = disk[iDiskReverseRead];
		disk[iDiskReverseRead] = '.';
	}
	checksum += (disk[i] - Convert.ToInt32('0')) * i;
	if (i >= iDiskReverseRead - 1) break;
}
Console.WriteLine($"The file checksum is {checksum}");

