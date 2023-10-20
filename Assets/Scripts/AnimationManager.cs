using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

//Handle arbitrary animation by calling this singleton instance. 
public class AnimationManager : MonoBehaviour
{
    public SceneFader sceneFader;   // Set this to SceneFader Canvas, initially activated
    public static AnimationManager instance;
    public GameObject deathScreen;

    // Complete screen
    public GameObject completeScreen;
    public TextMeshProUGUI collectItems; // For the collects
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        sceneFader = GetComponent<SceneFader>();
        deathScreen.SetActive(false);
        completeScreen.SetActive(false);
        DontDestroyOnLoad(gameObject);


    }

    public IEnumerator ActivateDeathScreen()
    {
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));
        deathScreen.SetActive(true);
        yield return new WaitForSeconds(0);
    }

    public IEnumerator DeactivateDeathScreen()
    {
        deathScreen.SetActive(false);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSeconds(0);
    }

    public IEnumerator ActivateCompleteScreen()
    {
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));
        completeScreen.SetActive(true);
        collectItems.text = GameManager.instance.rewardsAmount.ToString() + " / " + GameManager.instance.rewardsTotal.ToString();
        yield return new WaitForSeconds(0);
    }

    public IEnumerator DeactivateCompleteScreen()
    {
        completeScreen.SetActive(false);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSeconds(0);
    }


}
