using System;
using System.Collections.Generic;

/// <summary>
/// Pure gameplay logic for weighted symbol selection and triple-symbol payouts.
/// It has no dependency on Unity scene objects or animation timing.
/// </summary>
public class SpinResolver
{
    private const int ReelCount = 3;
    private readonly IReadOnlyList<SymbolData> symbols;
    private readonly Random rng;
    private readonly int totalWeight;

    /// <summary>
    /// Creates a resolver from the configured symbol weights. A seed can be
    /// supplied by tests to make a sequence reproducible.
    /// </summary>
    public SpinResolver(IReadOnlyList<SymbolData> symbols, int? seed = null)
    {
        if (symbols == null || symbols.Count == 0)
            throw new ArgumentException("At least one symbol is required.", nameof(symbols));

        this.symbols = symbols;
        rng = seed.HasValue ? new Random(seed.Value) : new Random();

        long weightTotal = 0;
        foreach (SymbolData symbol in symbols)
        {
            if (symbol == null || symbol.Weight <= 0)
                throw new ArgumentException("Symbols must be non-null and have a positive weight.", nameof(symbols));

            weightTotal += symbol.Weight;
        }

        if (weightTotal > int.MaxValue)
            throw new ArgumentException("The total symbol weight must not exceed Int32.MaxValue.", nameof(symbols));

        totalWeight = (int)weightTotal;
    }

    /// <summary>Resolves all three reels before any visual animation begins.</summary>
    public SpinResult Resolve(int bet)
    {
        if (bet <= 0)
            throw new ArgumentOutOfRangeException(nameof(bet), "Bet must be positive.");

        var landed = new SymbolData[ReelCount];
        for (int i = 0; i < ReelCount; i++)
            landed[i] = PickWeightedSymbol();

        bool isWin = landed[0] == landed[1] && landed[1] == landed[2];
        int payout = isWin ? checked(bet * landed[0].TripleMultiplier) : 0;
        return new SpinResult(landed, isWin, payout);
    }

    /// <summary>Performs one weighted random pick over the configured symbols.</summary>
    private SymbolData PickWeightedSymbol()
    {
        int roll = rng.Next(totalWeight);
        int cumulative = 0;

        foreach (SymbolData symbol in symbols)
        {
            cumulative += symbol.Weight;
            if (roll < cumulative)
                return symbol;
        }

        return symbols[symbols.Count - 1];
    }
}
