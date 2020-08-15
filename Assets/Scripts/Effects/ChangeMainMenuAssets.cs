using SajberSim.CardMenu;
using SajberSim.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class ChangeMainMenuAssets : MonoBehaviour
{
    public Texture defaultback;
    public static string charpath;
    public void UpdateBG()
    {
        Download dl = Download.Init();
        UpdateCharacter();
        string[] backgroundList = Stories.GetAllStoryAssetPaths("main"); //mainbg*.png
        int id = UnityEngine.Random.Range(0, backgroundList.Length + 1);
        if (id != 0)
        {
            string bgpath = backgroundList[id - 1];
            Debug.Log($"ChangeMainBackground: Trying to set main background to {bgpath}");
            dl.RawImage(gameObject, bgpath);
        }
        else
        {
            GetComponent<RawImage>().texture = defaultback;
        }
    }
    public void UpdateCharacter()
    {
        Download dl = Download.Init();
        List<string> charpaths = new List<string>();
        foreach (string path in Stories.GetAllStoryAssetPaths("characters"))
        {
            if (path.Contains("happy")) charpaths.Add(path);
        }
        if (charpaths.Count() == 0) return;
        charpath = charpaths[UnityEngine.Random.Range(0, charpaths.Count)];
        int tries = 0;
        //prioritize a character with a blush mood
        while (!File.Exists(charpath.Replace("happy", "blush")) && tries < 3)
        {
            charpath = charpaths[UnityEngine.Random.Range(0, charpaths.Count)];
            tries++;
        }

        //ladda in filen som texture
        dl.Image(GameObject.Find("Character"), $"file://{charpath}");
    }
}
