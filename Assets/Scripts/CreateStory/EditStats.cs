using Newtonsoft.Json;
using SajberSim.CardMenu;
using SajberSim.Colors;
using SajberSim.Helper;
using SajberSim.Translation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditStats : MonoBehaviour
{
    public CreateStory Main;
    public Text E_Stats;
    public ColorPicker E_ColorPickerText;
    public ColorPicker E_ColorPickerSplash;
    public GameObject fadeimage;
    private GameObject Card;
    private StoryCard CardComp;

    public void UpdateStats()
    {
        Manifest data = Manifest.Get(CreateStory.currentlyEditingPath + "/manifest.json");
        Card = Main.storyMenu.CreateCard(CreateStory.currentlyEditingPath, data, new Vector3(680.6f, -45.3f, 0), "Canvas/CreateMenu/EditMenu");
        Card.transform.localScale = new Vector3(1.06f, 1.06f, 1.06f);
        Card.name = "Preview card";
        Card.tag = "Untagged";
        CardComp = Card.GetComponent<StoryCard>();

        E_ColorPickerText.AssignColor(Colors.FromRGB(data.textcolor));
        E_ColorPickerSplash.AssignColor(Colors.FromRGB(data.overlaycolor));

        bool hasstart = false;
        bool hasthumbnail = false;
        bool hassteam = false;

        
        try
        {
            StoryStats stats = StoryStats.Get(CreateStory.currentlyEditingPath);
            E_Stats.text =
                //story
                $"{string.Format(Translate.Get("totalscripts"), stats.scripts)}\n" +
                $"{string.Format(Translate.Get("totallines"), stats.lines)}\n" +
                $"{string.Format(Translate.Get("totalaudio"), stats.audioclips)}\n" +
                $"{string.Format(Translate.Get("totalbackgrounds"), stats.backgrounds)}\n" +
                $"{string.Format(Translate.Get("totalcharacters"), stats.charactersprites)}\n" +
                $"{string.Format(Translate.Get("totaldialogues"), stats.textboxes)}\n" +
                $"{string.Format(Translate.Get("totalalerts"), stats.alerts)}\n" +
                $"{string.Format(Translate.Get("totalwords"), stats.words)}\n" +
                $"{string.Format(Translate.Get("totalbgchanges"), stats.backgroundchanges)}\n" +
                $"{string.Format(Translate.Get("totaldecisions"), stats.decisions)}\n\n" +

                //other
                $"{string.Format(Translate.Get("hascredits"), stats.hascredits ? Translate.Get("yes") : Translate.Get("no"))}\n" +
                $"{string.Format(Translate.Get("totalparticipants"), stats.participants)}\n" +
                $"{string.Format(Translate.Get("filesize"), stats.filesize)}\n" +
                $"";
        }
        catch (Exception e)
        {
            E_Stats.text = $"Something went wrong when trying to show stats: \n{e}";
            Debug.LogError($"Something went wrong when trying to show stats: \n{e}");
        }
    }
    public void UpdateTextColor(Color c)
    {
        if (CardComp == null) return;
        CardComp.Title.color = c;
        CardComp.Clock.color = c;
        CardComp.Playtime.color = c;
    }
    public void UpdateSplashColor(Color c)
    {
        if (CardComp == null) return;
        CardComp.Overlay.color = c;
    }
    public void SaveColors()
    {
        if (Main.currentWindow != CreateStory.CreateWindows.Edit) return;
        string manifestPath = CreateStory.currentlyEditingPath + "/manifest.json";
        Manifest data = Manifest.Get(manifestPath);

        data.textcolor = ColorUtility.ToHtmlStringRGB(CardComp.Title.color);
        data.overlaycolor = ColorUtility.ToHtmlStringRGB(CardComp.Overlay.color);

        JsonSerializer serializer = new JsonSerializer();
        using (StreamWriter sw = new StreamWriter(manifestPath))
        using (JsonWriter writer = new JsonTextWriter(sw))
        {
            serializer.Serialize(writer, data);
        }
    }
    public void PlayNovel()
    {
        if (CardComp == null) return;
        CardComp.Play();
    }
    public void PlayCredits()
    {
        ButtonCtrl main = GameObject.Find("ButtonCtrl").GetComponent<ButtonCtrl>();
        Credits.storypath = CreateStory.currentlyEditingPath;
        StartCoroutine(main.FadeToScene("credits"));
    }
}

