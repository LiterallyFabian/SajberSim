using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DelCharacter : MonoBehaviour, GameManager.INovelAction
{
    public GameManager Game;

    public void Run(string[] line)
    {
        string status = Working(line);
        if (status != "")
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "DEL|char"));
            Game.RunNext();
            return;
        }
        string name = line[1];
        if (int.TryParse(line[1], out int xd)) name = Game.people[int.Parse(line[1])].name; //ID if possible, else 
        GameObject character = GameObject.Find(name.ToLower());
        if (character) Destroy(character.gameObject);
        Game.RunNext();
    }
    public string Working(string[] line)
    {
        if (line.Length != 2) return $"The length of the line is {line.Length}, while the expected is 2.";
        return "";
    }
}
