using SajberSim.Helper;
using SajberSim.Translation;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CreateStory : MonoBehaviour
{
    public static string currentlyEditingPath = @"H:\School code stuff\CyberSim\CyberSim\Assets\Story\OpenHouse"; //Base path, should be root/SajberSim_Data/MyStories/X

    public InputField B_inputName;
    public InputField B_inputDescription;
    public InputField B_inputTags;
    public Dropdown B_inputGenre;
    public Dropdown B_inputAudience;
    public Dropdown B_inputLanguage;

    public GameObject ButtonGroup;
    public GameObject BasicsGroup;
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
    public enum CreateWindows
    {
        Basics,
        Details,
        Verify,
        Publish
    }

    void Start()
    {
        dl = new GameObject("downloadobj").AddComponent<Download>();
        transform.localScale = Vector3.zero;
        transform.localPosition = Vector3.zero;

        B_inputGenre.AddOptions(Helper.genres.ToList());
        B_inputAudience.AddOptions(Helper.audience.ToList());

        List<Dropdown.OptionData> dropdownList = new List<Dropdown.OptionData>();
        foreach (Language lang in Language.list)
            dropdownList.Add(new Dropdown.OptionData(lang.formal_name, dl.Flag(lang.flag_code)));
        B_inputLanguage.AddOptions(dropdownList);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ToggleMenu(bool open)
    {
        transform.localScale = open ? Vector3.one : Vector3.zero;
    }
    public void SetWindow(int window)
    {
        switch (window)
        {
            case 0:
                SetBasics();
                ButtonDetails.interactable = false;
                break;
        }
    }
    private void SetBasics()
    {
        BasicsGroup.transform.localScale = Vector3.one;
        Manifest data = Helper.GetManifest(currentlyEditingPath + "/manifest.json");
        Title.text = Translate.Get("details");
        Description.text = Translate.Get("detailsDescription");
        B_inputName.SetTextWithoutNotify(data.name);
        B_inputDescription.SetTextWithoutNotify(data.description);
        B_inputTags.SetTextWithoutNotify(string.Join(", ", data.tags));

    }

}
