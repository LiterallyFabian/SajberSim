using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SetupManager : MonoBehaviour
{
    string LatestEdit; //Latest edited or selected action
    string[] story; //Array with a full textfile
    string[] line; //Array with current action
    int dialogpos = 0; 
    string path;

    List<string> allmusic = new List<string>(); //list with all music
    List<string> allstories = new List<string>(); //list with all stories
    List<string> allcharsU = new List<string>(); //list with all Characters
    List<string> allchars = new List<string>(); //list with all characters
    List<string> allbacks = new List<string>(); //list with all backgrounds

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




    void Start()
    {
        path = Application.dataPath;
        story = File.ReadAllLines($"{path}/Modding/Dialogues/start.txt");
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
    IEnumerator ChangeBackground(string bg, GameObject item)
    {
        Debug.Log($"New background loaded: {bg}");
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{Application.dataPath}/Modding/Backgrounds/{bg}.png"))
        {
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError) Debug.LogError($"An error occured while downloading background: {uwr.error}");
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                item.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }

        }
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

        for (int i = 0; i < audioPaths.Length; i++)
            allmusic.Add(audioPaths[i].Replace(audioPath, "").Replace(".ogg", ""));

        for (int i = 0; i < charPaths.Length; i++)
        {
            string name = charPaths[i].Replace(charPath, "").Replace("neutral.png", "");
            allcharsU.Add(Char.ToUpper(name[0]) + name.Remove(0, 1));
            allchars.Add(name);
        }

        for (int i = 0; i < backgroundPaths.Length; i++)
            allbacks.Add(backgroundPaths[i].Replace(backgroundPath, "").Replace(".png", ""));

        //Fills dropdowns
        DDbackground.AddOptions(allbacks);
        DDcreatechar.AddOptions(allcharsU);
        DDplaymusic.AddOptions(allmusic);
        DDplaysound.AddOptions(allmusic);
        DDstories.AddOptions(allstories);
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
            }
            else //open menu
            {
                charmenu.GetComponent<RectTransform>().offsetMax = new Vector2(0, -241);
                charmenu.GetComponent<RectTransform>().offsetMin = new Vector2(649, 0);
                charmenutitle.text = " Skapa karaktär";
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
            }
            else //open menu
            {
                lookmenu.GetComponent<RectTransform>().offsetMax = new Vector2(-652, -195); //-right, -top
                lookmenu.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0); //left, bottom
                lookmenutitle.text = " Byt bakgrund";
            }
            lookmenuopen = !lookmenuopen;
        }
    }
    public void SetStory(int ID)
    {

    }
    public void UpdateName(string input) //När man uppdaterar namn i textbox
    {
        GameObject port = GameObject.Find("/Canvas/Textbox/Portrait");
        if (allchars.Contains(input.ToLower()))
        {
            StartCoroutine(UpdateSprite($"file://{Application.dataPath}/Modding/Characters/{input.ToLower()}port.png", port));
            GameObject.Find("/Canvas/Textbox/NameInput/Text").GetComponent<Text>().color = new Color32(23, 79, 23, 255);

        }
        else if(IsNum(input))
        {
            StartCoroutine(UpdateSprite($"file://{Application.dataPath}/Modding/Characters/unknown.png", port));
            GameObject.Find("/Canvas/Textbox/NameInput/Text").GetComponent<Text>().color = new Color32(23, 79, 23, 255);
        }
        else
            GameObject.Find("/Canvas/Textbox/NameInput/Text").GetComponent<Text>().color = new Color32(90, 23, 23, 255);
    }
    private bool IsNum(string input)
    {
        if (int.TryParse(input, out int n)) return true;
        else return false;
    }
    IEnumerator UpdateSprite(string path, GameObject item)
    {
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path);
        yield return uwr.SendWebRequest();
        var texture = DownloadHandlerTexture.GetContent(uwr);
        item.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
