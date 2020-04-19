using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class SetupManager : MonoBehaviour
{
    string LatestEdit; //Latest edited or selected action
    string[] story; //Array with a full textfile
    string[] line; //Array with current action
    int dialogpos = 0; 
    string path;

    List<string> allmusic = new List<string>(); //list with all music
    List<string> allstories = new List<string>(); //list with all stories
    List<string> allchars = new List<string>(); //list with all characters
    List<string> allbacks = new List<string>(); //list with all backgrounds

    public GameObject background;

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
    public void FillLists() //adds audio, backgrounds, stories and characters to a list
    {
        string dialoguePath = $@"{Application.dataPath}/Modding/Dialogues/".Replace("/", "\\");
        string audioPath = $@"{Application.dataPath}/Modding/Audio/".Replace("/", "\\");
        string charPath = $@"{Application.dataPath}/Modding/Characters/".Replace("/", "\\");
        string backgroundPath = $@"{Application.dataPath}/Modding/Backgrounds/".Replace("/", "\\");

        string[] storyPaths = Directory.GetFiles(dialoguePath, "*.txt");
        string[] audioPaths = Directory.GetFiles(audioPath, "*.ogg");
        string[] charPaths = Directory.GetFiles(charPath, "*neutral.png");
        string[] backgroundPaths = Directory.GetFiles(dialoguePath, "*.png");


        for (int i = 0; i < storyPaths.Length; i++)
            allstories.Add(storyPaths[i].Replace(dialoguePath, "").Replace(".txt", ""));

        for (int i = 0; i < audioPaths.Length; i++)
            allmusic.Add(audioPaths[i].Replace(audioPath, "").Replace(".ogg", ""));

        for (int i = 0; i < charPaths.Length; i++)
            allchars.Add(charPaths[i].Replace(charPath, "").Replace("neutral.png", ""));

        for (int i = 0; i < backgroundPath.Length; i++)
            allbacks.Add(backgroundPaths[i].Replace(backgroundPath, "").Replace(".png", ""));
    }
}
