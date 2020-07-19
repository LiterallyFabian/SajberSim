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
        if (GameManager.currentBackground == line[1])
            Game.RunNext();
        else
            StartCoroutine(SetBackground(line[1]));
        if (line.Length > 2) if (line[2] == "true") GameManager.RemoveCharacters();
    }
    public string Working(string[] line)
    {
        if (line.Length > 3) return $"Incorrect length, found {line.Length} arguments and expected 2 or 3.";
        if (line.Length < 2) return $"Missing arguments.";
        if (!File.Exists($"{Helper.currentStoryPath}/Backgrounds/{line[1]}.png")) return $"Image not found. Maybe there is a typo?\nExpected path: {GameManager.shortStoryPath}/Backgrounds/{line[1]}.png";
        return "";
    }
    private IEnumerator SetBackground(string back)
    {
        Game.fadeimage.SetActive(true);
        if (GameManager.backgroundHasChanged)
        {
            Game.fadeimage.GetComponent<Animator>().Play("darken");
            yield return new WaitForSeconds(0.5f);
            Game.dl.Image(Game.background, $"file://{Helper.currentStoryPath}/Backgrounds/{back}.png");
            Game.RunNext();
            Game.fadeimage.GetComponent<Animator>().Play("Fadein");
            yield return new WaitForSeconds(0.8f);
            Game.fadeimage.SetActive(false);
        }
        else
        {
            Game.dl.Image(Game.background, $"file://{Helper.currentStoryPath}/Backgrounds/{back}.png");
            Game.RunNext();
        }
        GameManager.currentBackground = back;
        GameManager.backgroundHasChanged = true;
       
    }
}
