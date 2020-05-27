using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SajberSim.Chararcter;
using SajberSim.Web;
using SajberSim.StoryDebug;
using SajberSim.Story;
using SajberSim.Helper;

public class ButtonCtrl : MonoBehaviour
{
    public GameObject OverwriteAlert;
    public GameObject Logo;
    public GameObject Settings;
    public GameObject Modding;
    public GameObject CreditsButton;
    public Button ContinueButton;
    public Button DebugButton;
    public Toggle uwu;
    public Slider Speed;
    public Slider Volume;
    public Text SpeedText;
    public Text VolumeText;
    public Text UwuText;
    public GameObject BehindSettings;
    public static Character[] people = new Character[4];
    public GameObject fadeimage;
    public AudioSource music;

    private readonly Helper shelper = new Helper();
    //pause stuff ingame
    public static bool paused = false;
    public GameObject PauseMenuGame;
    public GameObject SettingsMenuGame;


    private Download dl;

    //for easter egg. stop bullying my code
    private bool eggclicked;
    private string charpath;
    private bool eggran;


    public void Start()
    {
        dl = (new GameObject("downloadobj")).AddComponent<Download>();
        Cursor.visible = true;
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
        AudioListener.volume = PlayerPrefs.GetFloat("volume", 1f); //sets volume to player value

        UpdateCharacter();
    }
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && SceneManager.GetActiveScene().name != "menu")
            TogglePause();
    }
    void UpdateCharacter()
    {
        if (SceneManager.GetActiveScene().name == "menu")
        {
            List<string> charpaths = new List<string>();
            foreach (string path in shelper.GetAllStoryAssetPaths("characters"))
            {
                if (path.Contains("happy")) charpaths.Add(path);
            }
            charpath = charpaths[UnityEngine.Random.Range(0,charpaths.Count)];
            while (!File.Exists(charpath.Replace("happy", "blush")))
            {
                charpath = charpaths[UnityEngine.Random.Range(0, charpaths.Count)];
            }

            //ladda in filen som texture
            dl.Image(GameObject.Find("Character"), $"file://{charpath}");
        }
    }
    public void TogglePause()
    {
        if (!paused)
        {
            paused = true;
            OpenMenu(PauseMenuGame);
            Time.timeScale = 0;
        }
        else
        {
            CloseSettings();
            paused = false;
            Time.timeScale = 1;
        }
    }
    public void StartNew() //Just checks if a new story should be started
    {
        if (PlayerPrefs.GetString("story", "none") != "none") //story som inte är start hittad
            OverwriteAlert.SetActive(true);
        else
            StartNewConfirmed();
    }

    public void StartNewConfirmed() //confirmed that user wants to start a new
    {
        GameObject.Find("Canvas/StoryChoice").GetComponent<StartStory>().OpenMenu();
    }

    public void CharEasteregg()
    {
        if (!eggclicked)
        {
            if (File.Exists(charpath.Replace("happy", "blush")))
                dl.Image(GameObject.Find("Character"), $"file://{charpath.Replace("happy", "blush")}");
            eggclicked = true;
        }
        else
        {
            GameObject.Find("/Canvas/Character").GetComponent<Animator>().Play("characterbye");
            GameObject.Find("/CharEasterEgg").GetComponent<Animator>().Play("allchar popup");
            eggran = true;
        }
    }
    private void CreateCharacters(int id)
    {
        System.Random rnd = new System.Random();
        string[] config = File.ReadAllLines($"{Application.dataPath}/Modding/Characters/characterconfig.txt");

        people = new Character[config.Length]; //change size to amount of ppl
        PlayerPrefs.SetInt("characters", config.Length); //amount of characters

        for (int i = 0; i < config.Length; i++) //fill array from file
            people[i] = new Character(config[i].Split(',')[0], config[i].Split(',')[1], i);

        people = people.OrderBy(x => rnd.Next()).ToArray(); //randomize array

        for (int i = 0; i < people.Length; i++) //sparar ID i playerpref
            PlayerPrefs.SetInt($"character{i}",people[i].ID);
    }
    private void LoadCharacters(string story) //Loads characters from playerprefs
    {
        string path = $"{Application.dataPath}/Story/{story}/Characters/characterconfig.txt";
        if (!File.Exists(path)) return;
        string[] config = File.ReadAllLines(path);
        people = new Character[PlayerPrefs.GetInt("characters", 1)];

        for (int i = 0; i < people.Length; i++) //fill array from save
        {
            int tempID = PlayerPrefs.GetInt($"character{i}", 0);
            people[i] = new Character(config[tempID].Split(',')[0], config[tempID].Split(',')[1], i);
        }
    }
    public void Continue() //just opens everything SAVED
    {
        LoadCharacters(PlayerPrefs.GetString("story"));
        StartCoroutine(FadeToScene("game"));
    }

    public void OpenMenu(GameObject menu) //opens a menu, like settings or modding
    {
        menu.SetActive(true);
        BehindSettings.SetActive(true);
    }
    public void GoBack()
    {
        SettingsMenuGame.SetActive(false);
    }

    public void CloseSettings() //closes all menus
    {
        Settings.SetActive(false);
        BehindSettings.SetActive(false);
        Modding.SetActive(false);
        PauseMenuGame.SetActive(false);
        SettingsMenuGame.SetActive(false);
        OverwriteAlert.SetActive(false);
        GameManager.paused = false;
        
    }
    public void QuitGame() //nuff' said
    {
        Application.Quit();
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
    Process.Start("explorer.exe", $@"{Application.dataPath}/Modding/{path}/".Replace("/", "\\"));
    }
    public void OpenLink(string link)
    {
        Process.Start(link);
    }
    public void OpenLogfile()
    {
        Process.Start($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/LocalLow/Te18B/SajberSim/Player.log".Replace("/", "\\"));
    }
    /*public void Debug()
    {
        StoryDebugger.CreateLog();
        StartCoroutine(ToggleDebug());
        
    }*/
    public void GAMEOpenSettings()
    {
        PauseMenuGame.SetActive(false);
        SettingsMenuGame.SetActive(true);
        GameManager.paused = true;
    }
    IEnumerator ToggleDebug() //disables the button for 2 seconds to avoid doubleclicks
    {
        DebugButton.interactable = false;
        yield return new WaitForSeconds(2f);
        DebugButton.interactable = true;
    }
    public void StartScene(string scene) //seems like i couldn't start coroutines with buttons
    {
        StartCoroutine(FadeToScene(scene));
    }
    public void ReturnToMain()
    {
        CloseSettings();
        Time.timeScale = 1;
        StartCoroutine(FadeToScene("menu"));
    }
    public IEnumerator FadeToScene(string scene)
    {
        StartCoroutine(AudioFadeOut.FadeOut(music, 1.55f));
        if(eggran) GameObject.Find("/CharEasterEgg").GetComponent<Animator>().Play("allchar popdown"); //fade away easter egg if active
        fadeimage.SetActive(true); //Open image that will fade (starts at opacity 0%)

        for (float i = 0; i <= 1; i += Time.deltaTime/1.5f) //Starts fade, load scene when done
        {
            fadeimage.GetComponent<Image>().color = new Color(0, 0, 0, i);
            if (i > 0.5f) Cursor.visible = false;
            yield return null;
        }
        SceneManager.LoadScene(scene);
    }
    private void OnApplicationQuit()
    {
        DiscordRpc.Shutdown(); //Stänger Discord RPC

        if (Application.isEditor) return; //Skapar loggfil utanför Unity
        DateTime now = DateTime.Now;
        string sourceFile = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/../LocalLow/LiterallyFabian/SajberSim/Player.log".Replace("/", "\\");
        string destFile = $@"{Application.dataPath}/Logs/SajberSim {now.Year}.{now.Day}.{now.Month} - {now.Hour}.{now.Minute}.{now.Second}.txt".Replace("/", "\\");
        System.IO.Directory.CreateDirectory($@"{Application.dataPath}\Logs");
        System.IO.File.Copy(sourceFile, destFile, true);
    }
}