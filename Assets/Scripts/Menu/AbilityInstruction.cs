using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// They show abilities on a canvas, has the ability to scale up and down. 
/// </summary>
public class AbilityInstruction : MonoBehaviour
{
    // These layergroups correspond to what type of button combinations will be shown. (code smell warning)
    [SerializeField] private HorizontalLayoutGroup gJump;
    [SerializeField] private HorizontalLayoutGroup gMove;
    [SerializeField] private HorizontalLayoutGroup gDash;
    [SerializeField] private HorizontalLayoutGroup gMelee;
    [SerializeField] private HorizontalLayoutGroup gBow;
    [SerializeField] private HorizontalLayoutGroup gBowUp;
    [SerializeField] private HorizontalLayoutGroup gBowDown;
    [SerializeField] private HorizontalLayoutGroup gDBL;
    private HorizontalLayoutGroup currentGroup;
    [SerializeField] private Image BGImage; // For actiuvate/ deactivate

    // For animation
    [SerializeField] private Vector3 maxScale = new Vector3(1.45f, 1.45f, 1); // How far the UI will scale
    [SerializeField] private float animationDuration = 0.5f;    // Duration for it will scale up and down. 

    // Start is called before the first frame update
    private void Start()
    {
        gJump.gameObject.SetActive(false);
        gMove.gameObject.SetActive(false);
        gDash.gameObject.SetActive(false);
        gDBL.gameObject.SetActive(false);
        gMelee.gameObject.SetActive(false);
        gBow.gameObject.SetActive(false);
        gBowDown.gameObject.SetActive(false);
        gBowUp.gameObject.SetActive(false);
        BGImage.gameObject.SetActive(false);

    }

    /// <summary>
    /// Make the instruction text visible using the given string arguemnts
    /// </summary>
    /// <param name="instructions"></param>
    public void ShowInstructions(string groupType)
    {

        // The image
        BGImage.gameObject.SetActive(true);

        // Activate correct horizontal group
        currentGroup = GetGroupType(groupType);
        if (currentGroup == null)
        {
            return;
        }
        currentGroup.gameObject.SetActive(true);

        // Set images
        for (int i = 0; i < currentGroup.transform.childCount; i++)
        {
            Transform child = currentGroup.transform.GetChild(i);
            child.gameObject.SetActive(true);
        }

        StartCoroutine(ScaleInstructions());

    }

    private HorizontalLayoutGroup GetGroupType(string config)
    {
        if (config == "DBL")
        {
            return gDBL;
        }
        else if (config == "JUMP")
        {
            return gJump;
        }
        else if (config == "MOVE")
        {
            return gMove;
        }
        else if (config == "MELEE")
        {
            return gMelee;
        }
        else if (config == "BOW")
        {
            return gBow;
        }
        else if (config == "BOW_UP")
        {
            return gBowUp;
        }
        else if (config == "BOW_DOWN")
        {
            return gBowDown;
        }
        else if (config == "DASH")
        {
            return gDash;
        }
        else
        {
            print("Problem at caller!");
            return null;
        }
    }

    /// <summary>
    /// Deactiuvate the gameObject and the buttons
    /// </summary>
    public void HideInstructions()
    {
        // Disable coroutine for scaling
        StopCoroutine(ScaleInstructions());

        // Buttons disable
        for (int i = 0; i < currentGroup.transform.childCount; i++)
        {
            Transform child = currentGroup.transform.GetChild(i);
            child.gameObject.SetActive(false);
        }
        currentGroup.gameObject.SetActive(false);

        // Disable image last
        BGImage.gameObject.SetActive(false);
    }

    public IEnumerator ScaleInstructions()
    {
        while (true) // Continue looping indefinitely until HideInstructions is called
        {
            // Scale up to max scale
            float elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                BGImage.gameObject.transform.localScale = Vector3.Lerp(Vector3.one, maxScale, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            BGImage.gameObject.transform.localScale = maxScale; // Ensure it's exactly maxScale

            // Pause briefly at max scale
            yield return new WaitForSeconds(0); // Adjust the pause duration as needed

            // Scale down to regular scale
            elapsedTime = 0f;
            while (elapsedTime < animationDuration)
            {
                BGImage.gameObject.transform.localScale = Vector3.Lerp(maxScale, Vector3.one, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            BGImage.gameObject.transform.localScale = Vector3.one; // Ensure it's exactly Vector3.one
        }
    }




}
