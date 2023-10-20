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
        // Fading in
        fadeUI.FadeUIIn(fadeTime);

        // Wait some time
        yield return new WaitForSeconds(fadeTime);

        // Setting player object and respawn it
        GameManager.instance.player.gameObject.SetActive(true);
        GameManager.instance.player.transform.position = GameManager.instance.respawnPoint;
        GameManager.instance.player.Respawned();

        // Finally, load the scene. 
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
