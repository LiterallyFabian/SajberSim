using Newtonsoft.Json;
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

/// <summary>
/// Used to create a NEW story or change details
/// </summary>
public class CreateNew : MonoBehaviour
{
    public CreateStory Main;
    public InputField B_inputName;
    public InputField B_inputDescription;
    public InputField B_inputTags;
    public Dropdown B_inputGenre;
    public Dropdown B_inputAudience;
    public Dropdown B_inputLanguage;
    public Toggle B_inputCustomName;
    public Text B_Status;
    public Text B_TagsTitle;
    public Text B_AudienceTitle;
    private Download dl;
    public Button ButtonSave;
    public Button ButtonRevert;
    public bool hasEdited;

    // Start is called before the first frame update
    void Start()
    {
        dl = Download.Init();

        B_inputName.placeholder.GetComponent<Text>().text = string.Format(Translate.Get("defaultnameuser"), Helper.UsernameCache());
        SetNSFW();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Type(bool hasTyped)
    {
        hasEdited = hasTyped;
        Main.ButtonQuit.interactable = !hasTyped;
        Main.ButtonEdit.interactable = !hasTyped;
        Main.ButtonVerify.interactable = !hasTyped;
        Main.ButtonPublish.interactable = !hasTyped;
        ButtonRevert.interactable = hasTyped;
        ButtonSave.interactable = hasTyped;
    }
    public void UpdateTags(string input)
    {
        if(input == "") B_TagsTitle.text = string.Format(Translate.Get("xtags"), 0);
        else B_TagsTitle.text = string.Format(Translate.Get("xtags"), input.Split(',').Length);
    }
    public void SetNSFW()
    {
        B_AudienceTitle.text = B_inputAudience.value == 0 ? Translate.Get("audiencesfw") : Translate.Get("audiencensfw");
    }
    public void SetDropDowns()
    {
        if(dl == null) dl = Download.Init();
        B_inputGenre.AddOptions(Helper.genres.ToList());
        B_inputAudience.AddOptions(Helper.audience.ToList());

        List<Dropdown.OptionData> dropdownList = new List<Dropdown.OptionData>();
        foreach (Language lang in Language.Languages.Values)
            dropdownList.Add(new Dropdown.OptionData(lang.formal_name, dl.Flag(lang.iso_code)));
            
        B_inputLanguage.AddOptions(dropdownList);
    }
    /// <summary>
    /// Create a basics menu for when a novel already exists and set the data
    /// </summary>
    public void SetDetails()
    {
        Manifest data = Manifest.Get(CreateStory.currentlyEditingPath + "/manifest.json");
        try
        {
            B_inputName.SetTextWithoutNotify(data.name);
            B_inputDescription.SetTextWithoutNotify(data.description);
            B_inputTags.SetTextWithoutNotify(string.Join(", ", data.tags));
            B_inputAudience.SetValueWithoutNotify(Array.IndexOf(Helper.audience, data.rating));
            B_inputGenre.SetValueWithoutNotify(Array.IndexOf(Helper.genresid, data.genre));
            B_inputCustomName.SetIsOnWithoutNotify(data.customname);
            int langIndex = Array.IndexOf(Language.ListFlagCode().ToArray(), data.language);
            if (langIndex == -1)
                B_inputLanguage.SetValueWithoutNotify(0);
            else
                B_inputLanguage.SetValueWithoutNotify(langIndex);
            UpdateTags(B_inputTags.text);
            SetNSFW();
        }
        catch (Exception e)
        {
            Helper.Alert(string.Format(Translate.Get("errordetails"), Translate.Get("helpcontact"), e));
            Debug.LogError($"Something went wrong when trying to set details for story {CreateStory.currentlyEditingPath}: \n{e}");
        }
        Type(false);
    }
    /// <summary>
    /// UPDATE all data in manifest with details from details fields
    /// </summary>
    public void SaveDetails()
    {
        try
        {
            string path = CreateStory.currentlyEditingPath + "/manifest.json";
            Manifest data = Manifest.Get(path);
            data.name = B_inputName.text;
            data.description = B_inputDescription.text;
            data.tags = B_inputTags.text.Replace(", ", ",").Replace(" ,", ",").Split(',');
            data.genre = Helper.genresid[B_inputGenre.value];
            data.rating = Helper.audience[B_inputAudience.value];
            data.language = Language.ListFlagCode()[B_inputLanguage.value];
            data.customname = B_inputCustomName.isOn;

            if (data.rating == "Questionable" || data.rating == "Mature") data.nsfw = true;
            else data.nsfw = false;

            JsonSerializer serializer = new JsonSerializer();
            using (StreamWriter sw = new StreamWriter(path))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, data);
            }
            StartCoroutine(B_SetStatus(Translate.Get("saved")));
        }
        catch (Exception e)
        {
            Helper.Alert(string.Format(Translate.Get("errorsavingdetails"), Translate.Get("helpcontact"), e));
        }
        Type(false);
    }
    public void ResetFields()
    {
        B_inputGenre.SetValueWithoutNotify(0);
        B_inputAudience.SetValueWithoutNotify(0);
        B_inputLanguage.SetValueWithoutNotify(0);
        B_inputName.text = "";
        B_inputDescription.text = "";
        B_inputTags.text = "";
        Type(false);
    }
    IEnumerator B_SetStatus(string text)
    {
        B_Status.text = text;
        yield return new WaitForSeconds(3);
        B_Status.text = "";
    }
}
