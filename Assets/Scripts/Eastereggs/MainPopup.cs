using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MainPopup : MonoBehaviour
{
    //General
    private Download dl;

    //For making the main character blushing
    public static bool singlecharClicked;

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
        if (!singlecharClicked)
        {
            if (File.Exists(ButtonCtrl.charpath.Replace("happy", "blush")))
                dl.Image(GameObject.Find("Character"), $"file://{ButtonCtrl.charpath.Replace("happy", "blush")}");
            singlecharClicked = true;
        }
        else
        {
            //GameObject.Find("/Canvas/Character").GetComponent<Animator>().Play("characterbye");
            //GameObject.Find("/CharEasterEgg").GetComponent<Animator>().Play("allchar popup");
        }
    }
}
