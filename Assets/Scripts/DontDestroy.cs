using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Add this script to objects that shall not be destroyed when switching scenes. 
public class DontDestroy : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
