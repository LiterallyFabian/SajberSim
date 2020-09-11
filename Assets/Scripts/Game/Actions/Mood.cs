using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Mood : MonoBehaviour, INovelAction
{
    public void Run(string[] line)
    {
        NovelDebugInfo debugdata = Working(line);
        string status = debugdata.Message;
        if (debugdata.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "MOOD|character|mood"));
            GameManager.Instance.RunNext();
            return;
        }
        string name = _CharacterHelper.TryGetNameFromLine(line[1]).name;
        SetMood(name.ToLower(), line[2]);
    }
    public NovelDebugInfo Working(string[] line)
    {
        NovelDebugInfo NDI = new NovelDebugInfo(line, GameManager.dialoguepos);

        //Start debugging:
        if (line.Length != 3) return NDI.Done(string.Format(Translate.Get("invalidargumentlength"), line.Length, "3"));

        ///Assign name
        _CharacterHelper CC = _CharacterHelper.TryGetNameFromLine(line[1]);
        string name = CC.name;
        if (!CC.success) return NDI.Done(string.Format(Translate.Get("invalidcharacterconfig"), line[1], CC.customCharacters - 1, Path.Combine(GameManager.shortStoryPath, "Characters", "characterconfig.txt")));

        if (!_CharacterHelper.GetPath(name, line[2]).success) return NDI.Done(string.Format(Translate.Get("missingcharacter"), Path.Combine(Helper.currentStoryPath, "Characters", name, line[2] + ".png")));

        //Done
        return NDI;
    }
    private void SetMood(string name, string mood)
    {
        GameObject character = new GameObject();
        bool found = false;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("character"))
        {
            if (go.name.Split('|')[0] == name)
            {
                Destroy(character.gameObject);
                character = go;
                found = true;
            }
        }
        if (found)
        {
            GameManager.Instance.dl.Sprite(character, $"file://{_CharacterHelper.GetPath(name, mood).path}");
            character.name = $"{name}|{mood}";
        }
        GameManager.Instance.RunNext();
    }
}