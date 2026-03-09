<Query Kind="Program" />

public class Program
{
	// Function to generate the next secret number from the current one
	private static uint GetNextSecret(uint currentSecret)
	{
		// Step 1: Multiply by 64, XOR with current, then modulo 16777216
		uint stepVal = currentSecret * 64;
		currentSecret ^= stepVal;
		currentSecret %= 16777216;

		// Step 2: Divide by 32 (floor), XOR with current, then modulo 16777216
		stepVal = currentSecret / 32;
		currentSecret ^= stepVal;
		currentSecret %= 16777216;

		// Step 3: Multiply by 2048, XOR with current, then modulo 16777216
		stepVal = currentSecret * 2048;
		currentSecret ^= stepVal;
		currentSecret %= 16777216;

		return currentSecret;
	}

	public static void Main()
	{
		var initialSecrets = File.ReadAllLines(@"e:\source\advent2024\12-22\numbers.txt").Select(n=>uint.Parse(n)).ToList();

		ulong total = 0;

		foreach (var secret in initialSecrets)
		{
			uint current = secret;
			// Generate 2000 new secret numbers
			for (int i = 0; i < 2000; i++)
			{
				current = GetNextSecret(current);
			}
			total += current;
		}

		Console.WriteLine(total);
	}
}