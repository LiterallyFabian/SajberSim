using SajberSim.Helper;
using SajberSim.Translation;
using System.Collections;
using UnityEngine;
using Prefs = SajberSim.Helper.Helper.Prefs;

public class Alert : MonoBehaviour, INovelAction
{
    public void Run(string[] line)
    {
        NovelDebugInfo debugdata = Working(line);
        string status = debugdata.Message;
        if (debugdata.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "ALERT|(text)"));
            return;
        }
        StartCoroutine(SpawnAlert(line[1]));
    }

    public NovelDebugInfo Working(string[] line)
    {
        NovelDebugInfo NDI = new NovelDebugInfo(line, GameManager.dialoguepos);

        if (line.Length != 2) return NDI.Done(string.Format(Translate.Get("invalidargumentlength"), line.Length, 2));

        return NDI;
    }

    private IEnumerator SpawnAlert(string target) //ID 0
    {
        GameManager.textdone = false;
        GameManager.Instance.alertbox.SetActive(true);
        string written = target[0].ToString(); //written = det som står hittills

        for (int i = 1; i < target.Length; i++)
        {
            written = written + target[i];
            yield return new WaitForSeconds(PlayerPrefs.GetFloat(Prefs.delay.ToString(), 0.04f));
            if (GameManager.textdone) //avbryt och skriv hela
            {
                GameManager.Instance.alert.text = target;
                GameManager.textdone = true;
                break;
            }
            GameManager.Instance.alert.text = written;
        }
        GameManager.Instance.alert.text = target;
        GameManager.textdone = true;
    }
}