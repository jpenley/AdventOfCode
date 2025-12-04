using AoC2025;
using Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

var services = new ServiceCollection();

services.AddSingleton<ILineLoader, LineLoader>();

services.Scan(scan => scan
    .FromApplicationDependencies()
    .AddClasses(classes => classes.AssignableTo<IDay>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime());

var serviceProvider = services.BuildServiceProvider();

var days = serviceProvider.GetServices<IDay>().ToList();

var years = days.Select(d => d.Year)
    .Distinct()
    .OrderBy(y => y)
    .ToList();

foreach (var year in years)
{
    Console.WriteLine($"Year: {year}");
    var daysInYear = days
        .Where(d => d.Year == year)
        .OrderBy(d => d.Name)
        .ToList();
    foreach (var day in daysInYear)
    {
        Console.WriteLine($"  {day.Name}");
        Console.WriteLine($"    Part 1: {day.Part1()}");
        Console.WriteLine($"    Part 2: {day.Part2()}");
    }
}