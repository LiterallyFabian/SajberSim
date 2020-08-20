using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayAudio : MonoBehaviour, INovelAction
{
    public GameManager Game;
    public void Run(string[] line)
    {
        NovelDebugInfo debugdata = Working(line);
        string status = debugdata.Message;
        if (debugdata.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, $"{line[0].ToUpper()}|audio"));
            Game.RunNext();
            return;
        }
        Play(line);
        Game.RunNext();
    }
    public NovelDebugInfo Working(string[] line)
    {
        NovelDebugInfo NDI = new NovelDebugInfo(line, GameManager.dialoguepos);

        string path = $"{Helper.currentStoryPath}/Audio/{line[1]}.ogg";
        if (line.Length != 2) return NDI.Done(string.Format(Translate.Get("invalidargumentlength"), line.Length, 2));
        if (!File.Exists(path)) return NDI.Done(string.Format(Translate.Get("missingaudio"), line[1], $"{GameManager.shortStoryPath}/Audio/{line[1]}.ogg"));

        return NDI;
    }
    private void Play(string[] line)
    {
        if (line[0] == "PLAYSFX")
            Game.dl.Ogg(Game.SFX, $"file://{Helper.currentStoryPath}/Audio/{line[1]}.ogg", true);
        else if(GameManager.currentMusic != line[1])
            Game.dl.Ogg(Game.music, $"file://{Helper.currentStoryPath}/Audio/{line[1]}.ogg", true);
        GameManager.currentMusic = line[1];
    }
}
