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
using SajberSim.Colors;
using SajberSim.CardMenu;

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
    public Download dl;

    private int page = 0; //current page in story card menu, starting at 0
    public bool nsfw;
    public static bool detailsOpen = false;
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
        dl = Download.Init();

        string[] sorting = { Translate.Get("byname"), Translate.Get("bynamedec"), Translate.Get("bylongest"), Translate.Get("byshortest"), Translate.Get("bynewest"), Translate.Get("byoldest"), Translate.Get("byauthor"), Translate.Get("bymodified") };
        sortArgs = (Helper.StorySearchArgs)PlayerPrefs.GetInt("sorting", 0);
        sortWay.AddOptions(sorting.ToList());
        sortWay.SetValueWithoutNotify(PlayerPrefs.GetInt("sorting", 0));

        UpdatePreviewCards();
    }
    public void SetSearchPath(int n)
    {
        switch(n)
        {
            case 0: searchPath = Helper.StorySearchPaths.All; break;
            case 1: searchPath = Helper.StorySearchPaths.Own; break;
        }
    }
    /// <summary>
    /// Change the toggle & value if own novels only should be shown
    /// </summary>
    public void SetOwnToggle(bool n)
    {
        GameObject.Find("Canvas/StoryChoice/MyNovelsToggle").GetComponent<Toggle>().SetIsOnWithoutNotify(n);
        searchPath = n ? Helper.StorySearchPaths.Own : Helper.StorySearchPaths.All;
        Debug.Log(n ? "Card menu changed to only show own novels" : "Card menu changed to show all novels");
        if (page != 0) ResetPage();
        Stories.pathUpdateNeeded = true;
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
        Stories.pathUpdateNeeded = true;
        UpdatePreviewCards();
    }
    public void UserUpdateSort(int n)
    {
        sortArgs = (Helper.StorySearchArgs)n;
        Debug.Log($"Sorting arguments changed: {sortArgs}");
        PlayerPrefs.SetInt("sorting", n);
        Stories.pathUpdateNeeded = true;
        UpdatePreviewCards();
    }
    public void UserUpdateSearch(string search) //runs at lost focus or enter
    {
        Debug.Log($"Search term changed: {search}");
        searchTerm = search;
        if (page != 0) ResetPage();
        Stories.pathUpdateNeeded = true;
        UpdatePreviewCards();
    }
    public void UserEditSearch(string search) //runs every single letter
    {
        if (search == "")
        {
            Debug.Log($"Search term reset.");
            searchTerm = "";
            Stories.pathUpdateNeeded = true;
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
        Debug.Log("StoryMenu/Update: Request to update cards");
        string[] storyPaths = Stories.GetAllStoryPaths(sortArgs, nsfw, searchTerm, searchPath);
        string[] manifests = Manifest.GetAll(sortArgs, nsfw, searchTerm, searchPath);
        UpdateNoNovelNotice(storyPaths.Count());
        ClearPreviewCards();
        int cardPages = Stories.GetCardPages(sortArgs, nsfw, searchTerm, searchPath) + 1;
        for (int i = page * 6; i < page * 6 + 6; i++)
        {
            GameObject.Find("Canvas/StoryChoice/Pageinfo").GetComponent<Text>().text = string.Format(Translate.Get("page"), page+1, cardPages);
            if (manifests.Length == i) return; //cancel if story doesn't exist, else set all variables
            Manifest storydata = Manifest.Get(manifests[i]);
            if (storydata != null)
            {
                Vector3 position = Helper.CardPositions[Helper.CardPositions.Keys.ElementAt(i - (page * 6))];
                CreateCard(storyPaths[i], storydata, position);
            }
        }
        if (Stories.GetAllStoryPaths(Helper.StorySearchArgs.Alphabetical, true, "", Helper.StorySearchPaths.Workshop).Length > 0) Achievements.Grant(Achievements.List.ACHIEVEMENT_download);
    }
    private void UpdateNoNovelNotice(int novels)
    {
        if (novels == 0)
        {
            if(searchPath == Helper.StorySearchPaths.Own)
                GameObject.Find("Canvas/StoryChoice/NoNovelsNotice/Text").GetComponent<Text>().text = string.Format(Translate.Get("noownnovelsfound"), Translate.Get("ownnoveltoggle"));
            else if (searchPath == Helper.StorySearchPaths.All && searchTerm == "")
                GameObject.Find("Canvas/StoryChoice/NoNovelsNotice/Text").GetComponent<Text>().text = string.Format(Translate.Get("nonovelsfound"), Translate.Get("createnew"));
            else
                GameObject.Find("Canvas/StoryChoice/NoNovelsNotice/Text").GetComponent<Text>().text = Translate.Get("nonovelsfoundsearch");
            GameObject.Find("Canvas/StoryChoice/NoNovelsNotice").transform.localScale = Vector3.one;
        }
        else GameObject.Find("Canvas/StoryChoice/NoNovelsNotice").transform.localScale = Vector3.zero;
    }
    public GameObject CreateCard(string storyPath, Manifest data, Vector3 pos, string parent = "Canvas/StoryChoice")
    {
        if (GameObject.Find($"Canvas/StoryChoice/Card {data.name}")) Destroy(GameObject.Find($"Canvas/StoryChoice/Card {data.name}").gameObject);

        //spawn, place and resize
        GameObject menu = Instantiate(StoryCardTemplate, Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find(parent).GetComponent<Transform>()) as GameObject; 
        menu.transform.localPosition = pos;
        menu.name = $"Card {data.name}";

        //fill with data
        StoryCard cardDetails = menu.GetComponent<StoryCard>();
        cardDetails.SetData(data, storyPath);
        return menu;
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
    public void ChangePage(int change)
    {
        int pages = Stories.GetCardPages(sortArgs, nsfw, searchTerm, searchPath);
        if (pages == 0)
        {
            GameObject.Find("Canvas/StoryChoice/Pageinfo").GetComponent<Animator>().Play("storycard_pageinfojump", 0, 0);
            return;
        }

        ClearPreviewCards();
        if (page + change > pages) page = 0;
        else if (page + change < 0) page = pages;
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
        Debug.Log("StoryMenu/Clear: Request to delete preview cards");
        GameObject[] cards = GameObject.FindGameObjectsWithTag("PreviewCard");
        foreach (GameObject card in cards)
            Destroy(card);
    }
    public void OpenMenu(bool own = false)
    {
        if (own) SetOwnToggle(true);
        else SetOwnToggle(false);
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
