using SajberSim.Helper;
using SajberSim.Translation;
using System;
using System.IO;
using UnityEngine;

public class Character : MonoBehaviour, INovelAction
{
    public void Run(string[] line)
    {
        NovelDebugInfo debugdata = Working(line);
        string status = debugdata.Message;
        if (debugdata.Code == NovelDebugInfo.Status.Error)
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "CHAR|person|mood|x|y|(size)|(flip)"));
            GameManager.Instance.RunNext();
            return;
        }
        string name = "";
        if (int.TryParse(line[1], out int xd)) name = GameManager.people[int.Parse(line[1])].name; //ID if possible, else name
        else name = line[1];

        string mood = line[2];
        float x = (float)Convert.ToDouble(line[3], Language.Format);
        float y = (float)Convert.ToDouble(line[4], Language.Format);
        bool flip = false;
        float size = GameManager.charactersize;

        if (line.Length > 5) size = size * (float)Convert.ToDouble(line[5], Language.Format);
        if (line.Length == 7) if (line[6].ToLower() == "true") flip = true;
        CreateCharacter(name.ToLower(), mood, x, y, size, flip);
        GameManager.Instance.RunNext();
    }

    public NovelDebugInfo Working(string[] line)
    {
        NovelDebugInfo NDI = new NovelDebugInfo(line, GameManager.dialoguepos);

        ///Check length
        if (line.Length > 7 || line.Length < 5) return NDI.Done(string.Format(Translate.Get("invalidargumentlength"), line.Length, "5-7"));

        ///Assign name
        _CharacterConfig CC = _CharacterConfig.TryGetNameFromLine(line[1]);
        string name = CC.name;
        if (!CC.success) return NDI.Done(string.Format(Translate.Get("invalidcharacterconfig"), line[1], CC.customCharacters - 1, Path.Combine("Characters", "characterconfig.txt")));

        ///Check arguments 
        if (!File.Exists(Path.Combine(Helper.currentStoryPath, "Characters", name + line[2] + ".png")) && !File.Exists(Path.Combine(Helper.currentStoryPath, "Characters", name, line[2] + ".png"))) return NDI.Done(string.Format(Translate.Get("missingcharacter"), Path.Combine(GameManager.shortStoryPath, "Characters", name, line[2] + ".png")));
        if (!Helper.IsFloat(line[3])) return NDI.Done(string.Format(Translate.Get("invalidfloat"), $"X {Translate.Get("arg_coordinate")}", line[3]));
        if (!Helper.IsFloat(line[4])) return NDI.Done(string.Format(Translate.Get("invalidfloat"), $"Y {Translate.Get("arg_coordinate")}", line[4]));

        if (line.Length == 5) return NDI;
        if (!Helper.IsFloat(line[5])) return NDI.Done(string.Format(Translate.Get("invalidfloat"), Translate.Get("arg_size"), line[5]));

        return NDI;
    }

    private void CreateCharacter(string name, string mood, float x, float y, float size, bool flip) //ID 2
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
        if (!found) //karaktär finns ej
        {
            //skapa gameobj
            character.name = $"{name}|{mood}";
            character.gameObject.tag = "character";
            character.AddComponent<SpriteRenderer>();
            

            //sätt size + pos
            character.transform.position = new Vector3(x, y, -1f);
            character.transform.localScale = new Vector3(size * (flip ? -1 : 1), size, 0.6f);
        }
        else //karaktär finns
        {
            //ändra pos
            character.transform.position = new Vector3(x, y, -1f);
            character.transform.localScale = new Vector3(size * (flip ? -1 : 1), size, 0.6f);

            character.name = $"{name}|{mood}";
        }
        if (File.Exists(Path.Combine(Helper.currentStoryPath, "Characters", name + mood + ".png")))
            GameManager.Instance.dl.Sprite(character, $"file://{Path.Combine(Helper.currentStoryPath, "Characters", name + mood + ".png")}");
        else
            GameManager.Instance.dl.Sprite(character, $"file://{Path.Combine(Helper.currentStoryPath, "Characters", name, mood + ".png")}");
    }
}