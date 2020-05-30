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
using System;

/// <summary>
/// Puts all downloaded thumbnails in the story menu
/// </summary>
public class StartStory : MonoBehaviour
{
    private Helper shelper = new Helper();

    private int page = 0; //current page in story card menu, starting at 0

    public GameObject StoryCardTemplate;
    public bool nsfw;
    Helper.StorySearchArgs sortArgs;
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
            if(page != 0) ResetPage();
        }
        UpdatePreviewCards();
    }
    public void UserUpdateSort(int n)
    {
        sortArgs = (Helper.StorySearchArgs)n;
        Debug.Log($"Sorting arguments changed: {sortArgs}");
        UpdatePreviewCards();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdatePreviewCards()
    {
        Debug.Log("Request to update cards");
        string[] storyPaths = shelper.GetAllStoryPaths(sortArgs, nsfw);
        string[] manifests = shelper.GetAllManifests(sortArgs, nsfw);
        ClearPreviewCards();
        for (int i = page * 6; i < page * 6 + 6; i++)
        {
            GameObject.Find("Canvas/StoryChoice/Pageinfo").GetComponent<Text>().text = $"Page {page + 1}/{shelper.GetCardPages(sortArgs, nsfw)+1}";
            if (manifests.Length == i) return; //cancel if story doesn't exist, else set all variables
            Debug.Log($"Importing manifest {i}: {manifests[i]}");
            Manifest storydata = JsonConvert.DeserializeObject<Manifest>(File.ReadAllText(manifests[i]));
            Vector3 position = Helper.CardPositions[Helper.CardPositions.Keys.ElementAt(i - (page * 6))];
            CreateCard(storyPaths[i], storydata, position);
            
        }
    }
    private void CreateCard(string storyPath, Manifest data, Vector3 pos)
    {
        string name = data.name;
        string id = storyPath.Replace($"{Application.dataPath}/Story/", "");
        string language = data.language.ToUpper();
        string overlaycolor = data.overlaycolor.Replace("#", "");
        string textcolor = data.textcolor.Replace("#", "");
        bool isnsfw = data.nsfw;
        int playtime = data.playtime;
        if (GameObject.Find($"Canvas/StoryChoice/{id}")) Destroy(GameObject.Find($"Canvas/StoryChoice/{id}").gameObject);
        //spawn, place and resize
        GameObject menu = Instantiate(StoryCardTemplate, Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas/StoryChoice").GetComponent<Transform>()) as GameObject; ;
        
        menu.transform.localPosition = pos;
        //menu.transform.localScale = Vector3.one;
        menu.name = $"{id}";

        

        //Fill with data
        
            GameObject.Find($"Canvas/StoryChoice/{menu.name}/Thumbnail").GetComponent<Image>().color = Color.white;

        Color splashColor = Color.white;
        ColorUtility.TryParseHtmlString($"#{overlaycolor}", out splashColor);
        GameObject.Find($"Canvas/StoryChoice/{menu.name}/Overlay").GetComponent<Image>().color = splashColor;

        Color textColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 1); //standard gray
        ColorUtility.TryParseHtmlString($"#{textcolor}", out textColor);
        GameObject.Find($"Canvas/StoryChoice/{menu.name}/Title").GetComponent<Text>().color = textColor;
        
        GameObject clock = GameObject.Find($"Canvas/StoryChoice/{menu.name}/Clock");
        clock.GetComponent<Image>().color = textColor;
        GameObject.Find($"Canvas/StoryChoice/{menu.name}/Clock/TimeNumber").GetComponent<Text>().color = textColor;
        GameObject.Find($"Canvas/StoryChoice/{menu.name}/Clock/TimeNumber").GetComponent<Text>().text = TimeSpan.FromMinutes(playtime).ToString(@"h\hmm\m");
        GameObject.Find($"Canvas/StoryChoice/{menu.name}/NSFW").GetComponent<Text>().color = textColor;
        if (!isnsfw)
        {
            GameObject.Find($"Canvas/StoryChoice/{menu.name}/NSFW").GetComponent<Text>().color = new Color(0, 0, 0, 0);
            clock.transform.localPosition = new Vector3(clock.transform.localPosition.x, 47, 0);
        }

        GameObject.Find($"Canvas/StoryChoice/{menu.name}/Title").GetComponent<Text>().text = name;

        GameObject.Find($"Canvas/StoryChoice/{menu.name}/Flag").GetComponent<Image>().sprite = dl.Flag(language);
    }
    public void Play(int id)
    {
        Debug.Log($"Attempting to start story with ID {id}, path {shelper.GetAllStoryPaths(sortArgs, nsfw)[id]}");
        PlayerPrefs.SetString("story", shelper.GetAllStoryNames(sortArgs, nsfw)[id]);
        PlayerPrefs.SetString("script", "start");
        ButtonCtrl main = GameObject.Find("GameObject").GetComponent<ButtonCtrl>();
        main.CreateCharacters();
        StartCoroutine(main.FadeToScene("game"));


    }
    public void OpenDetails(int id)
    {
        Debug.Log($"Attempting to create details of local story with ID {id}, path {shelper.GetAllStoryPaths(sortArgs, nsfw)[id]}");
    }
    public void ChangePage(int change)
    {
        if (shelper.GetCardPages(sortArgs, nsfw) == 0)
        {
            GameObject.Find("Canvas/StoryChoice/Pageinfo").GetComponent<Animator>().Play("storycard_pageinfojump", 0, 0);
            return;
        }

        ClearPreviewCards();
        if (page + change > shelper.GetCardPages(sortArgs, nsfw)) page = 0;
        else if (page + change < 0) page = shelper.GetCardPages(sortArgs, nsfw);
        else page += change;
        UpdatePreviewCards();
    }
    private void ResetPage()
    {
        ClearPreviewCards();
        page = 0;
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
