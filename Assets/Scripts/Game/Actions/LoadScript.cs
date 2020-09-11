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
    public void Run(string[] line)
    {
        NovelDebugInfo debugdata = Working(line);
        string status = debugdata.Message;
        if (debugdata.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "LOADSCRIPT|script"));
            GameManager.Instance.ToggleDevmenu(true);

            return;
        }
        Load(line[1]);
    }
    public NovelDebugInfo Working(string[] line)
    {
        NovelDebugInfo NDI = new NovelDebugInfo(line, GameManager.dialoguepos);
        if (line.Length != 2) return NDI.Done(string.Format(Translate.Get("invalidargumentlength"), line.Length, 2));
        if (!File.Exists(Path.Combine(Helper.currentStoryPath, "Dialogues", line[1] + ".txt"))) return NDI.Done($"The script \"{line[1]}\" does not exist. Expected path: {Path.Combine(GameManager.shortStoryPath, "Dialogues", line[1] + ".txt")}");


        return NDI;
    }
    public void Load(string storyID)
    {
        string path = Path.Combine(Helper.currentStoryPath, "Dialogues", storyID + ".txt");
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
        GameManager.Instance.RunNext();
    }
}