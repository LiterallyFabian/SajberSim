using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Mood : MonoBehaviour, INovelAction
{
    public void Run(string[] line)
    {
        NovelDebugInfo debugdata = Working(line);
        string status = debugdata.Message;
        if (debugdata.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "MOOD|character|mood"));
            GameManager.Instance.RunNext();
            return;
        }
        string name = "";
        if (int.TryParse(line[1], out int xd)) name = GameManager.people[int.Parse(line[1])].name; //ID if possible, else name
        else name = line[1];
        SetMood(name.ToLower(), line[2]);
    }
    public NovelDebugInfo Working(string[] line)
    {
        NovelDebugInfo NDI = new NovelDebugInfo(line, GameManager.dialoguepos);

        //Start debugging:
        if (line.Length != 3) return NDI.Done(string.Format(Translate.Get("invalidargumentlength"), line.Length, "3"));
        string name = "";

        if (Helper.IsNum(line[1])) name = GameManager.people[int.Parse(line[1])].name; //ID if possible, else name
        else name = line[1];//$"{Helper.currentStoryPath}/Characters/{name}{line[2]}.png"
        if (!File.Exists(Path.Combine(Helper.currentStoryPath, "Characters", name + line[2] + ".png")) && !File.Exists(Path.Combine(Helper.currentStoryPath, "Characters", name, line[2] + ".png"))) return NDI.Done(string.Format(Translate.Get("missingcharacter"), Path.Combine(Helper.currentStoryPath, "Characters", name, line[2] + ".png")));

        //Done
        return NDI;
    }
    private void SetMood(string name, string mood)
    {
        GameObject character = new GameObject();
        bool found = false;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("character"))
        {
            if (go.name.Split('|')[0] == name)
            {
                Destroy(character.gameObject);
                character = go;
                found = true;
            }
        }
        if (found)
        {
            if (File.Exists(Path.Combine(Helper.currentStoryPath, "Characters", name + mood + ".png")))
                GameManager.Instance.dl.Sprite(character, $"file://{Path.Combine(Helper.currentStoryPath, "Characters", name + mood + ".png")}");
            else
                GameManager.Instance.dl.Sprite(character, $"file://{Path.Combine(Helper.currentStoryPath, "Characters", name, mood + ".png")}");
            character.name = $"{name}|{mood}";
        }
        GameManager.Instance.RunNext();
    }
}