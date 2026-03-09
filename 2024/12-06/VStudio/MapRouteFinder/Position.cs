namespace MapRouteFinder;

public struct Position(int c, int r)
{
  public int C = c;
  public int R = r;

  public override bool Equals(object? other)
  {
    return other is Position pos
           && pos.C == C
           && pos.R == R;
  }

  public override int GetHashCode()
  {
    return (C.ToString("D3") + R.ToString("D3")).GetHashCode();
  }

  public static bool operator ==(Position left, Position right)
  {
    return left.Equals(right);
  }

  public static bool operator !=(Position left, Position right)
  {
    return !(left == right);
  }
}