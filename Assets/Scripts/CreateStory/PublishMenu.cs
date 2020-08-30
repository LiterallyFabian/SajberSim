using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PublishMenu : MonoBehaviour
{
    public InputField P_Title;
    public InputField P_Description;
    public RawImage P_Thumbnail;
    public Dropdown P_Privacy;
    private Download dl;
    public GameObject P_NoThumbnailText;
    public Button P_Publish;
    public InputField P_Notes;
    public PublishNovel NovelPublisher;
    public GameObject P_NameNotice;
    public Texture P_DefaultThumbnail;
    private void Start()
    {
        dl = Download.Init();
        
    }
    public void FillData()
    {
        Manifest data = Manifest.Get(Path.Combine(CreateStory.editPath, "manifest.json"));
        P_Title.text = CreateStory.editName;
        P_Description.text = data.description;
        if (File.Exists(Path.Combine(CreateStory.editPath, "steam.png")))
        {
            dl.CardThumbnail(P_Thumbnail, Path.Combine(CreateStory.editPath, "steam.png"));
            P_Publish.interactable = true;
            P_NoThumbnailText.SetActive(false);
        }
        else
        {
            P_Publish.interactable = false;
            P_NoThumbnailText.SetActive(true);
            P_Thumbnail.texture = P_DefaultThumbnail;
        }
        if (data.id == "-1")
        {
            P_NameNotice.SetActive(false);
            P_Notes.text = Translate.Get("changenotedefault");
            P_Description.interactable = true;
            P_Title.interactable = true;
        }
        else
        {
            P_NameNotice.SetActive(true);
            P_Notes.text = "";
            P_Description.interactable = false;
            P_Title.interactable = false;
        }
        P_Privacy.ClearOptions();
        P_Privacy.AddOptions(Helper.privacysettings.ToList());
    }
    public void TryPublish()
    {
        P_Publish.interactable = false;
        NovelPublisher.TryPublish(CreateStory.editPath);
    }
    public void OpenURLPage()
    {
        Manifest data = Manifest.Get(Path.Combine(CreateStory.editPath, "manifest.json"));
        System.Diagnostics.Process.Start($"steam://openurl/https://steamcommunity.com/sharedfiles/itemedittext/?id={data.id}");
    }
}