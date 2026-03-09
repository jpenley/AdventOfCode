import os

def count_ways(design, patterns, memo):
    if design == "":
        return 1

    if design in memo:
        return memo[design]

    count = 0
    for pattern in patterns:
        if design.startswith(pattern):
            remaining = design[len(pattern):]
            count += count_ways(remaining, patterns, memo)

    memo[design] = count
    return count

def main():
    # Input
    with open("e:/source/advent2024/12-19/towels.txt", "r") as file:
        lines = file.readlines()

    patterns = sorted([pattern.strip() for pattern in lines[0].strip().split(',')], key=len)
    designs = lines[2:]

    # Calculate the number of ways for each design
    memo = {}
    total_ways = 0

    for design in designs:
        design = design.strip()
        ways = count_ways(design, patterns, memo)
        total_ways += ways

    print(f"Total Ways: {total_ways}")

if __name__ == "__main__":
    main()