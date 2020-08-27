using SajberSim.Helper;
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
    public PublishNovel NovelPublisher;
    private void Start()
    {
        dl = Download.Init();
        
    }
    public void FillData()
    {
        P_Title.text = CreateStory.currentlyEditingName;
        P_Description.text = Manifest.Get($"{CreateStory.currentlyEditingPath}/manifest.json").description;
        if (File.Exists($"{CreateStory.currentlyEditingPath}/steam.png"))
        {
            dl.CardThumbnail(P_Thumbnail, $"{CreateStory.currentlyEditingPath}/steam.png");
            P_Publish.interactable = true;
            P_NoThumbnailText.SetActive(false);
        }
        else
        {
            P_Publish.interactable = false;
            P_NoThumbnailText.SetActive(true);
        }
        
        P_Privacy.ClearOptions();
        P_Privacy.AddOptions(Helper.privacysettings.ToList());
    }
    public void TryPublish()
    {
        P_Publish.interactable = false;
        NovelPublisher.TryPublish(CreateStory.currentlyEditingPath);
    }
}