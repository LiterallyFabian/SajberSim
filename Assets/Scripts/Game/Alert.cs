using SajberSim.Helper;
using SajberSim.Translation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Alert : MonoBehaviour, GameManager.INovelAction
{
    public GameManager Game;
    public void Run(string[] line)
    {
        string status = Working(line);
        if (status != "")
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "ALERT|(text)"));
            return;
        }
        StartCoroutine(SpawnAlert(line[1]));
    }
    public string Working(string[] line)
    {
        if (line.Length != 2) return $"The length of the line is {line.Length}, while it should be 2.";
        return "";
    }
    private IEnumerator SpawnAlert(string target) //ID 0
    {
        GameManager.textdone = false;
        Game.alertbox.SetActive(true);
        string written = target[0].ToString(); //written = det som står hittills

        for (int i = 1; i < target.Length; i++)
        {
            written = written + target[i];
            yield return new WaitForSeconds(PlayerPrefs.GetFloat("delay", 0.04f));
            if (GameManager.textdone) //avbryt och skriv hela
            {
                Game.alert.text = target;
                GameManager.textdone = true;
                break;
            }
            Game.alert.text = written;
        }
        Game.alert.text = target;
        GameManager.textdone = true;
    }
}

