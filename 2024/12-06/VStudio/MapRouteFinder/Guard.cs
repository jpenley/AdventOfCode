namespace MapRouteFinder;

public class Guard
{
  public Direction StartDirection;
  public Position StartPosition;
  public char[,] Map;
  public bool IsRouteLoop = false;

  private readonly int _maxC;
  private readonly int _maxR;

  public Guard(char[,] map)
  {
    Map = map;
    _maxC = Map.GetLength(1);
    _maxR = Map.GetLength(0);
    GetStart();
  }

  private void GetStart()
  {
    for (var r = 0; r < _maxC; r++)
    {
      for (var c = 0; c < _maxR; c++)
      {
        if (Map[r, c] != '^') continue;
        StartPosition = new Position(c, r);
        StartDirection = new Direction(0, -1);
        return;
      }
    }
  }

  public List<Position> GetRoute()
  {
    var currentStep = new RouteStep(StartPosition, StartDirection);
    var uniquePositions = new HashSet<Position>();
    var uniqueRouteSteps = new HashSet<RouteStep>();
    while (currentStep.Position.C < _maxC
           && currentStep.Position.C >= 0
           && currentStep.Position.R < _maxR
           && currentStep.Position.R >= 0)
    {
      if (!uniqueRouteSteps.Add(currentStep))
      {
        IsRouteLoop = true;
        break;
      }

      uniquePositions.Add(currentStep.Position);
      currentStep = Step(currentStep);
    }
		
    return uniquePositions.ToList();
  }

  public RouteStep Step(RouteStep step)
  {
    var nextStep = new RouteStep(new Position(step.Position.C + step.Direction.C, step.Position.R + step.Direction.R), step.Direction);
    var nextPosition = nextStep.Position;
    if (nextPosition.C >= _maxC
        || nextPosition.C < 0
        || nextPosition.R >= _maxR
        || nextPosition.R < 0
        || Map[nextPosition.R, nextPosition.C] != '#') return nextStep;
    switch (step.Direction.C)
    {
      case 0 when step.Direction.R == -1:
        step.Direction.C = 1;
        step.Direction.R = 0;
        break;
      case 1 when step.Direction.R == 0:
        step.Direction.C = 0;
        step.Direction.R = 1;
        break;
      case 0 when step.Direction.R == 1:
        step.Direction.C = -1;
        step.Direction.R = 0;
        break;
      case -1 when step.Direction.R == 0:
        step.Direction.C = 0;
        step.Direction.R = -1;
        break;
    }
    nextStep = Step(step);
    return nextStep;
  }
}