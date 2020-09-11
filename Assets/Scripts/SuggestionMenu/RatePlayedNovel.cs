using SajberSim.Helper;
using SajberSim.Translation;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class RatePlayedNovel : MonoBehaviour
{
    public Text Description;
    private void Start()
    {
        Description.text = string.Format(Translate.Get("rateplayednovel"), Helper.currentStoryName);
    }
    public void OpenCommentLink()
    {
        Process.Start($@"steam://openurl/https://steamcommunity.com/sharedfiles/filedetails/?id={Helper.currentStoryID}");
        Close();
    }
    public void Close()
    {
        Destroy(gameObject);
    }
}
