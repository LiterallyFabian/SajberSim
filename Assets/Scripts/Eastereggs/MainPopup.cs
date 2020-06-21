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
        if (GameObject.Find("Helper"))
        {
            dl = GameObject.Find("Helper").GetComponent<Download>();
        }
        else
        {
            dl = new GameObject("downloadobj").AddComponent<Download>();
        }
    }

    public void Blush()
    {
        singlecharClicked++;
        if (singlecharClicked == 5)
        {
            if (File.Exists(ButtonCtrl.charpath.Replace("happy", "blush")))
            {
                dl.Image(GameObject.Find("Character"), $"file://{ButtonCtrl.charpath.Replace("happy", "blush")}");
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
