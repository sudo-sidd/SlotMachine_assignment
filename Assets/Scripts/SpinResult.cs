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
