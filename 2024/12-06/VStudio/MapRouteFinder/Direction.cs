namespace MapRouteFinder;

public struct Direction(int c, int r)
{
  public int C = c;
  public int R = r;

  public override bool Equals(object? other) =>
    other is Direction dir
    && dir.C == C
    && dir.R == R;

  public override int GetHashCode() => (C.ToString("D3") + R.ToString("D3")).GetHashCode();

  public static bool operator ==(Direction left, Direction right)
  {
    return left.Equals(right);
  }

  public static bool operator !=(Direction left, Direction right)
  {
    return !(left == right);
  }
}