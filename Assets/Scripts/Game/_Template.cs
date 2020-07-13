using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _Template : MonoBehaviour
{
    public GameManager Game;
    public void Run(string[] line)
    {
        if (Working(line) != "")
        {
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), Working(line)));
            return;
        }
    }
    public string Working(string[] line)
    {
        return "";
    }
}
