using System.Collections;
using System;
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
    private Coroutine[] runningReels;
    private int balance;
    private bool isSpinning;

    private void Awake()
    {
        if (symbols == null || symbols.Length == 0)
            throw new InvalidOperationException("At least one symbol must be assigned.");
        if (reels == null || reels.Length != 3)
            throw new InvalidOperationException("Exactly three reels must be assigned.");
        if (ui == null)
            throw new InvalidOperationException("A UIController must be assigned.");
        if (startingBalance < 0)
            throw new InvalidOperationException("Starting balance cannot be negative.");
        if (betOptions == null || betOptions.Length == 0)
            throw new InvalidOperationException("At least one bet option must be configured.");

        foreach (int bet in betOptions)
        {
            if (bet <= 0)
                throw new InvalidOperationException("Bet options must be positive.");
        }

        resolver = new SpinResolver(symbols);
        balance = startingBalance;
        runningReels = new Coroutine[reels.Length];

        foreach (ReelController reel in reels)
        {
            if (reel == null)
                throw new InvalidOperationException("All reel references must be assigned.");
            reel.Initialize(symbols);
        }
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
        {
            yield return lever.Pull(leverPullDuration);
        }

        for (int i = 0; i < reels.Length; i++)
        {
            float delay = firstReelStopDelay + i * reelStopStagger;
            runningReels[i] = StartCoroutine(reels[i].SpinAndStop(result.Symbols[i], delay));
        }

        foreach (Coroutine reel in runningReels)
        {
            yield return reel;
        }

        if (result.IsWin)
        {
            balance += result.Payout;
        }

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
