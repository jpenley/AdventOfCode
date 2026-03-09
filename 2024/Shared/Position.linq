<Query Kind="Statements" />

public class Position(int x, int y)
{
	public int X = x;
	public int Y = y;

	public override bool Equals(object? other)
	{
		var pos = other as Position;
		return other is null
			? false :
			pos.X == X
		   	&& pos.Y == Y;
	}

	public override int GetHashCode()
	{
		return (X.ToString("D3") + Y.ToString("D3")).GetHashCode();
	}

	public static bool operator ==(Position left, Position right)
	{
		return left is null ? right is null : left.Equals(right);
	}

	public static bool operator !=(Position left, Position right)
	{
		return !(left is null ? right is null : left == right);
	}
}