using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SajberSim.CardMenu;
using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateStory : MonoBehaviour
{
    public static string currentlyEditingPath = "";
    public static string currentlyEditingName = "";

    public CreateNew Menu_Create;

    public Text E_StatsTitle;
    public Text E_Stats;
    public GameObject E_Thumbnail;

    public GameObject ButtonGroup;
    public GameObject BasicsMenu;
    public GameObject EditsMenu;
    public Text Title;
    public Text Description;

    public Button ButtonDetails;
    public Button ButtonEdit;
    public Button ButtonVerify;
    public Button ButtonPublish;
    public Button ButtonQuit;
    public Button ButtonCreate;

    private Language lang;
    Download dl;
    public StartStory storyMenu;
    public CreateWindows currentWindow;
    public enum CreateWindows
    {
        Basics,
        Details,
        Edit,
        Debug,
        Publish
    }

    void Start()
    {
        dl = Download.Init();
        storyMenu = GameObject.Find("Canvas/StoryChoice").GetComponent<StartStory>();
        transform.localScale = Vector3.zero;
        transform.localPosition = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ToggleMenu(bool open)
    {
        transform.localScale = open ? Vector3.one : Vector3.zero;
        storyMenu.UpdatePreviewCards();
    }
    /// <summary>
    /// Toggles between all windows in the create menu
    /// </summary>
    public void SetWindow(int window)
    {
        ButtonDetails.interactable = true;
        ButtonEdit.interactable = true;
        ButtonVerify.interactable = true;
        ButtonPublish.interactable = true;
        ButtonPublish.gameObject.SetActive(true);
        ButtonCreate.gameObject.SetActive(false);
        BasicsMenu.transform.localScale = Vector3.zero;
        EditsMenu.transform.localScale = Vector3.zero;
        Menu_Create.ResetFields();
        switch (window)
        {
            case 0: // Create story and set basics
                currentWindow = CreateWindows.Basics;
                Menu_Create.ResetFields();
                ButtonDetails.interactable = false;
                ButtonEdit.interactable = false;
                ButtonVerify.interactable = false;
                ButtonPublish.gameObject.SetActive(false);
                ButtonCreate.gameObject.SetActive(true);
                BasicsMenu.transform.localScale = Vector3.one;
                Title.text = Translate.Get("createnewnovel");
                Description.text = Translate.Get("createnewdesc");
                currentlyEditingPath = "NEW";
                currentlyEditingName = "New novel";
                break;
            case 1: // Story created, fill fields with predefined info
                currentWindow = CreateWindows.Details;
                ButtonDetails.interactable = false;
                BasicsMenu.transform.localScale = Vector3.one;
                Title.text = Translate.Get("details");
                Description.text = Translate.Get("detailsdescription");
                Menu_Create.SetDetails();
                break;
            case 2: // Edit story 
                currentWindow = CreateWindows.Edit;
                EditsMenu.transform.localScale = Vector3.one;
                Title.text = Translate.Get("edit");
                Description.text = Translate.Get("editsdescription");
                ButtonEdit.interactable = false;
                SetEdits();
                break;
        }
    }

    /// <summary>
    /// Create an edit menu and set the data
    /// </summary>
    private void SetEdits()
    {
        UpdateStats();
    }
    public void PlayCredits()
    {
        Credits.storypath = currentlyEditingPath;
        SceneManager.LoadScene("credits");
    }
    public void UpdateStats()
    {
        GameObject card = storyMenu.CreateCard(currentlyEditingPath, Manifest.Get(currentlyEditingPath + "/manifest.json"), new Vector3(680.6f, -45.3f, 0), "Canvas/CreateMenu/EditMenu");
        card.transform.localScale = new Vector3(1.06f, 1.06f, 1.06f);
        card.name = "Preview card";
        card.tag = "Untagged";
        int dialogues = 0;
        int alerts = 0;
        int words = 0;
        int backgroundchanges = 0;
        int decisions = 0;
        int participants = 0;
        string filesize = $"{Math.Round(Helper.BytesTo(Helper.DirSize(new DirectoryInfo(currentlyEditingPath)), Helper.DataSize.Megabyte)), 1} Mb";
        bool hasstart = false;
        bool hascredits = false;
        bool hasthumbnail = false;
        bool hassteam = false;

        E_StatsTitle.text = string.Format(Translate.Get("statsabout"), currentlyEditingName);
        string[] scriptPaths = Stories.GetStoryAssetPaths("dialogues", currentlyEditingPath);
        List<string> scriptLines = new List<string>();
        foreach(string path in scriptPaths)
        {
            if (File.Exists(path))
            {
                scriptLines.AddRange(File.ReadAllLines(path));
            }
        }
        int scripts = scriptPaths.Length;
        int lines = scriptLines.Count();
        int audio = Stories.GetStoryAssetPaths("audio", currentlyEditingPath).Length;
        int backgrounds = Stories.GetStoryAssetPaths("backgrounds", currentlyEditingPath).Length;
        int characters = Stories.GetStoryAssetPaths("characters", currentlyEditingPath).Length;
        try
        {
            int i = 0;
            foreach (string line in scriptLines)
            {
                string action = line.Split('|')[0];
                switch (action)
                {
                    case "T":
                        dialogues++;
                        if (line.Split('|').Length == 3)
                            words += line.Split('|')[2].Count(f => f == ' ') + 1;
                        break;
                    case "BG": backgroundchanges++; break;
                    case "ALERT": alerts++; break;
                    case "QUESTION": 
                        decisions++;
                        if (line.Split('|').Length > 1)
                            words += line.Split('|')[1].Count(f => f == ' ') + 1;
                        break;
                }
                i++;
            }
            if(File.Exists(currentlyEditingPath + "/credits.txt"))
            {
                foreach(string line in File.ReadAllLines(currentlyEditingPath + "/credits.txt"))
                {
                    if (!line.Contains('|') && !line.StartsWith("-") && line != "") participants++;
                }
            }
            E_Stats.text =
                $"{string.Format(Translate.Get("totalscripts"), scripts)}\n" +
                $"{string.Format(Translate.Get("totallines"), lines)}\n" +
                $"{string.Format(Translate.Get("totalaudio"), audio)}\n" +
                $"{string.Format(Translate.Get("totalbackgrounds"), backgrounds)}\n" +
                $"{string.Format(Translate.Get("totalcharacters"), characters)}\n" +
                $"{string.Format(Translate.Get("totaldialogues"), dialogues)}\n" +
                $"{string.Format(Translate.Get("totalalerts"), alerts)}\n" +
                $"{string.Format(Translate.Get("totalwords"), words)}\n" +
                $"{string.Format(Translate.Get("totalbgchanges"), backgroundchanges)}\n" +
                $"{string.Format(Translate.Get("totaldecisions"), decisions)}\n\n" +
                $"{string.Format(Translate.Get("totalparticipants"), participants)}\n" +
                $"{string.Format(Translate.Get("filesize"), filesize)}\n" +
                $"{string.Format(Translate.Get("totaldecisions"), decisions)}\n" +
                $"{string.Format(Translate.Get("totaldecisions"), decisions)}\n" +
                $"";
        }
        catch(Exception e)
        {
            E_Stats.text = $"Something went wrong when trying to show stats: \n{e}";
            Debug.LogError($"Something went wrong when trying to show stats: \n{e}");
        }
    }


}
