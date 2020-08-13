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
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateStory : MonoBehaviour
{
    public static string currentlyEditingPath = "";
    public static string currentlyEditingName = "";

    public CreateNew Menu_Create;
    public EditStats Menu_Edit;

    

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
        Menu_Edit.SaveColors();
        switch (window)
        {
            case 0: // Create story and set basics
                currentWindow = CreateWindows.Basics;
                Menu_Create.ResetFields();
                Menu_Create.ButtonSave.interactable = false;
                Menu_Create.ButtonRevert.interactable = false;
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
                Description.text = string.Format(Translate.Get("detailsdescription"), currentlyEditingName);
                Menu_Create.SetDetails();
                break;
            case 2: // Edit story 
                currentWindow = CreateWindows.Edit;
                EditsMenu.transform.localScale = Vector3.one;
                Title.text = Translate.Get("editstats");
                Description.text = string.Format(Translate.Get("editsdescription"), currentlyEditingName);
                ButtonEdit.interactable = false;
                Menu_Edit.UpdateStats();
                break;
        }
    }

    public void PlayCredits()
    {
        Credits.storypath = currentlyEditingPath;
        SceneManager.LoadScene("credits");
    }
}
