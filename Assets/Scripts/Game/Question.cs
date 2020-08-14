using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Question : MonoBehaviour, INovelAction
{
    public GameManager Game;
    public void Run(string[] line)
    {
        NovelDebugInfo debugdata = Working(line);
        string status = debugdata.Message;
        if (debugdata.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "QUESTION|title|alt1_text|alt1_path|alt2_text|alt2_path|(altN_text|altN_path)"));
            Game.ToggleDevmenu(true);

            return;
        }
        if (line.Length == 6) //Normal 2 alt questions
        {
            string quest = line[1];
            string alt1 = line[2];
            Game.story1 = line[3];
            string alt2 = line[4];
            Game.story2 = line[5];
            OpenQuestion(quest, alt1, alt2);
        }
        else //More questions - dropdown menu
        {
            OpenQuestionDD(line);
        }
    }
    public NovelDebugInfo Working(string[] line)
    {
        if (line.Length < 6) return NovelDebugInfo.Error(string.Format(Translate.Get("missingarguments"), line.Length, "6+")); //Missing arguments, only found LENGTH arguments but the action expects 6+.
        if ((line.Length - 2) % 2 != 0) return NovelDebugInfo.Error(string.Format(Translate.Get("invalidargumentlength"), line.Length, "alternatives * 2 + 2")); //Incorrect length, found LENGTH arguments but the action expects alternatives * 2 + 2.
        int script = 0;
        for (int i = 3; i < line.Length; i += 2)
        {
            script++;
            if (!File.Exists($"{Helper.currentStoryPath}/Dialogues/{line[i]}.txt")) return NovelDebugInfo.Error($"Alternative {script} \"{line[i-1]}\" leads to the script \"{line[i]}\" which does not exist.");
        }
        return NovelDebugInfo.OK();
    }
    private void OpenQuestion(string question, string alt1, string alt2)
    {
        Game.questionbox.SetActive(true);
        Game.questionbox.transform.Find("Question").GetComponent<Text>().text = question;
        Game.questionbox.transform.Find("Alt1/Text").GetComponent<Text>().text = alt1;
        Game.questionbox.transform.Find("Alt2/Text").GetComponent<Text>().text = alt2;
    }
    private void OpenQuestionDD(string[] line)
    {
        Game.dropdownMenu.SetActive(true);
        Game.dropdownObject.GetComponent<Dropdown>().ClearOptions();
        List<string> options = new List<string>();
        options.Add("");
        for (int i = 2; i < line.Length; i += 2)
        {
            options.Add(line[i]);
        }
        Game.dropdownMenu.transform.Find("Question").GetComponent<Text>().text = line[1];
        Game.dropdownObject.GetComponent<Dropdown>().AddOptions(options);
    }
}
