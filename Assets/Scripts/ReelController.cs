using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Animates one masked reel. Three fixed Image slots continuously scroll downward;
/// wrapped slots are reused, which avoids creating objects during a spin.
/// </summary>
public class ReelController : MonoBehaviour
{
    [Tooltip("Three Images beneath this reel. Add a RectMask2D sized to one cell.")]
    [SerializeField] private Image[] slots;
    [SerializeField] private float cellHeight = 120f;
    [SerializeField] private float spinSpeed = 1800f;
    [SerializeField] private float landDuration = 0.35f;

    private SymbolData[] filler;

    /// <summary>
    /// Assigns the filler symbols and places the three slot Images at one-cell
    /// intervals before the first spin.
    /// </summary>
    public void Initialize(SymbolData[] symbols)
    {
        if (symbols == null || symbols.Length == 0)
            throw new System.ArgumentException("At least one filler symbol is required.", nameof(symbols));
        if (slots == null || slots.Length != 3)
            throw new System.InvalidOperationException("Each reel must have exactly three slot Images.");
        if (cellHeight <= 0f || landDuration <= 0f || spinSpeed <= 0f)
            throw new System.InvalidOperationException("Reel timing and cell height must be positive.");

        filler = symbols;
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].sprite = RandomSprite();
            slots[i].rectTransform.anchoredPosition = new Vector2(0f, (1 - i) * cellHeight);
        }
    }

    /// <summary>
    /// Free-spins for the requested delay, injects the resolved target on a wrap,
    /// then eases the entire strip until that target is centered.
    /// </summary>
    public IEnumerator SpinAndStop(SymbolData target, float stopDelay)
    {
        if (target == null)
            throw new System.ArgumentNullException(nameof(target));

        for (float t = 0f; t < stopDelay; t += Time.unscaledDeltaTime)
        {
            Scroll(spinSpeed * Time.unscaledDeltaTime, null);
            yield return null;
        }

        Image targetSlot = null;
        while (targetSlot == null)
        {
            targetSlot = Scroll(spinSpeed * Time.unscaledDeltaTime, target);
            yield return null;
        }

        float distance = targetSlot.rectTransform.anchoredPosition.y;
        float moved = 0f;
        for (float t = 0f; t < landDuration; t += Time.unscaledDeltaTime)
        {
            float eased = 1f - Mathf.Pow(1f - t / landDuration, 3f);
            float step = distance * eased - moved;
            MoveSlots(step);
            moved += step;
            yield return null;
        }

        MoveSlots(distance - moved);
    }

    private Image Scroll(float distance, SymbolData forced)
    {
        Image placed = null;

        float wrapThreshold = -1.5f * cellHeight;

        foreach (Image slot in slots)
        {
            RectTransform rt = slot.rectTransform;
            Vector2 pos = rt.anchoredPosition;

            pos.y -= distance;

            if (pos.y <= wrapThreshold)
            {
                pos.y += slots.Length * cellHeight;
                slot.sprite = forced != null ? forced.Sprite : RandomSprite();
                if (forced != null)
                    placed = slot;
            }

            rt.anchoredPosition = pos;
        }

        return placed;
    }

    private void MoveSlots(float distance)
    {
        foreach (Image slot in slots)
        {
            Vector2 pos = slot.rectTransform.anchoredPosition;
            pos.y -= distance;
            slot.rectTransform.anchoredPosition = pos;
        }
    }

    private Sprite RandomSprite() => filler[Random.Range(0, filler.Length)].Sprite;
}
