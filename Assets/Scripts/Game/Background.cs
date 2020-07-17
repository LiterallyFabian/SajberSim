using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour, GameManager.INovelAction
{
    public GameManager Game;
    public void Run(string[] line)
    {
        string status = Working(line);
        if (status != "")
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "BG|name|(clearcharacters)"));
            Game.RunNext();
            return;
        }
        Download dl = GameObject.Find("Helper").GetComponent<Download>();
        dl.Image(Game.background, $"file://{Helper.currentStoryPath}/Backgrounds/{line[1]}.png");
        if (line.Length > 2) if (line[2] == "true") GameManager.RemoveCharacters();
        Game.RunNext();
    }
    public string Working(string[] line)
    {
        if (line.Length > 3) return $"Incorrect length, found {line.Length} arguments and expected 2 or 3.";
        if (line.Length < 2) return $"Missing arguments.";
        if (!File.Exists($"{Helper.currentStoryPath}/Backgrounds/{line[1]}.png")) return $"Image not found. Maybe there is a typo?\nExpected path: {GameManager.shortStoryPath}/Backgrounds/{line[1]}.png";
        return "";
    }
}
