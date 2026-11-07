using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Changes the lever sprite for the short pull animation without changing the
/// lever RectTransform. Both sprites are rendered inside the same fixed frame.
/// </summary>
[RequireComponent(typeof(Image))]
public class LeverController : MonoBehaviour
{
    [SerializeField] private Image leverImage;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite pulledSprite;

    private void Reset()
    {
        leverImage = GetComponent<Image>();
    }

    private void Awake()
    {
        if (leverImage == null)
            leverImage = GetComponent<Image>();

        leverImage.preserveAspect = true;
        leverImage.sprite = idleSprite;
    }

    /// <summary>
    /// Shows the pulled sprite briefly, then restores the idle sprite.
    /// Sprite swapping never calls SetNativeSize, so the RectTransform stays fixed.
    /// </summary>
    public IEnumerator Pull(float duration)
    {
        if (idleSprite == null || pulledSprite == null)
            throw new System.InvalidOperationException("Both lever sprites must be assigned.");

        leverImage.sprite = pulledSprite;
        yield return new WaitForSeconds(duration);
        leverImage.sprite = idleSprite;
    }
}
