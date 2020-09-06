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
    public void Run(string[] line)
    {
        NovelDebugInfo debugdata = Working(line);
        string status = debugdata.Message;
        if (debugdata.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "BACKGROUND|name|(clearcharacters)"));
            GameManager.Instance.RunNext();
            return;
        }
        bool clear = false;
        if (line.Length > 2) clear = line[2].ToLower() == "true";
                if (GameManager.currentBackground == line[1])
            GameManager.Instance.RunNext();
        else
            StartCoroutine(SetBackground(line[1], clear));
    }
    public NovelDebugInfo Working(string[] line)
    {
        NovelDebugInfo NDI = new NovelDebugInfo(line, GameManager.dialoguepos);

        if (line.Length > 3 || line.Length < 2) return NDI.Done(string.Format(Translate.Get("invalidargumentlength"), line.Length, "2-3"));
        if (!File.Exists(Path.Combine(Helper.currentStoryPath, "Backgrounds", line[1] + ".png"))) return NDI.Done(string.Format(Translate.Get("missingimage"), Path.Combine(GameManager.shortStoryPath, "Backgrounds", line[1] + ".png")));
        //$"{Helper.currentStoryPath}/Backgrounds/{line[1]}.png")
        return NDI;
    }
    private IEnumerator SetBackground(string back, bool clearCharacters)
    {
        GameManager.Instance.fadeimage.SetActive(true);
        if (GameManager.backgroundHasChanged)
        {
            GameManager.ready = false;
            GameManager.Instance.fadeimage.GetComponent<Animator>().Play("darken");
            yield return new WaitForSeconds(0.5f);
            GameManager.Instance.dl.RawImage(GameManager.Instance.background, $"file://{Path.Combine(Helper.currentStoryPath, "Backgrounds", back + ".png")}");
            GameManager.RemoveCharacters();
            GameManager.Instance.RunNext();
            GameManager.Instance.fadeimage.GetComponent<Animator>().Play("Fadein");
            GameManager.ready = true;
            yield return new WaitForSeconds(0.8f);
            GameManager.Instance.fadeimage.SetActive(false);
        }
        else
        {
            GameManager.Instance.dl.RawImage(GameManager.Instance.background, $"file://{Path.Combine(Helper.currentStoryPath, "Backgrounds", back + ".png")}");
            GameManager.Instance.RunNext();
        }
        GameManager.currentBackground = back;
        GameManager.backgroundHasChanged = true;
    }
}