using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wait : MonoBehaviour, INovelAction
{
    public GameManager Game;
    public void Run(string[] line)
    {
        NovelDebugInfo status = Working(line);
        if (status.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, $"WAIT|{Translate.Get("seconds")}"));
            Game.RunNext();
            return;
        }
        StartCoroutine(Delay((float)Convert.ToDouble(line[1], Language.Format)));

    }
    public NovelDebugInfo Working(string[] line)
    {
        NovelDebugInfo NDI = new NovelDebugInfo(line, GameManager.dialoguepos);

        if (line.Length != 2) NDI.Message = string.Format(Translate.Get("invalidargumentlength"), line.Length, 2); //Incorrect length, found LENGTH arguments but the action expects 2.
        if (!Helper.IsFloat(line[1])) NDI.Message = string.Format(Translate.Get("invalidfloat"), Translate.Get("arg_time"), line[1]); //The time <b>TIME</b> is not a valid float (eg 7.5 or 2).

        //Done
        if (NDI.Message != "OK") NDI.Code = NovelDebugInfo.Status.Error;
        return NDI;
    }
    private IEnumerator Delay(float time) //ID 7
    {
        GameManager.ready = false;
        yield return new WaitForSeconds(time);
        GameManager.ready = true;
        Game.RunNext();
    }
}
