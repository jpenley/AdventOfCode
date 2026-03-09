namespace MapRouteFinder;

internal class Program
{
  public static void Main(string[] args)
  {
    var file = File.ReadAllLines(@"D:\Paylocity\advent2024\12-06\map.txt");

    var map = new char[file.Length, file[0].Length];
    var maxCol = map.GetLength(1);
    var maxRow = map.GetLength(0);

    for (var r = 0; r < maxRow; r++)
    {
      var chars = file[r].ToArray();
      for (var c = 0; c < maxCol; c++)
      {
        map[r, c] = chars[c];
      }
    }

    var guard = new Guard(map);
    var uniquePositions = guard.GetRoute();
    var loopingRoutes = 0;
    Parallel.For(1, uniquePositions.Count, i =>
    {
      var p = uniquePositions[i];
      var newMap = (char[,])map.Clone();
      newMap[p.R, p.C] = '#';
      var newGuard = new Guard(newMap);
      newGuard.GetRoute();
      if (newGuard.IsRouteLoop)
      {
        Interlocked.Increment(ref loopingRoutes);
      }
    });

    Console.WriteLine($"{uniquePositions.Count()} positions in original route");
    Console.WriteLine($"There are {loopingRoutes} object positions that result in loops.");
  }
}