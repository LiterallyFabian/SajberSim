using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour, INovelAction
{
    public GameManager Game;
    public void Run(string[] line)
    {
        NovelDebugInfo status = Working(line);
        if (status.Code == NovelDebugInfo.Status.Error)
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
    public NovelDebugInfo Working(string[] line)
    {
        if (line.Length > 3 || line.Length < 2) return NovelDebugInfo.Error(string.Format(Translate.Get("invalidargumentlength"), line.Length, "2-3"));
        if (!File.Exists($"{Helper.currentStoryPath}/Backgrounds/{line[1]}.png")) return NovelDebugInfo.Error(string.Format(Translate.Get("missingimage"), $"{GameManager.shortStoryPath}/Backgrounds/{line[1]}.png"));
        return NovelDebugInfo.OK();
    }
    private IEnumerator SetBackground(string back)
    {
        Game.fadeimage.SetActive(true);
        if (GameManager.backgroundHasChanged)
        {
            Game.fadeimage.GetComponent<Animator>().Play("darken");
            yield return new WaitForSeconds(0.5f);
            Game.dl.RawImage(Game.background, $"file://{Helper.currentStoryPath}/Backgrounds/{back}.png");
            Game.RunNext();
            Game.fadeimage.GetComponent<Animator>().Play("Fadein");
            yield return new WaitForSeconds(0.8f);
            Game.fadeimage.SetActive(false);
        }
        else
        {
            Game.dl.RawImage(Game.background, $"file://{Helper.currentStoryPath}/Backgrounds/{back}.png");
            Game.RunNext();
        }
        GameManager.currentBackground = back;
        GameManager.backgroundHasChanged = true;
    }
}
