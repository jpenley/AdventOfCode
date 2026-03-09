namespace MapRouteFinder;

public struct RouteStep(Position position, Direction direction)
{
  public Position Position = position;
  public Direction Direction = direction;

  public override bool Equals(object? other) =>
    other is RouteStep rs
    && rs.Position.Equals(Position)
    && rs.Direction.Equals(Direction);

  public override int GetHashCode() => HashCode.Combine(Position.GetHashCode(), Direction.GetHashCode());

  public static bool operator ==(RouteStep left, RouteStep right)
  {
    return left.Equals(right);
  }

  public static bool operator !=(RouteStep left, RouteStep right)
  {
    return !(left == right);
  }
}