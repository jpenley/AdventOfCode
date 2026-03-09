<Query Kind="Statements" />

List<int> list1 = new List<int>();
List<int> list2 = new List<int>();


var sr = new StreamReader(@"D:\Paylocity\advent2024\12-01\lists.txt");
var line = sr.ReadLine();
while(line != null){
	list1.Add(int.Parse(line.Substring(0,5)));
	list2.Add(int.Parse(line.Substring(8,5)));
	line = sr.ReadLine();
}
sr.Close();

list1.Sort();
list2.Sort();

var listDistance = new List<int>();

for(var i = 0; i < list1.Count; i++){
	listDistance.Add(Math.Abs(list1[i]-list2[i]));
}

listDistance.Sum().Dump();