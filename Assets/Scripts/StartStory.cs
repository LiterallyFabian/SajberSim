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
using System.Threading;

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
        GameObject.Find("Canvas/StoryChoice/NSFWtoggle").GetComponent<Toggle>().SetIsOnWithoutNotify(nsfw);
        dl = new GameObject("downloadobj").AddComponent<Download>();
        
    }
    public void UserUpdateNsfw(bool n)
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
    public void UserUpdateSort(int n)
    {
        searchArgs = (Helper.StorySearchArgs)n;
        Debug.Log(searchArgs);
        UpdatePreviewCards();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdatePreviewCards()
    {
        Debug.Log("Request to update cards");
        string[] storyPaths = shelper.GetAllStoryPaths(searchArgs, nsfw);
        string[] manifests = shelper.GetAllManifests(searchArgs, nsfw);
        
        for (int i = page * 6; i < page * 6 + 6; i++)
        {
            GameObject.Find("Canvas/StoryChoice/Pageinfo").GetComponent<Text>().text = $"Page {page + 1}/{shelper.GetCardPages(searchArgs, nsfw)+1}";
            if (manifests.Length == i) return; //cancel if story doesn't exist, else set all variables
            Debug.Log($"Importing manifest {i}: {manifests[i]}");
            Manifest storydata = JsonConvert.DeserializeObject<Manifest>(File.ReadAllText(manifests[i]));
            string name = storydata.name;
            string language = storydata.language.ToUpper();
            string overlaycolor = storydata.overlaycolor.Replace("#", "");
            string textcolor = storydata.textcolor.Replace("#", "");
            bool isnsfw = storydata.nsfw;
            int playtime = storydata.playtime;

            GameObject menu;
            //spawn, place and resize
            if (GameObject.Find($"Canvas/StoryChoice/Card {i}") == null) menu = Instantiate(StoryCardTemplate, Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas/StoryChoice").GetComponent<Transform>()) as GameObject;
            else menu = GameObject.Find($"Canvas/StoryChoice/Card {i}");
           
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
        Debug.Log($"Attempting to start story with ID {id}, path {shelper.GetAllStoryPaths(searchArgs, nsfw)[id]}");
        PlayerPrefs.SetString("story", shelper.GetAllStoryNames(searchArgs, nsfw)[id]);
        PlayerPrefs.SetString("script", "start");
        ButtonCtrl main = GameObject.Find("GameObject").GetComponent<ButtonCtrl>();
        main.CreateCharacters();
        StartCoroutine(main.FadeToScene("game"));


    }
    public void OpenDetails(int id)
    {
        Debug.Log($"Attempting to create details of local story with ID {id}, path {shelper.GetAllStoryPaths(searchArgs, nsfw)[id]}");
    }
    public void ChangePage(int change)
    {
        if (shelper.GetCardPages(searchArgs, nsfw) == 0)
        {
            GameObject.Find("Canvas/StoryChoice/Pageinfo").GetComponent<Animator>().Play("storycard_pageinfojump", 0, 0);
            return;
        }

        ClearPreviewCards();
        if (page + change > shelper.GetCardPages(searchArgs, nsfw)) page = 0;
        else if (page + change < 0) page = shelper.GetCardPages(searchArgs, nsfw);
        else page += change;
        UpdatePreviewCards();
    }
    public void ClearPreviewCards()
    {
        Debug.Log("Request to delete preview cards");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("PreviewCard");
        foreach (GameObject enemy in enemies)
            GameObject.Destroy(enemy);
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
