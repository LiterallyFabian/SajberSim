using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopAudio : MonoBehaviour, INovelAction
{
    public void Run(string[] line)
    {
        NovelDebugInfo debugdata = Working(line);
        string status = debugdata.Message;
        if (debugdata.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "STOPSOUNDS"));
            return;
        }
        StopSound(line[0].ToLower().Replace("STOP", ""));
        GameManager.Instance.RunNext();
    }
    public NovelDebugInfo Working(string[] line)
    {
        return NovelDebugInfo.OK();
    }
    private void StopSound(string source)
    {
        if (source == "MUSIC")
        {
            GameManager.Instance.music.GetComponent<AudioSource>().Stop();
            GameManager.currentMusic = "";
        }

        else if (source == "SFX")
            GameManager.Instance.SFX.GetComponent<AudioSource>().Stop();

        else if (source == "SOUNDS")
        {
            GameManager.Instance.music.GetComponent<AudioSource>().Stop();
            GameManager.Instance.SFX.GetComponent<AudioSource>().Stop();
            GameManager.currentMusic = "";
        }
    }
}
