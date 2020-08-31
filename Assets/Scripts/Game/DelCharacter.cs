using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelCharacter : MonoBehaviour, INovelAction
{
    public GameManager Game;

    public void Run(string[] line)
    {
        NovelDebugInfo debugdata = Working(line);
        string status = debugdata.Message;
        if (debugdata.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "DEL|char"));
            Game.RunNext();
            return;
        }
        string name = line[1];
        if (int.TryParse(line[1], out int xd)) name = GameManager.people[int.Parse(line[1])].name; //ID if possible, else 
        GameObject[] characters = GameObject.FindGameObjectsWithTag("character");
        foreach (GameObject character in characters)
            if (character.name.StartsWith(name.ToLower())) Destroy(character.gameObject);
        Game.RunNext();
    }
    public NovelDebugInfo Working(string[] line)
    {
        NovelDebugInfo NDI = new NovelDebugInfo(line, GameManager.dialoguepos);

        if (line.Length != 2) return NDI.Done(string.Format(Translate.Get("invalidargumentlength"), line.Length, 2));

        return NDI;
    }
}
