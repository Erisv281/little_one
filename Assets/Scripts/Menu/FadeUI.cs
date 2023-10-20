using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeUI : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Fade out the UI. Starts dark and make it brighter
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    IEnumerator FadeOut(float seconds)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 1;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / seconds;
            yield return null;
        }
        yield return null;
    }


    /// <summary>
    /// Fades in the UI, starts light and make it darker
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns></returns>
    IEnumerator FadeIn(float seconds)
    {
        canvasGroup.alpha = 0;
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / seconds;
            yield return null;
        }
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        yield return null;

    }

    public void FadeUIOut(float seconds)
    {
        StartCoroutine(FadeOut(seconds));

    }

    public void FadeUIIn(float seconds)
    {
        StartCoroutine(FadeIn(seconds));

    }
}
