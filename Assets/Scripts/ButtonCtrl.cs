using SajberSim.Chararcter;
using SajberSim.Helper;
using SajberSim.SaveSystem;
using SajberSim.Steam;
using SajberSim.Translation;
using SajberSim.Web;
using Steamworks;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

/// <summary>
/// Controls buttons on the main menu
/// </summary>
public class ButtonCtrl : MonoBehaviour
{
    public GameObject Logo;
    public GameObject SaveLoadMenu;
    public GameObject SettingsMenu;
    public GameObject BehindSettings;
    public static Person[] people = new Person[4];
    public GameObject fadeimage;
    public AudioSource music;
    public static string charpath;

    private Download dl;

    private void OnEnable()
    {
        if (!Helper.loggedin)
        {
            try
            {
                SteamClient.Init(Helper.AppID);
                Helper.loggedin = true;
                Debug.Log($"Steam: Connected to {SteamClient.Name} (ID: {SteamClient.SteamId})");
                PlayerPrefs.SetString("usernamecache", SteamClient.Name);
                PlayerPrefs.SetString("steamidcache", SteamClient.SteamId.ToString());
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Steam: Could not connect to steam. Is it open?\n{e}");
                Helper.Alert(Translate.Get("noconnection"));
                Helper.loggedin = false;
            }
        }
    }

    public void Start()
    {
        dl = Download.Init();
        Cursor.visible = true;
        GameObject.Find("Canvas/Version").GetComponent<Text>().text = 'v' + Application.version;
        SetLogin();
        //Stats.ShowAll();
    }

    private void SetLogin()
    {
        Text loginstatus = GameObject.Find("Canvas/Username").GetComponent<Text>();
        //Set login if you are logged in
        if (Helper.loggedin)
        {
            loginstatus.text = string.Format(Translate.Get("welcomeuser"), Helper.UsernameCache());
            dl.Image(GameObject.Find("Canvas/ProfilePictureFrame/ProfilePicture"), SteamAPI.GetProfile($"{SteamClient.SteamId}").Avatarmedium);
            Manifest.FixSteamID();
        }
        else
        {
            loginstatus.text = Translate.Get("offline");
            loginstatus.transform.localPosition = new Vector3(374.16f, 318.84f, 0);
            GameObject.Find("Canvas/ProfilePictureFrame").SetActive(false);
        }
    }

    //Called from profile picture
    public void OpenSteamProfile()
    {
        if (Helper.loggedin)
        {
            if (Application.isEditor)
                Process.Start($@"steam://openurl/{SteamAPI.GetProfile($"{SteamClient.SteamId}").Profileurl}");
            else
                SteamFriends.OpenUserOverlay(SteamClient.SteamId, "steamid");
        }
    }
    //Called from ButtonFind
    public void OpenWorkshop()
    {
        if (!Helper.loggedin)
        {
            Helper.Alert(Translate.Get("noconnection"));
            return;
        }
        Helper.Alert(Translate.Get("openedworkshop"));
        Process.Start($@"steam://openurl/https://steamcommunity.com/app/1353530/workshop/");
    }

    public static void CreateCharacters()
    {
        System.Random rnd = new System.Random();
        string configPath = $"{Helper.currentStoryPath}/Characters/characterconfig.txt";
        if (!File.Exists(configPath)) return;
        string[] config = File.ReadAllLines(configPath);

        people = new Person[config.Length]; //change size to amount of ppl
        PlayerPrefs.SetInt("characters", config.Length); //amount of characters

        for (int i = 0; i < config.Length; i++) //fill array from file
            people[i] = new Person(config[i].Split(',')[0], config[i].Split(',')[1], i);

        people = people.OrderBy(x => rnd.Next()).ToArray(); //randomize array

        for (int i = 0; i < people.Length; i++) //sparar ID i playerpref
            PlayerPrefs.SetInt($"character{i}", people[i].ID);
    }

    private void LoadCharacters() //Loads characters from playerprefs
    {
        string path = $"{Helper.currentStoryPath}/Characters/characterconfig.txt";
        if (!File.Exists(path)) return;
        string[] config = File.ReadAllLines(path);
        people = new Person[PlayerPrefs.GetInt("characters", 1)];

        for (int i = 0; i < people.Length; i++) //fill array from save
        {
            int tempID = PlayerPrefs.GetInt($"character{i}", 0);
            people[i] = new Person(config[tempID].Split(',')[0], config[tempID].Split(',')[1], i);
        }
    }

    public void OpenMenu(GameObject menu) //opens a menu, like settings or modding
    {
        menu.SetActive(true);
        BehindSettings.SetActive(true);
    }

    public void CloseSettings() //closes all menus
    {
        BehindSettings.SetActive(false);
        SaveLoadMenu.GetComponent<SaveMenu>().ToggleMenu(false);
        GameManager.paused = false;
    }

    public void QuitGame() //nuff' said
    {
        Application.Quit();
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public void OpenFolder(string path)
    {
        string fullPath = Path.Combine(Application.dataPath, path);
        if (!Directory.Exists(fullPath))
        {
            Debug.LogError($"Tried to open folder with argument \"{path}\" which does not exist (full path: {fullPath})");
            return;
        }
        if (path == "Logs") Helper.CreateLogfile();
        Process.Start(fullPath);
    }

    public void FindAndCloseSettings()
    {
        if (GameObject.Find("Canvas/Settings(Clone)"))
            GameObject.Find("Canvas/Settings(Clone)").GetComponent<Settings>().CloseMenu();
    }

    public void OpenLink(string link)
    {
        Process.Start(link);
    }

    public void OpenLogfile()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start(Path.Combine("~/.config/unity3d", Application.companyName, Application.productName, "Player.log"));
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("~/Library/Logs/Unity/Player.log");
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/LocalLow/Te18B/SajberSim/Player.log".Replace("/", "\\"));
        }
    }

    public void OpenSettings()
    {
        GameObject x = Instantiate(SettingsMenu, Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas").GetComponent<Transform>()) as GameObject;
        x.transform.localPosition = Vector3.zero;
    }

    public void StartScene(string scene) //seems like i couldn't start coroutines with buttons
    {
        StartCoroutine(FadeToScene(scene));
    }

    public IEnumerator FadeToScene(string scene)
    {
        StartCoroutine(AudioFadeOut.FadeOut(music, 0.4f));
        //if(MainPopup.singlecharClicked >= 5) GameObject.Find("/CharEasterEgg").GetComponent<Animator>().Play("allchar popdown"); //fade away easter egg if active
        MainPopup.singlecharClicked = 0;
        fadeimage.SetActive(true); //Open image that will fade (starts at opacity 0%)

        for (float i = 0; i <= 1; i += Time.deltaTime / 0.5f) //Starts fade, load scene when done
        {
            fadeimage.GetComponent<UnityEngine.UI.Image>().color = new UnityEngine.Color(0, 0, 0, i);
            if (i > 0.5f) Cursor.visible = false;
            yield return null;
        }
        SceneManager.LoadScene(scene);
    }

    private void OnApplicationQuit()
    {
        DiscordRpc.Shutdown(); //Stänger Discord RPC
        SteamClient.Shutdown();
        PlayerPrefs.Save();
        Helper.CreateLogfile();
    }
}