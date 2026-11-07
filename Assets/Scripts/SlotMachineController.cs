using System.Collections;
using UnityEngine;

/// <summary>
/// Coordinates the complete play loop: bet selection, lever pull, reel animation,
/// payout, and opening the next betting popup.
/// </summary>
public class SlotMachineController : MonoBehaviour
{
    [Header("Game")]
    [SerializeField] private SymbolData[] symbols;
    [SerializeField] private int startingBalance = 1000;
    [SerializeField] private int[] betOptions = { 10, 25, 50 };

    [Header("Timing")]
    [SerializeField] private float firstReelStopDelay = 0.8f;
    [SerializeField] private float reelStopStagger = 0.45f;
    [SerializeField] private float leverPullDuration = 0.12f;

    [Header("References")]
    [SerializeField] private ReelController[] reels;
    [SerializeField] private UIController ui;
    [SerializeField] private LeverController lever;

    private SpinResolver resolver;
    private int balance;
    private bool isSpinning;

    private void Awake()
    {
        resolver = new SpinResolver(symbols);
        balance = startingBalance;

        foreach (ReelController reel in reels)
            reel.Initialize(symbols);
    }

    private void OnEnable()
    {
        ui.BetSelected += HandleBetSelected;
    }

    private void OnDisable()
    {
        ui.BetSelected -= HandleBetSelected;
    }

    private void Start()
    {
        RefreshUI();
        OpenBetPopup();
    }

    private void HandleBetSelected(int betIndex)
    {
        if (isSpinning || betIndex < 0 || betIndex >= betOptions.Length)
            return;

        int bet = betOptions[betIndex];
        if (balance < bet)
            return;

        StartCoroutine(SpinRoutine(bet));
    }

    private IEnumerator SpinRoutine(int bet)
    {
        isSpinning = true;
        balance -= bet;
        ui.HideBetPopup();
        RefreshUI();

        SpinResult result = resolver.Resolve(bet);

        if (lever != null)
            yield return lever.Pull(leverPullDuration);

        var running = new Coroutine[reels.Length];
        for (int i = 0; i < reels.Length; i++)
        {
            float delay = firstReelStopDelay + i * reelStopStagger;
            running[i] = StartCoroutine(reels[i].SpinAndStop(result.Symbols[i], delay));
        }

        foreach (Coroutine reel in running)
            yield return reel;

        if (result.IsWin)
            balance += result.Payout;

        isSpinning = false;
        RefreshUI();
        OpenBetPopup();
    }

    private void OpenBetPopup()
    {
        if (!isSpinning)
            ui.ShowBetPopup(betOptions, balance);
    }

    private void RefreshUI()
    {
        ui.UpdateBalance(balance);
    }
}
