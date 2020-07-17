using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Textbox : MonoBehaviour, GameManager.INovelAction
{
    private Text textobj;
    private Text nameobj;
    public GameManager Game;

    public void Run(string[] line)
    {
        GameManager.textdone = false;
        string status = Working(line);
        if (status != "")
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "T|person|text|(showportrait)"));
            GameManager.textdone = true;
            return;
        }
        Person talker;
        if (int.TryParse(line[1], out int x))
            talker = Game.people[int.Parse(line[1])];
        else
            talker = new Person(line[1], "", 0);
        string text = Game.FillVars(line[2]);

        bool port = true;
        Debug.Log($"{talker.name} says: {text}");
        if (line.Length == 4) if (line[3] == "false") port = false;
        StartCoroutine(SpawnTextBox(talker, Helper.UwUTranslator(text), port));
    }

    public string Working(string[] line)
    {
        if (line.Length > 4 || line.Length < 3) return $"The length of the line is {line.Length}, while the expected is 3 or 4.";
        return "";
    }
    public IEnumerator SpawnTextBox(Person talker, string target, bool port) //ID 0
    {
        ChangeTextboxType(port);
        Download dl = GameObject.Find("Helper").GetComponent<Download>();
        Game.textbox.SetActive(true);
        dl.Image(Game.portrait, $"file://{Helper.currentStoryPath}/Characters/{talker.name.ToLower()}port.png");
        nameobj.text = talker.name;

        if (PlayerPrefs.GetFloat("delay", 0.04f) > 0.001f) //ifall man stängt av typing speed är denna onödig
        {
            string written = target[0].ToString(); //written = det som står hittills

            for (int i = 1; i < target.Length; i++)
            {
                written = written + target[i];
                yield return new WaitForSeconds(PlayerPrefs.GetFloat("delay", 0.04f));
                if (GameManager.textdone) //avbryt och skriv hela
                {
                    textobj.text = target;
                    GameManager.textdone = true;
                    break;
                }
                textobj.text = written;
            }
        }
        textobj.text = target;
        GameManager.textdone = true;
    }
    private void ChangeTextboxType(bool portrait)
    {
        if (portrait)
        {
            textobj = Game.commentPort;
            nameobj = Game.nametagPort;
            Game.portrait.transform.localScale = Vector3.one;
        }
        else
        {
            textobj = Game.comment;
            nameobj = Game.nametag;
            Game.portrait.transform.localScale = Vector3.zero;
        }
    }
}