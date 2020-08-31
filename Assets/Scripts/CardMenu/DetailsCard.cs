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
    public Manifest data;

    void Start()
    {
        
    }
    public void UpdateDetails(RawImage back)
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
        try
        {
            Flag.sprite = dl.Flag(Language.Languages[data.language].iso_code);
        }
        catch { }
        
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
    public void SendDeleteRequest()
    {
        GameObject box = Instantiate(Resources.Load($"Prefabs/DeleteBox", typeof(GameObject)), Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas").GetComponent<Transform>()) as GameObject;
        box.transform.localPosition = Vector3.zero;
        DeleteNovel dn = box.GetComponent<DeleteNovel>();
        dn.path = card.storyPath;
        dn.data = data;
    }
}
