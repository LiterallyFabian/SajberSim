using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SajberSim.Web;

/// <summary>
/// Easter eggs with source code. How convinient? 
/// </summary>
public class ChangeMoodOnPopups : MonoBehaviour
{
    
    
    //For making the individual characters popping up chaning their mood
    public Sprite blush;
    public Sprite angry;
    private int clicks = 0;

    public void UpdateMood()
    {
        Debug.Log("CLICK! " + gameObject.name);
        clicks++;
        if (clicks == 1) GetComponent<Image>().sprite = blush;
        if(clicks == 5) GetComponent<Image>().sprite = angry;
    }
}
