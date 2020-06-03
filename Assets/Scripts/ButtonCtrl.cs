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
using SajberSim.Translation;

public class ButtonCtrl : MonoBehaviour
{
    public GameObject OverwriteAlert;
    public GameObject Logo;
    public GameObject Modding;
    public GameObject CreditsButton;
    public GameObject SettingsMenu;
    public Button ContinueButton;
    public Button DebugButton;
    public GameObject BehindSettings;
    public static Character[] people = new Character[4];
    public GameObject fadeimage;
    public AudioSource music;
    public static string charpath;

    private Helper shelper;
    private Download dl;

    
    public GameObject PauseMenuGame;
    public GameObject SettingsMenuGame;


    

    public void Start()
    {
        if (GameObject.Find("Helper"))
        {
            shelper = GameObject.Find("Helper").GetComponent<Helper>();
            dl = GameObject.Find("Helper").GetComponent<Download>();
        }
        else
        {
            shelper = new GameObject("Helperobj").AddComponent<Helper>();
            dl = new GameObject("downloadobj").AddComponent<Download>();
        }
        
        Cursor.visible = true;
        if (PlayerPrefs.GetString("story", "none") == "none" || PlayerPrefs.GetString("script", "none") == "none") //ifall man inte har spelat tidigare kan man inte använda den knappen
            ContinueButton.interactable = false;

        UpdateCharacter();
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

    
    public void CreateCharacters()
    {
        System.Random rnd = new System.Random();
        string configPath = $"{Application.dataPath}/Story/{PlayerPrefs.GetString("story")}/Characters/characterconfig.txt";
        if (!File.Exists(configPath)) return;
        string[] config = File.ReadAllLines(configPath);

        people = new Character[config.Length]; //change size to amount of ppl
        PlayerPrefs.SetInt("characters", config.Length); //amount of characters

        for (int i = 0; i < config.Length; i++) //fill array from file
            people[i] = new Character(config[i].Split(',')[0], config[i].Split(',')[1], i);

        people = people.OrderBy(x => rnd.Next()).ToArray(); //randomize array

        for (int i = 0; i < people.Length; i++) //sparar ID i playerpref
            PlayerPrefs.SetInt($"character{i}",people[i].ID);
    }
    private void LoadCharacters() //Loads characters from playerprefs
    {
        string path = $"{Application.dataPath}/Story/{PlayerPrefs.GetString("story")}/Characters/characterconfig.txt";
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
        Manifest data = Helper.GetManifest($"{Application.dataPath}/Story/{PlayerPrefs.GetString("story")}/manifest.json");
        StartStory.storymenuOpen = false;
        GameManager.storyAuthor = data.author;
        GameManager.storyName = data.name;
        LoadCharacters();
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
    
    public void OpenFolder(string path)
    {
        if (!Directory.Exists($@"{Application.dataPath}/{path}"))
        {
            UnityEngine.Debug.LogError($"Tried to open folder with argument \"{path}\" which does not exist (full path: {Application.dataPath}/{path}");
            return;
        }
        if(path == "Logs") Helper.CreateLogfile();
        Process.Start("explorer.exe", $@"{Application.dataPath}/{path}");
    }
    public void FindAndCloseSettings()
    { 
        if(GameObject.Find("Canvas/Settings(Clone)"))
        GameObject.Find("Canvas/Settings(Clone)").GetComponent<Settings>().CloseMenu();
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
    public void OpenSettings()
    {
        GameObject x = Instantiate(SettingsMenu, Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas").GetComponent<Transform>()) as GameObject;
        x.transform.localPosition = Vector3.zero;
    }
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
    public IEnumerator FadeToScene(string scene)
    {
        StartCoroutine(AudioFadeOut.FadeOut(music, 1.55f));
        if(MainPopup.singlecharClicked) GameObject.Find("/CharEasterEgg").GetComponent<Animator>().Play("allchar popdown"); //fade away easter egg if active
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
        Helper.CreateLogfile();


    }
}