using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Handle arbitrary animation by calling this singleton instance. 
public class AnimationManager : MonoBehaviour
{
    public SceneFader sceneFader;
    public static AnimationManager instance;
    public GameObject deathScreen;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        sceneFader = GetComponentInChildren<SceneFader>();
        deathScreen.SetActive(false);
    }

    public IEnumerator activateDeathScreen()
    {
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));
        deathScreen.SetActive(true);
        yield return new WaitForSeconds(0);
    }

    public IEnumerator deactivateDeathScreen()
    {
        deathScreen.SetActive(false);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSeconds(0);
    }


}
