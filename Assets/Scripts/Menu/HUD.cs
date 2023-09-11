using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Represents the HUD meny which includes health, crones and key collection. 
public class HUD : MonoBehaviour
{

    [Header("Health system")]
    public Image[] harts;
    public Sprite fullHart;
    public Sprite halfFullHart;
    public Sprite emptyHart;

    private void Awake()
    {
        gameObject.SetActive(true);     // Set this to false, when moving away from the satrat_scene
    }

    private void Start()
    {
        GameManager.instance.player.onHealthChangedCallback += UpdateHartsHUD;
        UpdateHartsHUD();
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

}
