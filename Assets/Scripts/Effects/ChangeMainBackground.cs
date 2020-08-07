using SajberSim.CardMenu;
using SajberSim.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class ChangeMainBackground : MonoBehaviour
{
    public Texture defaultback;
    public void UpdateBG()
    {
        Download dl = FindObjectsOfType<Download>()[0];
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
}
