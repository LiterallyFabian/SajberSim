using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LoadScript : MonoBehaviour, INovelAction
{
    public GameManager Game;
    public void Run(string[] line)
    {
        NovelDebugInfo status = Working(line);
        if (status.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "LOADSCRIPT|script"));
            Game.ToggleDevmenu(true);

            return;
        }
        Load(line[1]);
    }
    public NovelDebugInfo Working(string[] line)
    {
        if (line.Length != 2) return NovelDebugInfo.Error(string.Format(Translate.Get("invalidargumentlength"), line.Length, 2));
        if (!File.Exists($"{Helper.currentStoryPath}/Dialogues/{line[1]}.txt")) return NovelDebugInfo.Error($"The script \"{line[1]}\" does not exist. Expected path: {GameManager.shortStoryPath}/Dialogues/{line[1]}.txt");
        return NovelDebugInfo.OK();
    }
    public void Load(string storyID)
    {
        string path = $"{Helper.currentStoryPath}/Dialogues/{storyID}.txt";
        if (!File.Exists(path))
        {
            Helper.Alert(string.Format(Translate.Get("invalidstory"), path));
            Helper.Alert($"Visual Novel: Tried to start non-existing story: {path}");
            Debug.LogError($"Visual Novel: Tried to start non-existing story: {path}");
            return;
        }
        GameManager.scriptPath = path;
        GameManager.scriptName = storyID;
        Debug.Log($"New story loaded: {storyID}");
        GameManager.dialoguepos = 0;
        GameManager.story = File.ReadAllLines(path);
        GameManager.ready = true;
        Game.RunNext();
    }
}
