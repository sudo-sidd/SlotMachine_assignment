using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Owns the betting popup and exposes a single bet-selection event to game logic.
/// Popup animation is implemented locally so the project does not require DOTween.
/// </summary>
public class UIController : MonoBehaviour
{
    [Header("Balance")]
    [SerializeField] private TMP_Text balanceText;

    [Header("Bet popup")]
    [SerializeField] private GameObject bettingPopup;
    [SerializeField] private CanvasGroup bettingPopupCanvasGroup;
    [SerializeField] private Button[] betButtons;
    [SerializeField] private TMP_Text[] betButtonLabels;
    [SerializeField] private RectTransform bettingPopupTransform;
    [SerializeField] private float popupAnimationDuration = 0.2f;
    [SerializeField] private float popupStartScale = 0.9f;

    /// <summary>Raised with the selected bet button index.</summary>
    public event Action<int> BetSelected;

    private Coroutine popupAnimation;
    private Vector3 popupVisibleScale = Vector3.one;

    private void Awake()
    {
        if (bettingPopupTransform != null)
            popupVisibleScale = bettingPopupTransform.localScale;

        for (int i = 0; i < betButtons.Length; i++)
        {
            int buttonIndex = i;
            betButtons[i].onClick.AddListener(() => SelectBet(buttonIndex));
        }

        HideBetPopupImmediate();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < betButtons.Length; i++)
        {
            int buttonIndex = i;
            betButtons[i].onClick.RemoveAllListeners();
        }
    }

    public void UpdateBalance(int balance)
    {
        balanceText.text = balance.ToString();
    }

    /// <summary>
    /// Updates button labels and disables bets the player cannot afford.
    /// </summary>
    public void ConfigureBetButtons(int[] bets, int balance)
    {
        if (bets.Length != betButtons.Length || bets.Length != betButtonLabels.Length)
            throw new InvalidOperationException("Bet button and label counts must match the bet options.");

        for (int i = 0; i < bets.Length; i++)
        {
            betButtonLabels[i].text = bets[i].ToString();
            betButtons[i].interactable = balance >= bets[i];
        }
    }

    public void ShowBetPopup(int[] bets, int balance)
    {
        ConfigureBetButtons(bets, balance);
        bettingPopup.SetActive(true);

        if (popupAnimation != null)
            StopCoroutine(popupAnimation);
        popupAnimation = StartCoroutine(AnimatePopup(true));
    }

    public void HideBetPopup()
    {
        if (popupAnimation != null)
            StopCoroutine(popupAnimation);
        popupAnimation = StartCoroutine(AnimatePopup(false));
    }

    private void SelectBet(int buttonIndex)
    {
        if (buttonIndex < 0 || buttonIndex >= betButtons.Length || !betButtons[buttonIndex].interactable)
            return;

        BetSelected?.Invoke(buttonIndex);
    }

    private System.Collections.IEnumerator AnimatePopup(bool show)
    {
        if (bettingPopupCanvasGroup == null || bettingPopupTransform == null)
        {
            bettingPopup.SetActive(show);
            yield break;
        }

        float elapsed = 0f;
        float startAlpha = show ? 0f : 1f;
        float endAlpha = show ? 1f : 0f;
        Vector3 startScale = show ? popupVisibleScale * popupStartScale : popupVisibleScale;
        Vector3 endScale = show ? popupVisibleScale : popupVisibleScale * popupStartScale;

        bettingPopupCanvasGroup.alpha = startAlpha;
        bettingPopupTransform.localScale = startScale;

        while (elapsed < popupAnimationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / popupAnimationDuration);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            bettingPopupCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, eased);
            bettingPopupTransform.localScale = Vector3.LerpUnclamped(startScale, endScale, eased);
            yield return null;
        }

        bettingPopupCanvasGroup.alpha = endAlpha;
        bettingPopupTransform.localScale = endScale;
        bettingPopup.SetActive(show);
        popupAnimation = null;
    }

    private void HideBetPopupImmediate()
    {
        bettingPopup.SetActive(false);
        if (bettingPopupCanvasGroup != null)
            bettingPopupCanvasGroup.alpha = 0f;
        if (bettingPopupTransform != null)
            bettingPopupTransform.localScale = popupVisibleScale * popupStartScale;
    }
}
