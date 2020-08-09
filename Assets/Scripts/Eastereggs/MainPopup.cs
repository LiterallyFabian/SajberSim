using SajberSim.Steam;
using SajberSim.Web;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainPopup : MonoBehaviour
{
    //General
    private Download dl;

    //For making the main character blushing
    public static int singlecharClicked;

    // Start is called before the first frame update
    private void Start()
    {
        dl = Download.Find();
    }

    public void Blush()
    {
        singlecharClicked++;
        if (singlecharClicked == 5)
        {
            if (File.Exists(ChangeMainMenuAssets.charpath.Replace("happy", "blush")))
            {
                dl.Image(GameObject.Find("Character"), $"file://{ChangeMainMenuAssets.charpath.Replace("happy", "blush")}");
                Achievements.Grant(Achievements.List.ACHIEVEMENT_findblush);
            }
        }
        else
        {
            //GameObject.Find("/Canvas/Character").GetComponent<Animator>().Play("characterbye");
            //GameObject.Find("/CharEasterEgg").GetComponent<Animator>().Play("allchar popup");
        }
    }
}
