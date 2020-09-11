using SajberSim.Chararcter;
using SajberSim.Colors;
using SajberSim.Helper;
using SajberSim.SaveSystem;
using SajberSim.Steam;
using SajberSim.Translation;
using SajberSim.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Prefs = SajberSim.Helper.Helper.Prefs;

public class GameManager : MonoBehaviour
{
    public static bool ready = true; // If the game is ready to go to the next line (eg delays, downloads)
    public static bool textdone = false; // False if a text is currently writing out
    public static int dialoguepos = 0;
    public const float charactersize = 0.8f; // This is the size of game characters, not letters

    public GameObject nameInput;

    public static GameManager Instance;

    //Textbox
    public GameObject textbox;

    public GameObject portrait;
    public Text comment; //The normal text
    public Text nametag; //The nametag
    public Text commentPort; //The normal text
    public Text nametagPort; //The nametag

    //Alert
    public GameObject alertbox;

    public Text alert;

    //UI
    public GameObject background;

    public GameObject music;
    public GameObject uwuwarning;
    public GameObject fadeimage;
    public GameObject saveinfo;
    public GameObject SFX;
    public GameObject pausemenu;
    public GameObject SettingsMenu;
    private float timeScaleCache = 1;

    //Dropdown
    public Text dropdownQ;

    public GameObject dropdownObject;
    public GameObject dropdownMenu;
    public GameObject dropdownItemBackground;
    public GameObject dropdownBackground;

    //Question
    public GameObject questionbox;

    public GameObject qbutton1;
    public GameObject qbutton2;
    public Text question;
    public Text alt1t;
    public Text alt2t;
    public string story1;
    public string story2;

    public static bool savemenuopen = false;
    private bool settingsopen = false;
    public static bool paused = false;
    public GameObject behindButton;

    public static string[] story;
    public Coroutine co;
    public static Person[] people;

    public Manifest data;
    public static Save save = null;
    public static bool usernameNeeded;
    public static bool usernameEntered;
    public static string username = "Player";
    public static string storyName;
    public static string storyAuthor;
    public static string scriptPath;
    public static string shortStoryPath;
    public static string scriptName;
    public static bool backgroundHasChanged = false;
    public static string currentBackground;
    public static string currentPortrait = "";
    public static string currentMusic;

    #region Actions

    public GameObject HelperObj;
    public Download dl;
    private Alert Action_Alert;
    private Background Action_Background;
    private Textbox Action_Textbox;
    private Character Action_Character;
    private DelCharacter Action_DelCharacter;
    private Question Action_Question;
    private LoadScript Action_LoadScript;
    private Wait Action_Wait;
    private PlayAudio Action_PlayAudio;
    private StopAudio Action_StopAudio;
    private Mood Action_Mood;

    private void SetActionClasses()
    {
        dl = Download.Init();
        Action_Alert = HelperObj.AddComponent<Alert>();
        Action_Background = HelperObj.AddComponent<Background>();
        Action_Textbox = HelperObj.AddComponent<Textbox>();
        Action_Character = HelperObj.AddComponent<Character>();
        Action_DelCharacter = HelperObj.AddComponent<DelCharacter>();
        Action_Question = HelperObj.AddComponent<Question>();
        Action_LoadScript = HelperObj.AddComponent<LoadScript>();
        Action_Wait = HelperObj.AddComponent<Wait>();
        Action_PlayAudio = HelperObj.AddComponent<PlayAudio>();
        Action_StopAudio = HelperObj.AddComponent<StopAudio>();
        Action_Mood = HelperObj.AddComponent<Mood>();
    }

    #endregion Actions

    private void Start()
    {
        Instance = this;
        SetActionClasses();
        shortStoryPath = new DirectoryInfo(Helper.currentStoryPath).Name;
        StartGame(save);
    }

    public void StartGame(Save save = null)
    {
        //Reset static values
        savemenuopen = false;
        usernameNeeded = false;
        usernameEntered = false;
        username = "Player";
        backgroundHasChanged = false;
        currentBackground = "";
        currentPortrait = "";
        currentMusic = "";
        paused = false;
        dialoguepos = 0;
        ready = true;
        textdone = false;
        scriptName = "start";
        people = Person.Assign();
        RemoveCharacters();
        ClearText();
        story = new string[0];
        Cursor.visible = true;
        AudioListener.volume = PlayerPrefs.GetFloat(Prefs.volume.ToString(), 1f);

        if (save != null)
        {
            Debug.Log($"GameManager/StartGame: Trying to load savefile {save.novelname}");
            scriptName = save.script;
            dialoguepos = save.line;
            if (save.music != "") Action_PlayAudio.Run($"PLAYMUSIC|{save.music}".Split('|'));
            if (save.background != "") Action_Background.Run($"BACKGROUND|{save.background}|true".Split('|'));
            foreach (PersonSave person in save.characters)
                Action_Character.Run($"CHAR|{person.name}|{person.mood}|{person.x}|{person.y}|{person.size}|{person.flipped}".Split('|'));
            ready = true;
            backgroundHasChanged = true;
            username = save.username;
            usernameEntered = true;
            currentPortrait = "";
            people = save.charconfig;
            GameManager.save = null;
        }
        scriptPath = Path.Combine(Helper.currentStoryPath, "Dialogues", scriptName + ".txt");
        if (File.Exists(scriptPath))
            story = File.ReadAllLines(scriptPath);
        else
        {
            background = Helper.Alert(string.Format(Translate.Get("missingstartscript"), $"{shortStoryPath}/Dialogues/start.txt"));
            Debug.LogError($"Visual Novel/Start: Starting script for story {shortStoryPath} is missing.");
        }
        data = Manifest.Get(Path.Combine(Helper.currentStoryPath, "manifest.json"));
        if (data.customname && !usernameEntered)
        {
            RequestName();
        }
        UnityEngine.Debug.Log($"Entered visual novel. Details:\nName: {Helper.currentStoryName}\nPath: {Helper.currentStoryPath}");
        UpdateDesign();
        RunNext();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!background.activeSelf) StartCoroutine(GoToMain());
        if (Input.GetKeyDown(KeyCode.F1)) //toggles debug stuff
        {
            ToggleDevmenu();
        }
        if (story.Length == 0) return;
        if (PlayerPrefs.GetInt(Prefs.uwu.ToString(), 0) == 1) uwuwarning.SetActive(true);
        else uwuwarning.SetActive(false);
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0) || story[dialoguepos] == "" || story[dialoguepos].StartsWith("//")) && !paused)
        {
            if (textdone && ready)
            {
                ClearText();
                RunNext();
            }
            else textdone = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                StartCoroutine(Pause(true));
            }
            else if (paused && settingsopen)
            {
                GameObject.Find("Canvas/Settings").GetComponent<Settings>().CloseMenu();
                settingsopen = false;
            }
            else if (paused && savemenuopen)
            {
                if (GameObject.Find("Canvas/SaveLoadMenu"))
                    GameObject.Find("Canvas/SaveLoadMenu").GetComponent<SaveMenu>().ToggleMenu(false);
                settingsopen = false;
            }
            else if (paused && !settingsopen && !savemenuopen)
            {
                StartCoroutine(Pause(false));
            }
        }
        if (data.customname && !usernameEntered && Input.GetKeyDown(KeyCode.Return)) SetName();
    }

    public void ToggleDevmenu(bool forceopen = false)
    {
        if (PlayerPrefs.GetInt(Prefs.devmenu.ToString(), 0) == 0 || forceopen)
        {
            fadeimage.SetActive(false);
            PlayerPrefs.SetInt(Prefs.devmenu.ToString(), 1);
            GameObject.Find("/Canvas/dev").transform.localScale = Vector3.one;
        }
        else
        {
            PlayerPrefs.SetInt(Prefs.devmenu.ToString(), 0);
            GameObject.Find("/Canvas/dev").transform.localScale = Vector3.zero;
        }
    }

    public void RunNext()
    {
        if (!ready || story.Length == 0) return;
        if (usernameNeeded && !usernameEntered) return;

        string[] line = story[dialoguepos].Split('|'); //line = nuvarande raden
        GameObject.Find("/Canvas/dev/varinfo").GetComponent<Text>().text = $"line = {dialoguepos}\naction = {story[dialoguepos].Split('|')[0]}\nready = {ready}\nscript: = {scriptName}\n\n{story[dialoguepos]}";
        if (line[0] == "" || line[0].StartsWith("//")) //blank/comment = ignore
        {
            dialoguepos++;
            RunNext();
        }
        else if (line[0] == "T" || line[0] == "T2") //textbox
        {
            dialoguepos++;
            Action_Textbox.Run(line);
        }
        else if (line[0] == "ALERT") //general box
        {
            dialoguepos++;
            Action_Alert.Run(line);
        }
        else if (line[0] == "BACKGROUND") //new background
        {
            dialoguepos++;
            Action_Background.Run(line);
        }
        else if (line[0] == "CHAR") //move or create character
        {
            dialoguepos++;
            Action_Character.Run(line);
        }
        else if (line[0] == "DELETE") //delete character
        {
            dialoguepos++;
            Action_DelCharacter.Run(line);
        }
        else if (line[0] == "QUESTION") //question
        {
            fadeimage.SetActive(false);
            ready = false;
            Action_Question.Run(line);
        }
        else if (line[0] == "LOADSTORY") //open new story (no question)
        {
            Action_LoadScript.Run(line);
        }
        else if (line[0] == "WAIT") //delay
        {
            dialoguepos++;
            Action_Wait.Run(line);
        }
        else if (line[0] == "MOOD") //set mood
        {
            dialoguepos++;
            Action_Mood.Run(line);
        }
        else if (line[0] == "PLAYMUSIC" || line[0] == "PLAYSFX")
        {
            dialoguepos++;
            Action_PlayAudio.Run(line);
        }
        else if (line[0] == "STOPSOUNDS" || line[0] == "STOPMUSIC" || line[0] == "STOPSFX")
        {
            dialoguepos++;
            Action_StopAudio.Run(line);
        }
        else if (line[0] == "FINISHGAME")
        {
            StartCoroutine(StartCredits());
        }
        else
        {
            Helper.Alert(string.Format(Translate.Get("invalidaction"), dialoguepos, scriptName, line[0]));
            Debug.LogError($"Visual Novel/Runnext: Error at line {dialoguepos} in {Path.Combine(Helper.currentStoryPath, "Dialogues", scriptName + ".txt")}\nError: \"{line[0]}\" is not a valid action. Trying to skip...\nText: {string.Join("|", line)}");
            dialoguepos++;
            RunNext();
        }
    }

    #region Characters

    public static void RemoveCharacters() //used in setup too
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("character");

        for (var i = 0; i < gameObjects.Length; i++)
            Destroy(gameObjects[i]);
    }

    public void RemoveCharactersWrap() //Only used for editor assignments as they can't run static methods
    {
        RemoveCharacters();
    }

    #endregion Characters

    #region Text

    public string FillVars(string text) //Changes {1.name} to the name of person 1, and {0.nick} to the nickname of 0 etc
    {
        MatchCollection matches = Regex.Matches(text, @"{(\d+)\.(\w+)}"); //Matches {1.name} with "1" & "name" as a group

        foreach (Match match in matches)
        {
            if (people.Length < int.Parse(match.Groups[1].Value)) break;
            string replace = "";
            if (match.Groups[2].Value == "name")
                replace = people[int.Parse(match.Groups[1].Value)].name;
            if (match.Groups[2].Value == "nick")
                replace = people[int.Parse(match.Groups[1].Value)].nick;

            text = new Regex("{(\\d+)\\.(\\w+)}").Replace(text, replace, 1); //Byter
        }
        if (!text.Contains("{") && !text.Contains("\\n")) return text;
        text = text.Replace("\\n", Environment.NewLine);
        text = text.Replace("{path}", Helper.currentStoryPath);
        text = text.Replace("{time}", DateTime.Now.ToString("HH:mm"));
        text = text.Replace("{date}", DateTime.Now.ToString("d"));
        text = text.Replace("{year}", DateTime.Now.ToString("yyyy"));
        text = text.Replace("{steamname}", Helper.UsernameCache());
        text = text.Replace("{name}", username);
        return text;
    }

    private void ClearText()
    {
        textbox.SetActive(false);
        alertbox.SetActive(false);
        questionbox.SetActive(false);
        commentPort.text = "";
        comment.text = "";
        nametag.text = "";
        nametagPort.text = "";
        alert.text = "";
        question.text = "";
        alt1t.text = "";
        alt2t.text = "";
    }

    #endregion Text

    #region Questions

    public void AnswerQuestion(int id)
    {
        Stats.Add(Stats.List.decisionsmade);
        string[] stories = { story1, story2 };
        Action_LoadScript.Load(stories[id - 1]);

        #region openhouse

        if (Helper.currentStoryName == "OpenHouse") // Made to get data about a campaign for https://cybergymnasiet.se/
            Analytics.CustomEvent("program_picked", new Dictionary<string, object> { { "program", stories[id - 1] } });

        #endregion openhouse

        questionbox.SetActive(false);
    }

    public void AnswerQuestionDD(int select)
    {
        Stats.Add(Stats.List.decisionsmade);
        string[] line = story[dialoguepos].Split('|');
        List<string> options = new List<string>();
        for (int i = 3; i < line.Length; i = i + 2)
        {
            options.Add(line[i]);
        }

        #region openhouse

        if (Helper.currentStoryName == "OpenHouse") // Made to get data about a campaign for https://cybergymnasiet.se/
            Analytics.CustomEvent("program_picked", new Dictionary<string, object> { { "program", options[select - 1] } });

        #endregion openhouse

        Action_LoadScript.Load(options[select - 1]);
        dropdownMenu.SetActive(false);
    }

    #endregion Questions

    #region UI

    public void RequestName()
    {
        usernameNeeded = true;
        nameInput.SetActive(true);
        Time.timeScale = 0;
        fadeimage.GetComponent<Image>().color = Color.black;
    }

    public void GameContinue()
    {
        StartCoroutine(Pause(false));
    }

    public IEnumerator Pause(bool n)
    {
        fadeimage.SetActive(false);
        paused = n;
        if (n)
        {
            timeScaleCache = Time.timeScale;
            Time.timeScale = 0;
            ScreenCapture.CaptureScreenshot(Path.Combine(Application.temporaryCachePath, "lastGame.png"));
        }
        else
        {
            Time.timeScale = timeScaleCache;
        }

        yield return new WaitForEndOfFrame();

        pausemenu.SetActive(n);
        //fadeimage.SetActive(false);
        behindButton.SetActive(n);
        GameObject settings = GameObject.Find("Canvas/Settings");
        if (settings != null) settings.GetComponent<Settings>().CloseMenu();
    }

    public void OpenSettings()
    {
        settingsopen = true;
        GameObject x = Instantiate(SettingsMenu, Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas").GetComponent<Transform>()) as GameObject;
        x.transform.localPosition = Vector3.zero;
        x.name = "Settings";
    }

    public void SettingsQuit()
    {
        StartCoroutine(GoToMain());
    }

    private IEnumerator GoToMain()
    {
        fadeimage.SetActive(true);
        fadeimage.GetComponent<Animator>().updateMode = AnimatorUpdateMode.UnscaledTime;
        fadeimage.GetComponent<Animator>().Play("darken");
        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;
        SceneManager.LoadScene("menu");
    }

    public IEnumerator StartCredits(bool addStats = true) //Avslutar & återställer spelet och startar credits
    {
        StartCoroutine(FadeOut(music.GetComponent<AudioSource>(), 1.3f, 0));
        Credits.storypath = Helper.currentStoryPath;

        fadeimage.SetActive(true); //Open image that will fade (starts at opacity 0%)
        fadeimage.GetComponent<Animator>().Play("darken");
        yield return new WaitForSeconds(0.5f);
        if (addStats) Stats.Add(Stats.List.novelsfinished);
        if (File.Exists(Path.Combine(Helper.currentStoryPath, "credits.txt")))
            SceneManager.LoadScene("credits");
        else
            SceneManager.LoadScene("menu");
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public void SetStoryFromDev(string story)
    {
        if (!File.Exists(Path.Combine(Helper.currentStoryPath, "Dialogues", story + ".txt"))) return;
        Action_StopAudio.Run("STOPSOUNDS".Split('|'));
        Action_LoadScript.Run($"LOADSCRIPT|{story}".Split('|'));
        RemoveCharacters();
    }

    public void SetName()
    {
        InputField input = nameInput.transform.Find("InputField").GetComponent<InputField>();
        if (input.text.Length == 0) return;

        username = input.text;
        usernameEntered = true;
        Time.timeScale = 1;
        if (username == "Fabina") Achievements.Grant(Achievements.List.ACHIEVEMENT_imfabina);
        Achievements.Grant(Achievements.List.ACHIEVEMENT_setname);
        Destroy(nameInput);
    }

    private void UpdateDesign()
    {
        StoryDesign design = StoryDesign.Get();
        string textboxpath = Path.Combine(Helper.currentStoryPath, "textbox.png");
        if (File.Exists(textboxpath))
        {
            Debug.Log($"Found textbox at path {textboxpath} and will try to update...");
            dl.Image(textbox, "file://" + textboxpath);
            dl.Image(alertbox, "file://" + textboxpath);
        }

        Color textColor = Colors.FromRGB(design.textcolor);
        commentPort.color = textColor;
        comment.color = textColor;
        dropdownQ.color = textColor;
        question.color = textColor;
        alert.color = textColor;

        Color buttonColor = Colors.FromRGB(design.questioncolor);
        qbutton1.GetComponent<Image>().color = buttonColor;
        qbutton2.GetComponent<Image>().color = buttonColor;
        dropdownObject.GetComponent<Image>().color = buttonColor;
        dropdownBackground.GetComponent<Image>().color = Helper.ModifyColor(buttonColor, 1.05f);
        dropdownItemBackground.GetComponent<Image>().color = Helper.ModifyColor(buttonColor, 1.1f);

        Color buttonTextColor = Colors.FromRGB(design.questiontextcolor);
        alt1t.color = buttonTextColor;
        alt2t.color = buttonTextColor;

        Color nameColor = Colors.FromRGB(design.namecolor);
        nametag.color = nameColor;
        nametagPort.color = nameColor;
    }

    #endregion UI

    #region Saving

    public IEnumerator SaveInfo() //just shows the user that the game got saved. simple huh?
    {
        saveinfo.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        saveinfo.SetActive(false);
    }

    public Save Save(int id)
    {
        bool newCard = false;
        if (id == -1)
        {
            newCard = true;
            bool found = false;
            while (!found)
            {
                id++;
                if (!File.Exists(Path.Combine(Helper.savesPath, id + ".save"))) found = true;
            }
        }
        List<PersonSave> chars = new List<PersonSave>();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("character"))
        {
            PersonSave save = new PersonSave
            {
                x = go.transform.position.x,
                y = go.transform.position.y,
                flipped = go.transform.localScale.x < 0,
                size = go.transform.localScale.y / charactersize,
                name = go.name.Split('|')[0],
                mood = go.name.Split('|')[1]
            };
            chars.Add(save);
        }
        Save file = new Save
        {
            line = dialoguepos,
            background = currentBackground,
            music = currentMusic,
            novelname = Helper.currentStoryName,
            charconfig = people,
            path = Helper.currentStoryPath,
            username = username,
            script = scriptName,
            date = DateTime.Now,
            splashcolor = data.overlaycolor,
            textcolor = data.textcolor,
            characters = chars.ToArray()
        };
        SajberSim.SaveSystem.Save.Create(file, id, newCard);
        return file;
    }

    #endregion Saving
}