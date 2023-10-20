using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the HUD meny which includes health, crones and key collection. 
/// </summary>
public class HUD : MonoBehaviour
{

    [Header("Health system")]
    public Image[] harts;
    public Sprite fullHart;
    public Sprite halfFullHart;
    public Sprite emptyHart;
    [SerializeField] private Vector3 maxScale = new Vector3(1.45f, 1.45f, 1); // How far the UI will scale

    private void Awake()
    {
        gameObject.SetActive(true);     // Set this to false, when moving away from the satrat_scene
    }

    private void Start()
    {
        UpdatePlayerCallback();
        UpdateHartsHUD();
    }

    public void UpdatePlayerCallback()
    {
        GameManager.instance.player.onHealthChangedCallback += UpdateHartsHUD;  // Change
    }


    /// <summary>
    /// Initializing all Images depending on player maxhealth
    /// </summary>
    public void InitHarts()
    {
        //Start by disabling all current health
        foreach (Image hart in harts)
        {
            hart.gameObject.SetActive(false);
        }

        //Set the amount of harts from the player health. 
        for (int i = 0; i < GameManager.instance.player.maxHealth / 2; i++)
        {
            harts[i].gameObject.SetActive(true);
            harts[i].sprite = fullHart;
        }
        //Eg. Maxhealth = 4, need 2 hearts. 
        //max = 6, need 3 hearts because 1 health == half heart
    }

    /// <summary>
    /// Fill the initialized hearts depending on player current health. 
    /// </summary>
    public void UpdateHarts()
    {
        float tempHealth = GameManager.instance.player.health;
        for (int i = 0; i < GameManager.instance.player.maxHealth / 2; i++)
        {

            if (tempHealth >= 2)
            {
                harts[i].sprite = fullHart;     //Full hart
                tempHealth -= 2;
            }
            else if (tempHealth >= 1)
            {
                harts[i].sprite = halfFullHart;    //Empty hart
                tempHealth -= 1;
            }
            else
            {
                harts[i].sprite = emptyHart; //Half hart

            }

        }

    }

    public void UpdateHartsHUD()
    {
        // Call this when player collects more health
        InitHarts();
        UpdateHarts();
    }


    /// <summary>
    /// Scale all hearts
    /// </summary>
    public void ScaleHearts(float animationDuration)
    {
        StartCoroutine(ScaleHeartsRoutine(animationDuration));
    }

    public IEnumerator ScaleHeartsRoutine(float animationDuration)
    {
        foreach (Image hart in harts)
        {
            StartCoroutine(ScaleHeart(hart.gameObject, animationDuration));
            yield return new WaitForSeconds(animationDuration);
        }
    }



    /// <summary>
    /// Scale the heart up and down for each heart once. Call this function when calling increase maxharts. 
    /// </summary>
    public IEnumerator ScaleHeart(GameObject hart, float animationDuration)
    {
        // Scale up to max scale
        float elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            hart.transform.localScale = Vector3.Lerp(Vector3.one, maxScale, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hart.transform.localScale = maxScale; // Ensure it's exactly maxScale

        // Pause briefly at max scale
        yield return new WaitForSeconds(0); // Adjust the pause duration as needed

        // Scale down to regular scale
        elapsedTime = 0f;
        while (elapsedTime < animationDuration)
        {
            hart.transform.localScale = Vector3.Lerp(maxScale, Vector3.one, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hart.transform.localScale = Vector3.one; // Ensure it's exactly Vector3.one

    }
}
