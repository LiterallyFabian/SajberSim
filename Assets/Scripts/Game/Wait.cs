using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wait : MonoBehaviour, GameManager.INovelAction
{
    public GameManager Game;
    public void Run(string[] line)
    {
        string status = Working(line);
        if (status != "")
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "WAIT|seconds"));
            Game.RunNext();
            return;
        }
        StartCoroutine(Delay((float)Convert.ToDouble(line[1], Language.Format)));

    }
    public string Working(string[] line)
    {
        if (line.Length != 2) return $"The length of the line is {line.Length}, while the expected is 2.";
        if(!Helper.IsFloat(line[1])) return $"The time <b>{line[3]}</b> is not a valid float (eg 7.5 or 2)";
        return "";
    }
    private IEnumerator Delay(float time) //ID 7
    {
        GameManager.ready = false;
        yield return new WaitForSeconds(time);
        GameManager.ready = true;
        Game.RunNext();
    }
}
