using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _Template : MonoBehaviour, INovelAction
{
    public GameManager Game;
    public void Run(string[] line)
    {
        NovelDebugInfo debugdata = Working(line);
        string status = debugdata.Message;
        if (debugdata.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "syntax"));
            return;
        }
    }
    public NovelDebugInfo Working(string[] line)
    {
        NovelDebugInfo NDI = new NovelDebugInfo(line, GameManager.dialoguepos);

        //Start debugging:

        //Done
        if (NDI.Message != "OK") NDI.Code = NovelDebugInfo.Status.Error;
        return NDI;
    }
}
