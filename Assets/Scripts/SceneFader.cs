using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SceneFader : MonoBehaviour
{

    [SerializeField] private float fadeTime;    // How long the fade shall last
    [SerializeField] private Image fadeOutImage;
    public enum FadeDirection
    {
        In,
        Out,
    }

    private void Start()
    {
        //fadeOutImage.setActive(false);
    }


    /// <summary>
    /// Either decreases or increases the alpha value depending on the fadedirection. 
    /// </summary>
    /// <param name="alpha"></param>
    /// <param name="fadeDirection"></param>
    public void SetColorImage(ref float alpha, FadeDirection fadeDirection)
    {
        fadeOutImage.color = new Color(fadeOutImage.color.r, fadeOutImage.color.g, fadeOutImage.color.b, alpha);
        alpha += Time.deltaTime * (1 / fadeTime) * (fadeDirection == FadeDirection.Out ? -1 : 1);

    }

    public IEnumerator Fade(FadeDirection fadeDirection)
    {
        float alpha = fadeDirection == FadeDirection.Out ? 1 : 0;
        float fadeEndValue = fadeDirection == FadeDirection.Out ? 0 : 1;
        if (fadeDirection == FadeDirection.Out)
        {
            while (alpha >= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }

            fadeOutImage.enabled = false;
        }
        else
        {
            fadeOutImage.enabled = true;
            while (alpha <= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
        }

    }

    public IEnumerator FadeAndLoadScene(FadeDirection fadeDirection, string levelToLoad)
    {
        fadeOutImage.enabled = true;
        yield return Fade(fadeDirection);
        SceneManager.LoadScene(levelToLoad);
    }

    public void FadeAndLoadSceneCoroutine(string levelToLoad)
    {
        StartCoroutine(FadeAndLoadScene(FadeDirection.In, levelToLoad));
    }
}
