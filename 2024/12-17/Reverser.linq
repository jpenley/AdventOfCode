<Query Kind="Statements" />

var a = 51342988;
var b = 0;
var c = 0;

var output = new List<int>();

while (a > 0)
{
	b = mod8(a);//When reversed just add enough to a to make the mod right.
	b = shift3(b);//never more that 7 here
	c = divbypow2(a, b);//divide by up to 128
	b = shiftbyc(b, c);//c maxs at 7
	b = shift3(b);//could be big but reversible

	a = div8(a);//a could be anything under 8

	output.Add(mod8(b));
}
String.Join(',', output).Dump();

var pq = new PriorityQueue<State, int>();
pq.Enqueue(new State(0, 0, 0, new List<int>()), 0);
pq.Enqueue(new State(1, 0, 0, new List<int>()), 1);
pq.Enqueue(new State(2, 0, 0, new List<int>()), 2);
pq.Enqueue(new State(3, 0, 0, new List<int>()), 3);
pq.Enqueue(new State(4, 0, 0, new List<int>()), 4);
pq.Enqueue(new State(5, 0, 0, new List<int>()), 5);
pq.Enqueue(new State(6, 0, 0, new List<int>()), 6);
pq.Enqueue(new State(7, 0, 0, new List<int>()), 7);

var targetA = 51342988;
int[] targetP = { 2, 4, 1, 3, 7, 5, 4, 0, 1, 3, 0, 3, 5, 5, 3, 0 };

while (true)
{
	a = mult8(a);
	b = shift3(b);
}


//this is reversible as is
int shift3(int b)
{
	return b ^ 3;
}

int mod8(int a)
{
	return a % 8;
}

int divbypow2(int a, int b)
{
	int[] p = { 1, 2, 4, 8, 16, 32, 64, 128 };
	return a / p[b];
}

int shiftbyc(int b, int c)
{
	return b ^ c;
}

int div8(int a)
{
	return a / 8;
}

int mult8(int a)
{
	return a * 8;
}

//int unmod8(int a)
//{
//
//}
//
//int undivbypow2(int a)
//{
//
//}

public class State(int a, int b, int c, List<int> output)
{
	public int A = a;
	public int B = b;
	public int C = c;
	public List<int> Output = output;
}