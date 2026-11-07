using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays a two-frame lever animation using two independently positioned
/// Image layers. Both layers stay in the hierarchy; only rendering is toggled.
/// </summary>
public class LeverController : MonoBehaviour
{
    [SerializeField] private Image idleImage;
    [SerializeField] private Image pulledImage;

    private void Reset()
    {
        idleImage = GetComponent<Image>();
    }

    private void Awake()
    {
        if (idleImage == null || pulledImage == null)
            throw new System.InvalidOperationException("Both lever Image components must be assigned.");

        idleImage.preserveAspect = true;
        pulledImage.preserveAspect = true;
        SetFrame(false);
    }

    /// <summary>
    /// Shows frame two for the requested duration, then returns to frame one.
    /// </summary>
    public IEnumerator Pull(float duration)
    {
        if (idleImage == null || pulledImage == null)
            throw new System.InvalidOperationException("Both lever Image components must be assigned.");

        SetFrame(true);
        yield return new WaitForSecondsRealtime(Mathf.Max(0f, duration));
        SetFrame(false);
    }

    private void SetFrame(bool pulled)
    {
        idleImage.enabled = !pulled;
        pulledImage.enabled = pulled;
    }
}
