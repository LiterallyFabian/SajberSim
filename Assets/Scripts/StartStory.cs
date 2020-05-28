using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using SajberSim.Helper;
using System.Linq;
using System.IO;
using UnityEngine.UI;
using SajberSim.Web;
using System.Net;
using System.Text;
using UnityEditor;

/// <summary>
/// Puts all downloaded thumbnails in the story menu
/// </summary>
public class StartStory : MonoBehaviour
{
    private Helper shelper = new Helper();
    private int page = 0;
    public GameObject StoryCardTemplate;
    public bool nsfw;
    Helper.StorySearchArgs searchArgs;
    Download dl;




    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("nsfw", 0) == 0) nsfw = false;
        else nsfw = true;
        dl = new GameObject("downloadobj").AddComponent<Download>();
        
    }
    public void UpdateNsfw(bool n)
    {
        if (n)
        {
            PlayerPrefs.SetInt("nsfw", 1);
            nsfw = true;
        }
        else
        {
            PlayerPrefs.SetInt("nsfw", 0);
            nsfw = false;
        }
        UpdatePreviewCards();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdatePreviewCards()
    {
        ClearPreviewCards();

        string[] storyPaths = shelper.GetAllStoryPaths(Helper.StorySearchArgs.Alphabetical, nsfw);
        string[] manifests = shelper.GetAllManifests(Helper.StorySearchArgs.Alphabetical, nsfw);
        for (int i = page * 6; i < page * 6 + 6; i++)
        {
            GameObject.Find("Canvas/StoryChoice/Pageinfo").GetComponent<Text>().text = $"Page {page + 1}/{shelper.GetCardPages() + 1}";
            if (manifests.Length == i) return; //cancel if story doesn't exist, else set all variables
            Debug.Log($"Importing manifest {i}: {manifests[i]}");
            Manifest storydata = JsonConvert.DeserializeObject<Manifest>(File.ReadAllText(manifests[i]));
            string name = storydata.name;
            string language = storydata.language.ToUpper();
            string overlaycolor = storydata.overlaycolor.Replace("#", "");
            string textcolor = storydata.textcolor.Replace("#", "");
            bool nsfw = storydata.nsfw;
            int playtime = storydata.playtime;

            //spawn, place and resize
            GameObject menu = Instantiate(StoryCardTemplate, Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas/StoryChoice").GetComponent<Transform>()) as GameObject;
            menu.transform.localPosition = Helper.CardPositions[Helper.CardPositions.Keys.ElementAt(i - (page * 6))];
            menu.transform.localScale = Vector3.one;
            menu.name = $"Card {i}";

            //fill with data
            if (File.Exists($"{storyPaths[i]}/thumbnail.png"))
                dl.CardThumbnail(GameObject.Find($"Canvas/StoryChoice/{menu.name}/Thumbnail"), $"{storyPaths[i]}/thumbnail.png");
            else
                GameObject.Find($"Canvas/StoryChoice/{menu.name}/Thumbnail").GetComponent<Image>().color = Color.white;

            Color splashColor = Color.white;
            ColorUtility.TryParseHtmlString($"#{overlaycolor}", out splashColor);
            GameObject.Find($"Canvas/StoryChoice/{menu.name}/Overlay").GetComponent<Image>().color = splashColor;

            Color textColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 1); //standard gray
            ColorUtility.TryParseHtmlString($"#{textcolor}", out textColor);
            GameObject.Find($"Canvas/StoryChoice/{menu.name}/Title").GetComponent<Text>().color = textColor;

            GameObject.Find($"Canvas/StoryChoice/{menu.name}/Title").GetComponent<Text>().text = name;

            GameObject.Find($"Canvas/StoryChoice/{menu.name}/Flag").GetComponent<Image>().sprite = dl.Flag(language);
        }
    }
    public void Play(int id)
    {
        Debug.Log($"Attempting to start story with ID {id}, path {shelper.GetAllStoryPaths()[id]}");
        PlayerPrefs.SetString("story", shelper.GetAllStoryNames()[id]);
        PlayerPrefs.SetString("script", "start");
        ButtonCtrl main = GameObject.Find("GameObject").GetComponent<ButtonCtrl>();
        main.CreateCharacters();
        StartCoroutine(main.FadeToScene("game"));


    }
    public void OpenDetails(int id)
    {
        Debug.Log($"Attempting to create details of local story with ID {id}, path {shelper.GetAllStoryPaths()[id]}");
    }
    public void ChangePage(int change)
    {
        ClearPreviewCards();
        if (page + change > shelper.GetCardPages()) page = 0;
        else if (page + change < 0) page = shelper.GetCardPages();
        else page += change;
        UpdatePreviewCards();
    }
    public void ClearPreviewCards()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("PreviewCard");

        for (int i = 0; i < gameObjects.Length; i++)
            Destroy(gameObjects[i]);
    }
    public void OpenMenu()
    {
        UpdatePreviewCards();
        GameObject.Find("Canvas/StoryChoice").GetComponent<Transform>().localScale = Vector3.one;
    }
    public void CloseMenu()
    {
        GameObject.Find("Canvas/StoryChoice").GetComponent<Transform>().localScale = Vector3.zero;
        ClearPreviewCards();
    }
    
    
}
