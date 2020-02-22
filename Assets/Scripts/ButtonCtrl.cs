using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static PersonClass;

public class ButtonCtrl : MonoBehaviour
{
    public GameObject OverwriteAlert;
    public GameObject Logo;
    public GameObject Settings;
    public GameObject Modding;
    public Button ContinueButton;
    public Toggle uwu;
    public Slider Speed;
    public Slider Volume;
    public Text SpeedText;
    public Text VolumeText;
    public Text UwuText;
    public GameObject BehindSettings;
    public static Character[] people = new Character[4];


    public void Start()
    {
        if (PlayerPrefs.GetString("story", "start") != "start") //ifall man inte har spelat tidigare kan man inte använda den knappen
            ContinueButton.interactable = true;
        else
            ContinueButton.interactable = false;

        SpeedText.text = $"{Math.Round(PlayerPrefs.GetFloat("delay", 0.04f) * 1000)}ms";
        VolumeText.text = $"{Math.Round(PlayerPrefs.GetFloat("volume", 1f) * 100)}%";

        if (PlayerPrefs.GetInt("uwu", 0) == 1)
        {
            uwu.SetIsOnWithoutNotify(true);
            UwuText.text = "(◠‿◠✿)";
        }
        
        Speed.SetValueWithoutNotify(PlayerPrefs.GetFloat("delay",0.04f));
        Volume.SetValueWithoutNotify(PlayerPrefs.GetFloat("volume", 1f));

    }
    public void StartNew() //Just checks if a new story should be started
    {
        if (PlayerPrefs.GetString("story", "start") != "start") //story som inte är start hittad
            OverwriteAlert.SetActive(true);
        else
            StartNewConfirmed();
    }
    public void StartNewConfirmed() //starts a NEW story
    {
        PlayerPrefs.SetString("story", "start"); //återställ storyn på förfrågan
        CreateCharacters();
        SceneManager.LoadScene("game");
        UnityEngine.Debug.Log("New game created.");
    }
    private void CreateCharacters()
    {
        System.Random rnd = new System.Random();
        string[] config = File.ReadAllLines($"{Application.dataPath}/Characters/characterconfig.txt");

        people = new Character[config.Length]; //change size to amount of ppl
        PlayerPrefs.SetInt("characters", config.Length); //amount of characters

        for (int i = 0; i < config.Length; i++) //fill array from file
            people[i] = new Character(config[i].Split(',')[0], config[i].Split(',')[1], i);

        people = people.OrderBy(x => rnd.Next()).ToArray(); //randomize array

        for (int i = 0; i < people.Length; i++) //sparar ID i playerpref
            PlayerPrefs.SetInt($"character{i}",people[i].ID);
    }
    private void CreateDevCharacters()
    {
        System.Random rnd = new System.Random();
        string[] config = File.ReadAllLines($"{Application.dataPath}/Characters/characterconfig.txt");

        people = new Character[config.Length]; //change size to amount of ppl
        PlayerPrefs.SetInt("characters", config.Length); //amount of characters

        for (int i = 0; i < config.Length; i++) //fill array from file
            people[i] = new Character(config[i].Split(',')[0], config[i].Split(',')[1], i);

        people = people.OrderBy(x => rnd.Next()).ToArray(); //randomize array
    }
    private void LoadCharacters() //Loads characters from playerprefs
    {
        string[] config = File.ReadAllLines($"{Application.dataPath}/Characters/characterconfig.txt");
        people = new Character[PlayerPrefs.GetInt("characters", 1)];

        for (int i = 0; i < people.Length; i++) //fill array from save
        {
            int tempID = PlayerPrefs.GetInt($"character{i}", 0);
            people[i] = new Character(config[tempID].Split(',')[0], config[tempID].Split(',')[1], i);
        }
    }
    public void Continue() //just opens everything SAVED
    {
        LoadCharacters();
        SceneManager.LoadScene("game");
    }
    public void CancelNew() //closes the new game alert
    {
        OverwriteAlert.SetActive(false);
    }
    public void OpenMenu(GameObject menu) //opens a menu, like settings or modding
    {
        menu.SetActive(true);
        Logo.SetActive(false);
        BehindSettings.SetActive(true);
    }

    public void CloseSettings() //closes a menu
    {
        Settings.SetActive(false);
        Logo.SetActive(true);
        BehindSettings.SetActive(false);
        Modding.SetActive(false);
        
    }
    public void QuitGame() //nuff' said
    {
        Application.Quit();
    }
    public void OpenDev()
    {
        CreateDevCharacters();
        SceneManager.LoadScene("dev");
    }
    public void ResetAll() //fucking nukes all the stats like the US during 1945
    {
        PlayerPrefs.DeleteAll();
    }
    public void ChangeSpeed(float value) //runs when the speed slider is changed
    {
        PlayerPrefs.SetFloat("delay", value);
        SpeedText.text = $"{Math.Round(PlayerPrefs.GetFloat("delay",0.04f)*1000)}ms";
    }
    public void ChangeVolume(float newVolume)
    {
        PlayerPrefs.SetFloat("volume", newVolume);
        AudioListener.volume = PlayerPrefs.GetFloat("volume",1f);
        VolumeText.text = $"{Math.Round(PlayerPrefs.GetFloat("volume", 1f)*100)}%";
    }
    public void UwUToggle(bool uwu)
    {
        if (uwu)
        {
            UwuText.text = "(◠‿◠✿)";
            PlayerPrefs.SetInt("uwu", 1);
        }
        else //disable / false
        {
            UwuText.text = "";
            PlayerPrefs.SetInt("uwu", 0);
        }
    }
    public void OpenFolder(string path)
    {
    Process.Start("explorer.exe", $@"{Application.dataPath}/{path}/".Replace("/", "\\"));
    }
    public void OpenLink(string link)
    {
        Process.Start(link);
    }
}
