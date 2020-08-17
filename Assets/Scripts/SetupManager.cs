using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Linq.Expressions;
using SajberSim.Web;
using SajberSim.Story;
using SajberSim.Chararcter;
using SajberSim.Helper;

public class SetupManager : MonoBehaviour
{
    //Story cur = new Story("start");
    string LatestEdit; //Latest edited or selected action
    string CurrentStory = "start"; //Name of .txt file
    string[] story; //Array with a full textfile
    string[] line; //Array with current action
    int dialogpos = 0; 
    string path;

    List<string> allmusic = new List<string>(); //list with all music
    List<string> allstories = new List<string>(); //list with all stories
    List<string> allcharsU = new List<string>(); //list with all Characters in uppercase
    List<string> allchars = new List<string>(); //list with all characters
    List<string> allbacks = new List<string>(); //list with all backgrounds
    List<string> allspawned = new List<string>(); //list with all currently spawned characters

    public GameObject background;

    //Character menu (right)
    public GameObject charmenu;
    public Text charmenutitle;
    bool charmenuopen = true;
    
    //Aestetics menu (left)
    public GameObject lookmenu;
    public Text lookmenutitle;
    bool lookmenuopen = true;

    //Dropdowns
    public Dropdown DDbackground;
    public Dropdown DDcreatechar;
    public Dropdown DDdeletechar;
    public Dropdown DDplaysound;
    public Dropdown DDplaymusic;
    public Dropdown DDstories;

    private string LastName = ""; // Last name used in a textbox

    //Objs
    Download dl;




    void Start()
    {
        dl = new GameObject("downloadobj").AddComponent<Download>();
        path = Application.dataPath;
        story = File.ReadAllLines($"{path}/Modding/Dialogues/{CurrentStory}.txt");
        while (true)
        {
            if (story[dialogpos].StartsWith("//") || story[dialogpos] == "")
                dialogpos++;
            else //first action catched
            {
                LatestEdit = story[dialogpos].Split('|')[0];
                break;
            }
        }
        FillLists();

    }

    void Update()
    {
        
    }

    public void ChangeBackground(int id)
    {
        background.name = allbacks[id];
        dl.Sprite(background, $"{path}/Modding/Backgrounds/{allbacks[id]}.png");
    }
    public void FillLists() //adds audio, backgrounds, stories and characters to a list, and then updates the dropdowns
    {
        string dialoguePath = $@"{Application.dataPath}/Modding/Dialogues/";
        string audioPath = $@"{Application.dataPath}/Modding/Audio/";
        string charPath = $@"{Application.dataPath}/Modding/Characters/";
        string backgroundPath = $@"{Application.dataPath}/Modding/Backgrounds/";

        string[] storyPaths = Directory.GetFiles(dialoguePath, "*.txt");
        string[] audioPaths = Directory.GetFiles(audioPath, "*.ogg");
        string[] charPaths = Directory.GetFiles(charPath, "*neutral.png");
        string[] backgroundPaths = Directory.GetFiles(backgroundPath, "*.png");

        //Fills lists
        for (int i = 0; i < storyPaths.Length; i++)
            allstories.Add(storyPaths[i].Replace(dialoguePath, "").Replace(".txt", ""));

        allmusic.Add("");
        for (int i = 0; i < audioPaths.Length; i++)
            allmusic.Add(audioPaths[i].Replace(audioPath, "").Replace(".ogg", ""));

        allcharsU.Add("");
        for (int i = 0; i < charPaths.Length; i++)
        {
            string name = charPaths[i].Replace(charPath, "").Replace("neutral.png", "");
            allcharsU.Add(Char.ToUpper(name[0]) + name.Remove(0, 1));
            allchars.Add(name);
        }
        

        for (int i = 0; i < backgroundPaths.Length; i++)
            allbacks.Add(backgroundPaths[i].Replace(backgroundPath, "").Replace(".png", ""));

        //Put current story first
        allstories = SetFirst(CurrentStory, allstories);
        


        //Fills dropdowns
        DDbackground.ClearOptions();
        DDbackground.AddOptions(allbacks);
        DDcreatechar.ClearOptions();
        DDcreatechar.AddOptions(allcharsU);
        DDplaymusic.ClearOptions();
        DDplaymusic.AddOptions(allmusic);
        DDplaysound.ClearOptions();
        DDplaysound.AddOptions(allmusic);
        DDstories.ClearOptions();
        DDstories.AddOptions(allstories);

    }
    private List<string> SetFirst(string first, List<string> li)
    {
        List<string> oldlist = li;
        int index = li.FindIndex(x => x.StartsWith(first));
        li[0] = li[index];
        li[index] = oldlist[0];
        
        return li;
    }
    public void ToggleMenu(string id)
    {
        if(id == "char")
        {
            if (charmenuopen) //close menu
            {
                charmenu.GetComponent<RectTransform>().offsetMax = new Vector2(0, -425); //-right, -top
                charmenu.GetComponent<RectTransform>().offsetMin = new Vector2(649, -182); //left, bottom
                charmenutitle.text = " Karaktärmeny";
                GameObject.Find("/Canvas/CharPanel/ToggleCharMenu").GetComponent<Transform>().rotation = new Quaternion(0, 0, 90, 0);
            }
            else //open menu
            {
                charmenu.GetComponent<RectTransform>().offsetMax = new Vector2(0, -241);
                charmenu.GetComponent<RectTransform>().offsetMin = new Vector2(649, 0);
                charmenutitle.text = " Skapa karaktär";
                GameObject.Find("/Canvas/CharPanel/ToggleCharMenu").GetComponent<Transform>().rotation = new Quaternion(0, 0, 0, 0);
            }
            charmenuopen = !charmenuopen;
        }
        else if(id == "look")
        {
            if (lookmenuopen) //close menu
            {
                lookmenu.GetComponent<RectTransform>().offsetMax = new Vector2(-652, -426); //-right, -top
                lookmenu.GetComponent<RectTransform>().offsetMin = new Vector2(0, -230); //left, bottom
                lookmenutitle.text = " Utseendemeny";
                GameObject.Find("/Canvas/LookPanel/ToggleLookMenu").GetComponent<Transform>().rotation = new Quaternion(0, 0, 90, 0);
            }
            else //open menu
            {
                lookmenu.GetComponent<RectTransform>().offsetMax = new Vector2(-652, -195); //-right, -top
                lookmenu.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0); //left, bottom
                lookmenutitle.text = " Byt bakgrund";
                GameObject.Find("/Canvas/LookPanel/ToggleLookMenu").GetComponent<Transform>().rotation = new Quaternion(0, 0, 0, 0);
            }
            lookmenuopen = !lookmenuopen;
        }
    }
    /// <summary>
    /// Changes active story, picked from dropdown ingame
    /// </summary>
    public void SetStory(int ID)
    {

    }
    /// <summary>
    /// Submits textbox ingame
    /// </summary>
    public void SubmitTextbox()
    {
        string name = GameObject.Find("/Canvas/Textbox/NameInput").GetComponent<InputField>().text;
        string msg = GameObject.Find("/Canvas/Textbox/TextInput").GetComponent<InputField>().text;
        AddLine($"T|{name}|{msg}",0);
    }
    /// <summary>
    /// Adds action to current story script
    /// </summary>
    /// <param name="line">Completed line to add</param>
    /// <param name="pos">Position to add line at</param>
    private void AddLine(string line, int pos)
    {
        LatestEdit = line.Split('|')[0];
        using (StreamWriter sw = new StreamWriter($"{path}/Modding/Dialogues/{CurrentStory}.txt", true))
        {
            sw.Write($"\n{line}");
        }
        dialogpos++;
    }
    public void UpdateName(string input) //När man uppdaterar namn i textbox
    {
        LastName = input;
        GameObject port = GameObject.Find("/Canvas/Textbox/Portrait");
        if (allchars.Contains(input.ToLower()))
        {
            dl.Image(port, $"file://{path}/Modding/Characters/{input.ToLower()}port.png");
            GameObject.Find("/Canvas/Textbox/NameInput/Text").GetComponent<Text>().color = new Color32(23, 79, 23, 255);

        }
        else if(Helper.IsNum(input))
        {
            dl.Image(port, $"file://{path}/Modding/Characters/unknown.png");
            GameObject.Find("/Canvas/Textbox/NameInput/Text").GetComponent<Text>().color = new Color32(23, 79, 23, 255);
        }
        else
            GameObject.Find("/Canvas/Textbox/NameInput/Text").GetComponent<Text>().color = new Color32(90, 23, 23, 255);
    }
    public void SubmitCharacter(int id) //input from dropdown
    {
        if (id == 0) return;
        string name = allcharsU[id];
        allspawned.Add(name);
        CreateCharacter(name);
        FillLists();
    }
    void CreateCharacter(string id) //creates the character
    {
        GameObject character = new GameObject($"{id.ToLower()}_neutral");
        character.gameObject.tag = "character";
        character.AddComponent<SpriteRenderer>();
        dl.Sprite(character, $"file://{path}/Modding/Characters/{id.ToLower()}neutral.png");

        //sätt size + pos
        character.transform.position = new Vector3(0, 0, -1f);
        character.transform.localScale = new Vector3(GameManager.charactersize, GameManager.charactersize, 0.6f);
        character.AddComponent<BoxCollider2D>().size = new Vector2(3.7f, 11f);
        character.GetComponent<BoxCollider2D>().offset = new Vector2(0, -1);
        character.AddComponent<CharacterCreation>();
        
    }
    /// <summary>
    /// Goes from start to a line in the current story.
    /// </summary>
    public void GoToLine(int Line)
    {

    }
    public void GoToNextLine()
    {
        
    }
    public void GoToPreviousLine()
    {
        dialogpos--;
        GoToLine(dialogpos);
    }
    public void GoToLastLine()
    {

    }
    public void GoToFirstLine()
    {
        
    }
    
}
