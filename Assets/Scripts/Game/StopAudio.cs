using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopAudio : MonoBehaviour, GameManager.INovelAction
{
    public GameManager Game;
    public void Run(string[] line)
    {
        string status = Working(line);
        if (status != "")
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "STOPSOUNDS"));
            return;
        }
        StopSound(line[0].ToLower().Replace("STOP", ""));
        Game.RunNext();
    }
    public string Working(string[] line)
    {
        return "";
    }
    private void StopSound(string source)
    {
        if (source == "MUSIC")
        {
            Game.music.GetComponent<AudioSource>().Stop();
            GameManager.currentMusic = null;
        }

        else if (source == "SFX")
            Game.SFX.GetComponent<AudioSource>().Stop();

        else if (source == "SOUNDS")
        {
            Game.music.GetComponent<AudioSource>().Stop();
            Game.SFX.GetComponent<AudioSource>().Stop();
            GameManager.currentMusic = null;
        }
    }
}
