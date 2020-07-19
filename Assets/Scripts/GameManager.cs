using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System.Globalization;
using SajberSim.Web;
using SajberSim.Chararcter;
using System.Runtime.CompilerServices;
using SajberSim.Steam;
using SajberSim.Helper;
using SajberSim.Colors;
using SajberSim.Translation;
using System.Reflection;
/// <summary>
/// Needs a huge rewrite, but yeah this script runs the entire visual novel scene
/// </summary>
public class GameManager : MonoBehaviour
{
    public static bool ready = true; // If the game is ready to go to the next line (eg delays, downloads)
    public static bool textdone = false; // False if a text is currently writing out
    public static int dialoguepos = 0;
    public static float charactersize = 0.8f; // This is the size of game characters, not letters

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

    private bool settingsopen = false;
    public static bool paused = false;
    
    public static string[] story;
    public Coroutine co;
    public Person[] people = ButtonCtrl.people;
    public string musicplaying = "none";

    #region Classes
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
    #endregion

    public static string storyName;
    public static string storyAuthor;
    public static string scriptPath;
    public static string shortStoryPath;
    public static string scriptName;

    public interface INovelAction
    {
        /// <summary>
        /// Runs the action if it passes the debug stage
        /// </summary>
        /// <param name="line">Full line to the action to run</param>
        void Run(string[] line);
        /// <summary>
        /// Debugs the action and returns appropriate feedback
        /// </summary>
        /// <param name="line">Full line to debug</param>
        /// <returns>"" if everything works, else an error message</returns>
        string Working(string[] line);
    }

    private void SetActionClasses()
    {
        dl = HelperObj.AddComponent<Download>();
        Action_Alert = HelperObj.AddComponent<Alert>();
        Action_Background = HelperObj.AddComponent<Background>();
        Action_Textbox = HelperObj.AddComponent<Textbox>();
        Action_Character = HelperObj.AddComponent<Character>();
        Action_DelCharacter = HelperObj.AddComponent<DelCharacter>();
        Action_Question = HelperObj.AddComponent<Question>();
        Action_LoadScript = HelperObj.AddComponent<LoadScript>();
        Action_Wait = HelperObj.AddComponent<Wait>();

        Action_Alert.Game = GetComponent<GameManager>();
        Action_Background.Game = GetComponent<GameManager>();
        Action_Textbox.Game = GetComponent<GameManager>();
        Action_Character.Game = GetComponent<GameManager>();
        Action_DelCharacter.Game = GetComponent<GameManager>();
        Action_Question.Game = GetComponent<GameManager>();
        Action_LoadScript.Game = GetComponent<GameManager>();
        Action_Wait.Game = GetComponent<GameManager>();
    }
    private void Start()
    {
        SetActionClasses();
        paused = false;
        dialoguepos = 0;
        ready = true;
        textdone = false;
        scriptName = "start";
        Cursor.visible = true;
        AudioListener.volume = PlayerPrefs.GetFloat("volume", 1f);
        scriptPath = $"{Helper.currentStoryPath}/Dialogues/{PlayerPrefs.GetString("script", "start")}.txt";
        if (File.Exists(scriptPath))
        story = File.ReadAllLines(scriptPath);
        shortStoryPath = new DirectoryInfo(Helper.currentStoryPath).Name;


        PlayerPrefs.SetString("tempstory", PlayerPrefs.GetString("story", "start"));

        UnityEngine.Debug.Log($"Entered visual novel. Details:\nName: {Helper.currentStoryName}\nPath: {Helper.currentStoryPath}");
        UpdateDesign();
        RunNext();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) //toggles debug stuff
        {
            ToggleDevmenu();
        }

        if (PlayerPrefs.GetInt("uwu", 0) == 1) uwuwarning.SetActive(true);
        else uwuwarning.SetActive(false);
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0) || story[dialoguepos] == "" || story[dialoguepos].StartsWith("//")) && !paused)
        {
            if (textdone && ready)
            {
                ClearText();
                textbox.SetActive(false);
                alertbox.SetActive(false);
                RunNext();
            }
            else textdone = true;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                Pause(true);
            }
            else if(paused && settingsopen)
            {
                GameObject.Find("Canvas/Settings").GetComponent<Settings>().CloseMenu();
                settingsopen = false;
            }
            else if(paused && !settingsopen)
            {
                Pause(false);
            }
        }
    }
    public void ToggleDevmenu(bool forceopen = false)
    {
        if (PlayerPrefs.GetInt("devmenu", 0) == 0 || forceopen)
        {
            PlayerPrefs.SetInt("devmenu", 1);
            GameObject.Find("/Canvas/dev").transform.localScale = Vector3.one;
        }
        else
        {
            PlayerPrefs.SetInt("devmenu", 0);
            GameObject.Find("/Canvas/dev").transform.localScale = Vector3.zero;
        }
    }
    public void RunNext()
    {
        if (!ready) return;

        string[] line = story[dialoguepos].Split('|'); //line = nuvarande raden
        GameObject.Find("/Canvas/dev/varinfo").GetComponent<Text>().text = $"line = {dialoguepos}\naction = {story[dialoguepos].Split('|')[0]}\nready = {ready}\nscript: = {scriptName}\n\n{story[dialoguepos]}";
        line[0] = line[0].ToLower();
        if (line[0] == "" || line[0].StartsWith("//")) //blank/comment = ignore
        {
            dialoguepos++;
            RunNext();
        }

        else if (line[0] == "t") //textbox
        {
            fadeimage.SetActive(false);
            dialoguepos++;
            Action_Textbox.Run(line);
        }
        else if (line[0] == "alert") //general box
        {
            dialoguepos++;
            Action_Alert.Run(line);
        }
        else if (line[0] == "bg") //new background
        {
            dialoguepos++;
            Action_Background.Run(line);
        }
        else if (line[0] == "char") //move or create character
        {
            dialoguepos++;
            Action_Character.Run(line);
        }
        else if (line[0] == "del") //delete character
        {
            dialoguepos++;
            Action_DelCharacter.Run(line);
        }
        else if (line[0] == "question") //question
        {
            fadeimage.SetActive(false);
            ready = false;
            Action_Question.Run(line);
        }
        else if (line[0] == "loadstory") //open new story (no question)
        {
            Action_LoadScript.Run(line);
        }
        else if (line[0] == "wait") //delay
        {
            dialoguepos++;
            Action_Wait.Run(line);
        }
        else if (line[0] == "playmusic")
        {
            string arg = line[1];
            dialoguepos++;
            if(arg != musicplaying) dl.Ogg(music, $"file://{Helper.currentStoryPath}/Audio/{arg}.ogg", true);
            musicplaying = arg;
            RunNext();
        }
        else if (line[0] == "stopsounds")
        {
            dialoguepos++;
            StopSounds();
        }
        else if (line[0] == "playsfx")
        {
            dialoguepos++;
            dl.Ogg(SFX, $"file://{Helper.currentStoryPath}/Audio/{line[1]}.ogg", true);
            RunNext();
        }
        else if (line[0] == "finishgame")
        {
            StartCoroutine(StartCredits());
        }
        else
        {
            Helper.Alert($"An invalid action was found at line {dialoguepos} in script {PlayerPrefs.GetString("script")}.txt. \"{line[0]}\" is not a valid action.");
            Debug.LogError($"Visual Novel: Error at line {dialoguepos} in {Helper.currentStoryPath}/Dialogues/{PlayerPrefs.GetString("script")}.txt\nError: \"{line[0]}\" is not a valid action. Trying to skip...\nText: {string.Join("|", line)}");
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
    #endregion

    #region Text
    
    public string FillVars(string text) //Changes {1.name} to the name of person 1, and {0.nick} to the nickname of 0 etc
    {
        MatchCollection matches = Regex.Matches(text, @"{(\d+)\.(\w+)}"); //Matches {1.name} with "1" & "name" as a group

        foreach (Match match in matches)
        {
            string replace = "";
            if (match.Groups[2].Value == "name")
                replace = people[int.Parse(match.Groups[1].Value)].name;
            if (match.Groups[2].Value == "nick")
                replace = people[int.Parse(match.Groups[1].Value)].nick;

            text = new Regex("{(\\d+)\\.(\\w+)}").Replace(text, replace, 1); //Byter 
        }
        text = text.Replace("{path}", Helper.currentStoryPath);
        text = text.Replace("\\n", Environment.NewLine);
        return text;
    }
    private void ClearText()
    {
        commentPort.text = "";
        comment.text = "";
        nametag.text = "";
        nametagPort.text = "";
        alert.text = "";
        question.text = "";
        alt1t.text = "";
        alt2t.text = "";
    }

    #endregion

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
        if(Helper.currentStoryName == "OpenHouse") // Made to get data about a campaign for https://cybergymnasiet.se/
            Analytics.CustomEvent("program_picked", new Dictionary<string, object> {{ "program", options[select-1] }});
        #endregion openhouse
        Action_LoadScript.Load(options[select-1]);
        dropdownMenu.SetActive(false);
    }

    #endregion

    #region Generic
    public void StopSounds()
    {
        music.GetComponent<AudioSource>().Stop();
        SFX.GetComponent<AudioSource>().Stop();
        musicplaying = "none";
        RunNext();
    }

    #endregion

    #region UI
    public void Pause(bool n)
    {
        paused = n;
        pausemenu.SetActive(n);
    }
    public void OpenSettings()
    {
        settingsopen = true;
        GameObject x = Instantiate(SettingsMenu, Vector3.zero, new Quaternion(0, 0, 0, 0), GameObject.Find("Canvas").GetComponent<Transform>()) as GameObject;
        x.transform.localPosition = Vector3.zero;
        x.name = "Settings";
    }
    public void GoToMain()
    {
        SceneManager.LoadScene("menu");
    }
    public IEnumerator StartCredits(bool addStats = true) //Avslutar & återställer spelet och startar credits
    {
        StartCoroutine(FadeOut(music.GetComponent<AudioSource>(), 1.3f, 0));
        Credits.storypath = Helper.currentStoryPath;
        PlayerPrefs.DeleteKey("story");
        PlayerPrefs.DeleteKey("script");

        fadeimage.SetActive(true); //Open image that will fade (starts at opacity 0%)
        fadeimage.GetComponent<Animator>().Play("darken");
        yield return new WaitForSeconds(0.5f);
        if (addStats) Stats.Add(Stats.List.novelsfinished);
        SceneManager.LoadScene("credits");
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
    private void UpdateDesign()
    {
        StoryDesign design = StoryDesign.Get();

        if (File.Exists($"{Helper.currentStoryPath}/textbox.png"))
        {
            Debug.Log($"Found textbox at path {Helper.currentStoryPath}/textbox.png and will try to update...");
            dl.Image(textbox, $"{Helper.currentStoryPath}/textbox.png");
        }

        Color textColor = Colors.DarkPurple;
        ColorUtility.TryParseHtmlString($"#{design.textcolor.Replace("#", "")}", out textColor);
        comment.color = textColor;
        dropdownQ.color = textColor;
        question.color = textColor;
        alert.color = textColor;

        Color buttonColor = Colors.IngameBlue;
        ColorUtility.TryParseHtmlString($"#{design.questioncolor.Replace("#", "")}", out buttonColor);
        qbutton1.GetComponent<Image>().color = buttonColor;
        qbutton2.GetComponent<Image>().color = buttonColor;
        dropdownObject.GetComponent<Image>().color = buttonColor;
        dropdownBackground.GetComponent<Image>().color = Helper.ModifyColor(buttonColor, 1.05f);
        dropdownItemBackground.GetComponent<Image>().color = Helper.ModifyColor(buttonColor, 1.1f);


        Color buttonTextColor = Colors.UnityGray;
        ColorUtility.TryParseHtmlString($"#{design.questiontextcolor.Replace("#", "")}", out buttonTextColor);
        alt1t.color = buttonTextColor;
        alt2t.color = buttonTextColor;
    }
    #endregion

    #region Saving
    public IEnumerator SaveInfo() //just shows the user that the game got saved. simple huh?
    {
        saveinfo.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        saveinfo.SetActive(false);
    }
    #endregion
}