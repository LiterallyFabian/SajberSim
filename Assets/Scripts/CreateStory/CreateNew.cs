using SajberSim.CardMenu;
using SajberSim.Helper;
using SajberSim.Steam;
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
    private static readonly char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

    // Start is called before the first frame update
    private void Start()
    {
        dl = Download.Init();

        B_inputName.placeholder.GetComponent<Text>().text = string.Format(Translate.Get("defaultnameuser"), Helper.UsernameCache());
        SetNSFW();
    }

    public void Type(bool hasTyped)
    {
        hasEdited = hasTyped;
        if (Main.currentWindow != CreateStory.CreateWindows.Basics)
        {
            Main.ButtonQuit.interactable = !hasTyped;
            Main.ButtonEdit.interactable = !hasTyped;
            Main.ButtonVerify.interactable = !hasTyped;
            if (Helper.loggedin) Main.ButtonPublish.interactable = !hasTyped;
            ButtonRevert.interactable = hasTyped;
            ButtonSave.interactable = hasTyped;
        }
        else
        {
            Main.ButtonCreate.interactable = B_inputName.text != "";
        }
    }

    public void UpdateTags(string input)
    {
        if (input == "") B_TagsTitle.text = string.Format(Translate.Get("xtags"), 0);
        else B_TagsTitle.text = string.Format(Translate.Get("xtags"), input.Split(',').Length);
    }

    public void SetNSFW()
    {
        B_AudienceTitle.text = B_inputAudience.value == 0 ? Translate.Get("audiencesfw") : Translate.Get("audiencensfw");
    }

    public void SetDropDowns()
    {
        if (dl == null) dl = Download.Init();
        B_inputGenre.AddOptions(Helper.genres.ToList());
        B_inputAudience.AddOptions(Helper.audience.ToList());

        List<Dropdown.OptionData> dropdownList = new List<Dropdown.OptionData>();
        foreach (Language lang in Language.Languages.Values)
            dropdownList.Add(new Dropdown.OptionData(lang.localized_name, dl.Flag(lang.iso_code)));

        B_inputLanguage.AddOptions(dropdownList);
    }

    /// <summary>
    /// Create a basics menu for when a novel already exists and set the data
    /// </summary>
    public void SetDetails()
    {
        Manifest data = Manifest.Get(Path.Combine(CreateStory.editPath, "manifest.json"));
        try
        {
            B_inputName.SetTextWithoutNotify(data.name);
            B_inputDescription.SetTextWithoutNotify(data.description);
            B_inputTags.SetTextWithoutNotify(string.Join(", ", data.tags));
            B_inputAudience.SetValueWithoutNotify(Array.IndexOf(Helper.audience, data.rating));
            B_inputGenre.SetValueWithoutNotify(Array.IndexOf(Helper.genresid, data.genre));
            B_inputCustomName.SetIsOnWithoutNotify(data.customname);
            int langIndex = Array.IndexOf(Language.ListWindowsKeys().ToArray(), data.language);
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
            Debug.LogError($"Something went wrong when trying to set details for story {CreateStory.editPath}: \n{e}");
        }
        Type(false);
    }

    /// <summary>
    /// UPDATE or SAVE all data in manifest with details from details fields
    /// </summary>
    public void SaveDetails()
    {
        bool isNew = false;
        try
        {
            string path = "";
            if (CreateStory.editPath == "NEW") //CREATE NEW
            {
                string fixedName = new string(B_inputName.text.Select(ch => invalidFileNameChars.Contains(ch) ? '_' : ch).ToArray());
                string destPath = Path.Combine(Helper.customPath, fixedName);
                if (Directory.Exists(destPath)) destPath += "_" + UnityEngine.Random.Range(1000, 9999);

                //NO template
                if (!Directory.Exists(Helper.templatePath))
                {
                    Helper.Alert($"Visual novel template could not be found, it have most likely been removed. Your new novel will be saved without a template.\n\nTo reset the template, enter code \"TEMPLATE\" in Fabinas vault.");
                    Directory.CreateDirectory(destPath);
                }
                //template
                else
                {
                    Helper.CopyDirectory(Helper.templatePath, destPath);
                }

                CreateStory.editPath = destPath;
                CreateStory.editName = B_inputName.text;
                isNew = true;
                Achievements.Grant(Achievements.List.ACHIEVEMENT_create);
                Stats.Add(Stats.List.novelscreated);
            }

            path = Path.Combine(CreateStory.editPath, "manifest.json");
            Manifest data = new Manifest();
            if (File.Exists(path))
                data = Manifest.Get(path);

            data.name = B_inputName.text;
            data.description = B_inputDescription.text;
            data.tags = B_inputTags.text.Replace(", ", ",").Replace(" ,", ",").Split(',');
            data.genre = Helper.genresid[B_inputGenre.value];
            data.rating = Helper.audience[B_inputAudience.value];
            data.language = Language.ListWindowsKeys()[B_inputLanguage.value];
            data.customname = B_inputCustomName.isOn;
            data.author = Helper.UsernameCache();
            data.authorid = Helper.SteamIDCache();

            if (isNew) data.uploaddate = DateTime.Now;
            data.lastEdit = DateTime.Now;

            if (data.rating == "Questionable" || data.rating == "Mature") data.nsfw = true;
            else data.nsfw = false;

            Manifest.Write(path, data);

            if (isNew) Main.SetWindow(2);
            Stories.pathUpdateNeeded = true;
            StartCoroutine(B_SetStatus(Translate.Get("saved")));
        }
        catch (Exception e)
        {
            Helper.Alert(string.Format(Translate.Get("errorsavingdetails") + "\n\n", Translate.Get("helpcontact"), e));
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

    private IEnumerator B_SetStatus(string text)
    {
        B_Status.text = text;
        yield return new WaitForSeconds(3);
        B_Status.text = "";
    }
}