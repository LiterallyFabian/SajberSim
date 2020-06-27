using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Newtonsoft.Json;
using SajberSim.Helper;
using SajberSim.Web;
using SajberSim.Translation;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using SajberSim.Steam;

/// <summary>
/// Puts all downloaded thumbnails in the story menu
/// </summary>
public class StartStory : MonoBehaviour
{
    public GameObject StoryCardTemplate;
    public GameObject DetailsTemplate;
    public GameObject CloseButtonBehind;
    public Dropdown sortWay;

    
    Helper.StorySearchArgs sortArgs;
    Download dl;

    private int page = 0; //current page in story card menu, starting at 0
    public bool nsfw;
    public bool detailsOpen = false;
    private Helper.StorySearchPaths searchPath = Helper.StorySearchPaths.All;
    public static bool storymenuOpen = false;
    public static bool creatingStory = false;
    private string searchTerm = "";
    

        

    // Start is called before the first frame update
    void Start()
    {
        storymenuOpen = false;
        if (PlayerPrefs.GetInt("nsfw", 0) == 0) nsfw = false;
        else nsfw = true;
        GameObject.Find("Canvas/StoryChoice/NSFWtoggle").GetComponent<Toggle>().SetIsOnWithoutNotify(nsfw);
        dl = new GameObject("downloadobj").AddComponent<Download>();

        string[] sorting = { Translate.Fields["byname"], Translate.Fields["bynamedec"], Translate.Fields["bylongest"], Translate.Fields["byshortest"], Translate.Fields["bynewest"], Translate.Fields["byoldest"], Translate.Fields["byauthor"] };
        sortArgs = (Helper.StorySearchArgs)PlayerPrefs.GetInt("sorting", 0);
        sortWay.AddOptions(sorting.ToList());
        sortWay.SetValueWithoutNotify(PlayerPrefs.GetInt("sorting", 0));

        UpdatePreviewCards();
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
        PlayerPrefs.SetInt("sorting", n);
        UpdatePreviewCards();
    }
    public void UserUpdateSearch(string search) //runs at lost focus or enter
    {
        Debug.Log($"Search term changed: {search}");
        searchTerm = search;
        if (page != 0) ResetPage();
        UpdatePreviewCards();
    }
    public void UserEditSearch(string search) //runs every single letter
    {
        if (search == "")
        {
            Debug.Log($"Search term reset.");
            searchTerm = "";
            UpdatePreviewCards();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!detailsOpen && Input.GetKeyDown(KeyCode.Escape))
            CloseMenu();
        if (detailsOpen)
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetMouseButtonDown(1))
                DeleteDetails();
    }
    public void UpdatePreviewCards()
    {
        Debug.Log("Request to update cards");
        string[] storyPaths = Helper.GetAllStoryPaths(sortArgs, nsfw, searchTerm, searchPath);
        string[] manifests = Helper.GetAllManifests(sortArgs, nsfw, searchTerm, searchPath);
        ClearPreviewCards();
        for (int i = page * 6; i < page * 6 + 6; i++)
        {
            GameObject.Find("Canvas/StoryChoice/Pageinfo").GetComponent<Text>().text = $"{Translate.Fields["page"]} {page + 1}/{Helper.GetCardPages(sortArgs, nsfw, searchTerm, searchPath)+1}";
            if (manifests.Length == i) return; //cancel if story doesn't exist, else set all variables
            Manifest storydata = Helper.GetManifest(manifests[i]); 
            Vector3 position = Helper.CardPositions[Helper.CardPositions.Keys.ElementAt(i - (page * 6))];
            CreateCard(storyPaths[i], storydata, position, i);
        }
    }
    private void CreateCard(string storyPath, Manifest data, Vector3 pos, int no)
    {
        string name = data.name;
        string id = storyPath.Replace($"{UnityEngine.Application.dataPath}/Story/", "");
        string language = data.language.ToUpper();
        string overlaycolor = data.overlaycolor.Replace("#", "");
        string textcolor = data.textcolor.Replace("#", "");
        bool isnsfw = data.nsfw;
        int playtime = data.playtime;
        if (GameObject.Find($"Canvas/StoryChoice/{id}")) Destroy(GameObject.Find($"Canvas/StoryChoice/{id}").gameObject);
        //spawn, place and resize
        GameObject menu = Instantiate(StoryCardTemplate, Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas/StoryChoice").GetComponent<Transform>()) as GameObject; 
        
        menu.transform.localPosition = pos;
        //menu.transform.localScale = Vector3.one;
        menu.name = $"Card {no}";



        //Fill with data
        if (File.Exists($"{storyPath}/thumbnail.png"))
            dl.CardThumbnail(menu.transform.Find($"Thumbnail"), $"{storyPath}/thumbnail.png");
        else
            menu.transform.Find("Thumbnail").GetComponent<Image>().color = Color.white;

        Color splashColor = Color.white;
        ColorUtility.TryParseHtmlString($"#{overlaycolor}", out splashColor);
        menu.transform.Find("Overlay").GetComponent<Image>().color = splashColor;

        Color textColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 1); //standard gray
        ColorUtility.TryParseHtmlString($"#{textcolor}", out textColor);
        menu.transform.Find("Title").GetComponent<Text>().color = textColor;
        
        Transform clock = menu.transform.Find("Clock");
        clock.GetComponent<Image>().color = textColor;
        clock.transform.Find("TimeNumber").GetComponent<Text>().color = textColor;
        clock.transform.Find("TimeNumber").GetComponent<Text>().text = TimeSpan.FromMinutes(playtime).ToString(@"h\hmm\m");
        
        if (!isnsfw)
        {
            menu.transform.Find("NSFW").GetComponent<Text>().color = new Color(0, 0, 0, 0); //hide
            clock.transform.localPosition = new Vector3(clock.transform.localPosition.x, 47, 0);
        }
        else if (isnsfw) // easier to read than just an else 
        {
            menu.transform.Find("NSFW").GetComponent<Text>().color = textColor; //show
            clock.transform.localPosition = new Vector3(clock.transform.localPosition.x, 57, 0);
        }

        menu.transform.Find("Title").GetComponent<Text>().text = name;

        menu.transform.Find("Flag").GetComponent<Image>().sprite = dl.Flag(language);
    }
    private void CreateDetails(int n)
    {
        string folderPath = Helper.GetAllStoryPaths(sortArgs, nsfw, searchTerm, searchPath)[n];
        Manifest data = Helper.GetManifest($"{folderPath}/manifest.json");

        string name = data.name;
        string id = folderPath.Replace($"{UnityEngine.Application.dataPath}/Story/", "");
        string language = data.language.ToUpper();
        bool isnsfw = data.nsfw;
        int playtime = data.playtime;
        string[] tags = data.tags;
        string genre = data.genre;
        string author = data.author;
        string description = data.description;
        int publishdate = data.publishdate;

        GameObject details = Instantiate(DetailsTemplate, Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas/StoryChoice").GetComponent<Transform>()) as GameObject;
        Vector3 startPos = Helper.CardPositions[Helper.CardPositions.Keys.ElementAt(n - (page * 6))];
        details.transform.localPosition = new Vector3(0, startPos.y, 1);
        details.name = $"Detailscard {n}";

        if (File.Exists($"{folderPath}/thumbnail.png"))
            dl.CardThumbnail(details.transform.Find($"Thumbnail"), $"{folderPath}/thumbnail.png");
        else
            details.transform.Find("Thumbnail").GetComponent<Image>().color = Color.white;

        details.transform.Find("Title").GetComponent<Text>().text = name;
        details.transform.Find("Author").GetComponent<Text>().text = $"{string.Format(Translate.Get("publishedby"), $"<b>{author}</b>")} {Helper.TimeAgo(DateTime.ParseExact(publishdate.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture))}";
        details.transform.Find("Description").GetComponent<Text>().text = description;
        details.transform.Find("TagsTitle/Tags").GetComponent<Text>().text = string.Join(", ", tags);
        details.transform.Find("NsfwTitle/nsfw").GetComponent<Text>().text = isnsfw ? Translate.Get("yes") : Translate.Get("no");
        details.transform.Find("NsfwTitle/nsfw").GetComponent<Text>().color = isnsfw ? new Color(0.5f, 0, 0) : new Color(0.1960784f, 0.1960784f, 0.1960784f, 1);
        details.transform.Find("GenreTitle/Genre").GetComponent<Text>().text = Translate.Get(genre);
        details.transform.Find("LengthTitle/Length").GetComponent<Text>().text = TimeSpan.FromMinutes(playtime).ToString(@"h\hmm\m");
        details.transform.Find("Flag").GetComponent<Image>().sprite = dl.Flag(language);
        detailsOpen = true;
    }
    public void DeleteDetails()
    {
        GameObject card = GameObject.FindGameObjectWithTag("DetailsCard");
        if (!card) return;
        card.GetComponent<Animator>().Play("detailsClose");
        StartCoroutine(DelCard(card));
        detailsOpen = false;
    }
    public static IEnumerator DelCard(GameObject card)
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(card);
    }

    public void Play(int id)
    {
        string story = Helper.GetAllStoryNames(sortArgs, nsfw, searchTerm, searchPath)[id];
        string path = Helper.GetAllStoryPaths(sortArgs, nsfw, searchTerm, searchPath)[id];
        Manifest data = Helper.GetManifest($"{path}/manifest.json");
        Helper.currentStoryPath = path;
        Debug.Log($"Attempting to start story with ID {id}, path {path}");
        PlayerPrefs.SetString("story", story);
        PlayerPrefs.SetString("script", "start");
        ButtonCtrl main = GameObject.Find("ButtonCtrl").GetComponent<ButtonCtrl>();
        main.CreateCharacters();
        storymenuOpen = false;
        GameManager.storyAuthor = data.author;
        GameManager.storyName = data.name;
        Achievements.Grant(Achievements.List.ACHIEVEMENT_play1);
        Stats.Add(Stats.List.novelsstarted);
        StartCoroutine(main.FadeToScene("game"));
    }
    public void OpenDetails(int id)
    {
        string path = Helper.GetAllStoryPaths(sortArgs, nsfw, searchTerm, searchPath)[id];
        Debug.Log($"Attempting to create details of local story with ID {id}, path {path}");
        CreateDetails(id);
    }
    public void ChangePage(int change)
    {
        if (Helper.GetCardPages(sortArgs, nsfw, searchTerm, searchPath) == 0)
        {
            GameObject.Find("Canvas/StoryChoice/Pageinfo").GetComponent<Animator>().Play("storycard_pageinfojump", 0, 0);
            return;
        }

        ClearPreviewCards();
        if (page + change > Helper.GetCardPages(sortArgs, nsfw, searchTerm, searchPath)) page = 0;
        else if (page + change < 0) page = Helper.GetCardPages(sortArgs, nsfw, searchTerm);
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
    public void OpenMenu(bool both = true)
    {
        if (!both)
            UpdatePreviewCards();
        GameObject.Find("Canvas/StoryChoice").GetComponent<Animator>().Play("openStorymenu");
        CloseButtonBehind.SetActive(true);
        storymenuOpen = true;
    }
    public void CloseMenu()
    {
        GameObject.Find("Canvas/StoryChoice").GetComponent<Animator>().Play("closeStorymenu");
        CloseButtonBehind.SetActive(false);
        DeleteDetails();
        storymenuOpen = false;
    }
    
    
}
