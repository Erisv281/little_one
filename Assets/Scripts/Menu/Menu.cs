using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private FadeUI fadeUI;
    [SerializeField] private float fadeTime;

    public void Start()
    {
        fadeUI = GetComponent<FadeUI>();
        fadeUI.FadeUIOut(fadeTime);
    }

    public IEnumerator FadeAndStartGame(string sceneToLoad)
    {
        fadeUI.FadeUIIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        GameManager.instance.player.transform.position = GameManager.instance.respawnPoint;
        GameManager.instance.player.Respawned();
        SceneManager.LoadScene(sceneToLoad);
    }

    public void startGame(string sceneToLoad)
    {
        Input.ResetInputAxes();
        StartCoroutine(FadeAndStartGame(sceneToLoad));
    }

    public void quitToDesktop()
    {
        Input.ResetInputAxes();
        Debug.Log("Quit game");
        Application.Quit();
    }


}
