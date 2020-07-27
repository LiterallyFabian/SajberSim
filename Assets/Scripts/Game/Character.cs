using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour, GameManager.INovelAction
{
    public GameManager Game;

    public void Run(string[] line)
    {
        string status = Working(line);
        if (status != "")
        {
            UnityEngine.Debug.LogWarning($"Error at line {GameManager.dialoguepos} in script {GameManager.scriptPath}: {status}");
            Helper.Alert(string.Format(Translate.Get("erroratline"), GameManager.dialoguepos, GameManager.scriptPath, string.Join("|", line), status, "CHAR|person|mood|x|y|(size)|(flip)"));
            Game.RunNext();
            return;
        }
        string name = "";
        if (int.TryParse(line[1], out int xd)) name = Game.people[int.Parse(line[1])].name; //ID if possible, else name
        else name = line[1];

        string mood = line[2];
        float x = (float)Convert.ToDouble(line[3], Language.Format);
        float y = (float)Convert.ToDouble(line[4], Language.Format);
        bool flip = false;
        float size = GameManager.charactersize;
        
        if (line.Length > 5) size = size * (float)Convert.ToDouble(line[5], Language.Format);
        if (line.Length == 7) if (line[6].ToLower() == "true") flip = true;
        CreateCharacter(name.ToLower(), mood, x, y, size, flip);
        Game.RunNext();
    }
    public string Working(string[] line)
    {
        if (line.Length > 7 || line.Length < 5) return string.Format(Translate.Get("invalidargumentlength"), line.Length, "5-7");
        string name = "";

        if (Helper.IsNum(line[1])) name = Game.people[int.Parse(line[1])].name; //ID if possible, else name
        else name = line[1];
        if (!File.Exists($"{Helper.currentStoryPath}/Characters/{name}{line[2]}.png")) return string.Format(Translate.Get("missingcharacter"), $"{GameManager.shortStoryPath}/Characters/{name}{line[2]}.png");
        if (!Helper.IsFloat(line[3])) return string.Format(Translate.Get("invalidfloat"), $"X {Translate.Get("arg_coordinate")}", line[3]);
        if (!Helper.IsFloat(line[4])) return string.Format(Translate.Get("invalidfloat"), $"Y {Translate.Get("arg_coordinate")}", line[4]);
        if (line.Length == 5) return "";
        if (!Helper.IsFloat(line[5])) return string.Format(Translate.Get("invalidfloat"), Translate.Get("arg_size"), line[5]);
        return "";
    }
    private void CreateCharacter(string name, string mood, float x, float y, float size, bool flip) //ID 2
    {
        if (GameObject.Find(name) == null) //karaktär finns ej
        {
            //skapa gameobj
            GameObject character = new GameObject(name);
            character.gameObject.tag = "character";
            character.AddComponent<SpriteRenderer>();
            Game.dl.Sprite(character, $"file://{Helper.currentStoryPath}/Characters/{name}{mood}.png");

            //sätt size + pos
            character.transform.position = new Vector3(x, y, -1f);
            character.transform.localScale = new Vector3(size * (flip ? 1 : -1), size, 0.6f);
        }
        else //karaktär finns
        {
            //ändra pos
            GameObject character = GameObject.Find(name);
            character.transform.position = new Vector3(x, y, -1f);
            character.transform.localScale = new Vector3(size * (flip ? 1 : -1), size, 0.6f);

            //ändra mood
            Game.dl.Sprite(character, $"file://{Helper.currentStoryPath}/Characters/{name}{mood}.png");
        }
    }
}
