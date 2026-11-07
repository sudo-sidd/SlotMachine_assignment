using UnityEngine;

/// <summary>
/// Stores the configuration for one slot-machine symbol.
/// A SymbolData asset is created in the Project window and then assigned to the
/// machine. Keeping this data in a ScriptableObject separates editable game
/// values from the code that runs the spin.
/// </summary>
[CreateAssetMenu(fileName = "Symbol", menuName = "Slots/Symbol Data")]
public class SymbolData : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private Sprite sprite;
    [Tooltip("Higher weight means this symbol lands more often.")]
    [SerializeField, Min(1)] private int weight = 1;
    [Tooltip("Payout multiplier when all three reels show this symbol.")]
    [SerializeField, Min(0)] private int tripleMultiplier = 5;

    public string DisplayName => displayName;
    public Sprite Sprite => sprite;
    public int Weight => weight;
    public int TripleMultiplier => tripleMultiplier;
}
