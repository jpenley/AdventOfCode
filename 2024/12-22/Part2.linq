<Query Kind="Program" />

public class Program
{
	// Maximum value for pruning
	const uint MODULO = 16777216;

	// Computes the next secret number from the current one
	private static uint GetNextSecret(uint currentSecret)
	{
		// Step 1: Multiply by 64, XOR with current, then prune
		uint stepVal = currentSecret * 64;
		currentSecret ^= stepVal;
		currentSecret %= MODULO;

		// Step 2: Divide by 32 (floor), XOR with current, then prune
		stepVal = currentSecret / 32;
		currentSecret ^= stepVal;
		currentSecret %= MODULO;

		// Step 3: Multiply by 2048, XOR with current, then prune
		stepVal = currentSecret * 2048;
		currentSecret ^= stepVal;
		currentSecret %= MODULO;

		return currentSecret;
	}

	// Generates the entire list of secret numbers (initial + 2000 new)
	// and returns the list of computed last-digit prices,
	// as well as the list of changes between consecutive prices.
	private static (List<int> prices, List<int> changes) GeneratePricesAndChanges(uint initialSecret)
	{
		// We'll store the prices for 2001 secret values:
		// the initial plus the next 2000.
		List<uint> secrets = new List<uint>(2001);
		// First secret is the initial
		secrets.Add(initialSecret);

		// Generate the remaining 2000 secrets
		for (int i = 0; i < 2000; i++)
		{
			uint next = GetNextSecret(secrets[secrets.Count - 1]);
			secrets.Add(next);
		}

		// Now compute prices (last digit of each secret number)
		List<int> prices = new List<int>(2001);
		foreach (uint s in secrets)
		{
			// last digit of s
			prices.Add((int)(s % 10));
		}

		// Compute changes between consecutive prices
		List<int> changes = new List<int>(2000);
		for (int i = 0; i < prices.Count - 1; i++)
		{
			changes.Add(prices[i + 1] - prices[i]);
		}

		return (prices, changes);
	}

	public static void Main()
	{
		// Read all initial secrets from stdin
		var initialSecrets = File.ReadAllLines(@"e:\source\advent2024\12-22\numbers.txt").Select(n => uint.Parse(n)).ToList();

		// For each buyer (each initial secret), generate their prices and changes
		// We'll store these in a list to avoid re-generating for each pattern
		List<(List<int> prices, List<int> changes)> buyersData
			= new List<(List<int> prices, List<int> changes)>(initialSecrets.Count);

		foreach (var secret in initialSecrets)
		{
			buyersData.Add(GeneratePricesAndChanges(secret));
		}

		// We want to find the single 4-change sequence that maximizes the total bananas.
		// Changes can range from -9 to +9 (since prices are 0–9, difference is in [-9, 9]).
		// We'll brute force all 19^4 possible 4-change patterns.
		long maxBananas = 0; // track the best sum we can achieve

		// For each possible pattern of 4 consecutive changes
		for (int c1 = -9; c1 <= 9; c1++)
		{
			for (int c2 = -9; c2 <= 9; c2++)
			{
				for (int c3 = -9; c3 <= 9; c3++)
				{
					for (int c4 = -9; c4 <= 9; c4++)
					{
						// We'll check each buyer's changes to see
						// if and where (first occurrence) this sequence appears.
						long totalForThisPattern = 0;

						foreach (var (prices, changes) in buyersData)
						{
							// We look for the earliest matching occurrence of [c1,c2,c3,c4]
							// in the changes list. If found at index i, the selling price
							// is prices[i+1+3], specifically prices[i+4], because i is
							// the index for the first change. The price we get is the
							// price at the time the last change is applied (the "current" price).

							// Explanation:
							// changes[i] is the difference between prices[i+1] and prices[i]
							// changes[i+1] is the difference between prices[i+2] and prices[i+1]
							// ...
							// changes[i+3] is the difference between prices[i+4] and prices[i+3]
							//
							// So if the sequence starts at changes[i], the sale happens right
							// after changes[i+3] is applied, so the sale price is prices[i+4].
							// We must ensure i+3 < changes.Count => i <= changes.Count - 4

							for (int i = 0; i <= changes.Count - 4; i++)
							{
								if (changes[i] == c1 &&
									changes[i + 1] == c2 &&
									changes[i + 2] == c3 &&
									changes[i + 3] == c4)
								{
									// We found the sequence. Add the sale price.
									// sale price = prices[i+4]
									totalForThisPattern += prices[i + 4];
									break; // only sell once per buyer
								}
							}
						}

						if (totalForThisPattern > maxBananas)
						{
							maxBananas = totalForThisPattern;
						}
					}
				}
			}
		}

		Console.WriteLine(maxBananas);
	}
}