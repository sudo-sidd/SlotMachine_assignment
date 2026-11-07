/// <summary>
/// Immutable description of one completed random spin decision.
/// This object contains the result before the reels animate, so visual timing
/// cannot affect the outcome.
/// </summary>
public class SpinResult
{
    public SymbolData[] Symbols { get; }
    public bool IsWin { get; }
    public int Payout { get; }

    public SpinResult(SymbolData[] symbols, bool isWin, int payout)
    {
        Symbols = symbols;
        IsWin = isWin;
        Payout = payout;
    }
}
