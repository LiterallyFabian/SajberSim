using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using SajberSim.Web;
using SajberSim.Translation;
using SajberSim.CardMenu;
using SajberSim.Helper;
using System;
using System.Globalization;
using SajberSim.Colors;

public class DetailsCard : MonoBehaviour
{
    public RawImage Background;
    public Image Flag;
    public Text Title;
    public Text Author;
    public Text Description;
    public Text Tags;
    public Text Genre;
    public Text Length;
    public Text NsfwStatus;
    public Button Edit;
    public Download dl;
    public StoryCard card;

    void Start()
    {
        
    }
    public void UpdateDetails(Manifest data, RawImage back)
    {
        Edit.interactable = card.myNovel;
        Background.texture = back.texture;
        Title.text = data.name;
        Author.text = $"{string.Format(Translate.Get("publishedby"), $"<b>{data.author}</b>")} {Helper.TimeAgo(data.uploaddate)}";
        Description.GetComponent<Text>().text = data.description;
        Tags.text = string.Join(", ", data.tags);
        NsfwStatus.text = data.nsfw ? Translate.Get("yes") : Translate.Get("no");
        NsfwStatus.color = data.nsfw ? Colors.NsfwRed : Colors.UnityGray;
        Genre.text = Translate.Get(data.genre);
        Length.text = card.stats.wordsK;
        Flag.sprite = dl.Flag(data.language);
        StartStory.detailsOpen = true;
    }
    public void Play()
    {
        card.Play();
    }
    public void GoBack()
    {
        StartStory.detailsOpen = false;
        GetComponent<Animator>().Play("detailsClose");
        Destroy(gameObject, 2);
    }
    public void EditCard()
    {
        GoBack();
        card.Edit();
    }
}
