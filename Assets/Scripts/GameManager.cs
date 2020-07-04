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
/// <summary>
/// Needs a huge rewrite, but yeah this script runs the entire visual novel scene
/// </summary>
public class GameManager : MonoBehaviour
{
    public static bool ready = true; // If the game is ready to go to the next line (eg delays, downloads)
    public static bool textdone = false; // False if a text is currently writing out
    public static int dialoguepos = 0;
    public static float charactersize = 0.8f; // This is the size of game characters, not letters
    public GameObject textbox;
    public GameObject alertbox;
    public GameObject portrait;
    public GameObject background;
    public GameObject music;
    public GameObject uwuwarning;
    public GameObject questionbox;
    public GameObject fadeimage;
    public GameObject saveinfo;
    public GameObject SFX;
    public GameObject pausemenu;
    public GameObject dropdownObject;
    public GameObject dropdownMenu;
    public GameObject dropdownItemBackground;
    public GameObject dropdownBackground;
    public GameObject SettingsMenu;
    public GameObject qbutton1;
    public GameObject qbutton2;
    private bool settingsopen = false;
    public static bool paused = false;
    public Text comment; //The normal text
    public Text personname; //The nametag
    public Text alert;
    public Text question;
    public Text alt1t;
    public Text alt2t;
    public Text dropdownQ;
    public string story1;
    public string story2;
    public static string[] story;
    public Coroutine co;
    public Character[] people = ButtonCtrl.people;
    public string musicplaying = "none";
    NumberFormatInfo lang = new NumberFormatInfo();
    Download dl;

    public static string storyName;
    public static string storyAuthor;




    // Start is called before the first frame update
    void Start()
    {
        dl = new GameObject("downloadobj").AddComponent<Download>();
        lang.NumberDecimalSeparator = "."; 

        paused = false;
        dialoguepos = 0;
        ready = true;
        textdone = false;
        Cursor.visible = true;
        AudioListener.volume = PlayerPrefs.GetFloat("volume", 1f);

        story = File.ReadAllLines($"{Helper.currentStoryPath}/Dialogues/{PlayerPrefs.GetString("script", "start")}.txt");
        PlayerPrefs.SetString("tempstory", PlayerPrefs.GetString("story", "start"));

        UnityEngine.Debug.Log($"Entered visual novel. Details:\nName: {Helper.currentStoryName}\nPath: {Helper.currentStoryPath}");
        UpdateDesign();
        RunNext();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) //toggles debug stuff
        {
            if(PlayerPrefs.GetInt("devmenu", 0) == 0)
            {
                PlayerPrefs.SetInt("devmenu", 1);
                GameObject.Find("/Canvas/dev").transform.localScale = Vector3.one; // might be a dumb approach but can't use .SetActive together with .Find
            }
            else
            {
                PlayerPrefs.SetInt("devmenu", 0);
                GameObject.Find("/Canvas/dev").transform.localScale = Vector3.zero;
            }
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
    void RunNext()
    {
        if (!ready) return;

        string[] line = story[dialoguepos].Split('|'); //line = nuvarande raden
        GameObject.Find("/Canvas/dev/varinfo").GetComponent<Text>().text = $"line = {dialoguepos}\naction = {story[dialoguepos].Split('|')[0]}\nready = {ready}\nstory = {PlayerPrefs.GetString("tempstory", "start")}\n\n{story[dialoguepos]}";
        line[0] = line[0].ToLower();
        if (line[0] == "" || line[0].StartsWith("//")) //blank/comment = ignore
        {
            dialoguepos++;
            RunNext();
        }

        else if (line[0] == "t") //textbox
        {
            Character talker = people[0];
            if (int.TryParse(line[1], out int x))
                talker = people[int.Parse(line[1])];
            else
                talker = new Character(line[1], "", 0);

            string text = FillVars(line[2]);
            UnityEngine.Debug.Log($"{talker.name} says: {text}");
            co = StartCoroutine(SpawnTextBox(talker, UwUTranslator(text)));
            dialoguepos++;
        }
        else if (line[0] == "alert") //general box
        {
            string text = FillVars(line[1]);
            Debug.Log($"Alert: {text}");
            StartCoroutine(SpawnAlert(UwUTranslator(text)));
        }
        else if (line[0] == "bg") //new background
        {
            if (line.Length > 2) RemoveCharacters();
            dialoguepos++;
            ChangeBackground(line[1]);
        }
        else if (line[0] == "char") //move or create character
        {
            string name = "";
            if (int.TryParse(line[1], out int xd)) name = people[int.Parse(line[1])].name; //ID if possible, else name
            else name = line[1];

            string mood = line[2];
            float x = (float)Convert.ToDouble(line[3], lang);
            float y = (float)Convert.ToDouble(line[4], lang);
            int align = int.Parse(line[5]);
            dialoguepos++;
            CreateCharacter(name.ToLower(), mood, x, y, align);
        }
        else if (line[0] == "del") //delete character
        {
            string name = "";
            if (int.TryParse(line[1], out int xd)) name = people[int.Parse(line[1])].name; //ID if possible, else name
            else name = line[1];
            dialoguepos++;
            Destroy(UnityEngine.GameObject.Find(name.ToLower()));
            RunNext();
        }
        else if (line[0] == "question") //question
        {
            ready = false;
            if (line.Length == 6) //Normal 2 alt questions
            {
                string quest = line[1];
                string alt1 = line[2];
                story1 = line[3];
                string alt2 = line[4];
                story2 = line[5];
                OpenQuestion(quest, alt1, alt2);
            }
            else //More questions - dropdown menu
            {
                OpenQuestionDD(line);
            }
        }
        else if (line[0] == "loadstory") //open new story (no question)
        {
            LoadScript(line[1]);
            if (line.Length > 2)
                RemoveCharacters();
        }
        else if (line[0] == "openscene") //delay
        {
            SceneManager.LoadScene(line[1]);
        }
        else if (line[0] == "wait") //delay
        {
            StartCoroutine(Delay((float)Convert.ToDouble(line[1], lang)));
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
    private void CreateCharacter(string name, string mood, float x, float y, int align) //ID 2
    {
        if (GameObject.Find(name) == null) //karaktär finns ej
        {
            //skapa gameobj
            GameObject character = new GameObject(name);
            character.gameObject.tag = "character";
            character.AddComponent<SpriteRenderer>();
            dl.Sprite(character, $"file://{Helper.currentStoryPath}/Characters/{name}{mood}.png");

            //sätt size + pos
            character.transform.position = new Vector3(x, y, -1f);
            character.transform.localScale = new Vector3(charactersize * align, charactersize, 0.6f);
        }
        else //karaktär finns
        {
            //ändra pos
            GameObject character = GameObject.Find(name);
            character.transform.position = new Vector3(x, y, -1f);
            character.transform.localScale = new Vector3(charactersize * align, charactersize, 0.6f);

            //ändra mood
            dl.Sprite(character, $"file://{Helper.currentStoryPath}/Characters/{name}{mood}.png");
        }
        RunNext();
    }
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
    private IEnumerator SpawnTextBox(Character talker, string target) //ID 0
    {
        textdone = false;
        textbox.SetActive(true);
        dl.Image(portrait, $"file://{Helper.currentStoryPath}/Characters/{talker.name.ToLower()}port.png");
        personname.text = talker.name;

        if (PlayerPrefs.GetFloat("delay", 0.04f) > 0.001f) //ifall man stängt av typing speed är denna onödig
        {
            string written = target[0].ToString(); //written = det som står hittills

            for (int i = 1; i < target.Length; i++)
            {
                written = written + target[i];
                yield return new WaitForSeconds(PlayerPrefs.GetFloat("delay", 0.04f));
                if (textdone) //avbryt och skriv hela
                {
                    comment.text = target;
                    textdone = true;
                    break;
                }
                comment.text = written;
            }
        }
        comment.text = target;
        textdone = true;
    }

    private IEnumerator SpawnAlert(string target) //ID 0
    {
        textdone = false;
        alertbox.SetActive(true);
        string written = target[0].ToString(); //written = det som står hittills

        for (int i = 1; i < target.Length; i++)
        {
            written = written + target[i];
            yield return new WaitForSeconds(PlayerPrefs.GetFloat("delay", 0.04f));
            if (textdone) //avbryt och skriv hela
            {
                alert.text = target;
                textdone = true;
                break;
            }
            alert.text = written;
        }
        alert.text = target;
        textdone = true;
    }
    private string UwUTranslator(string text)
    {
        if (PlayerPrefs.GetInt("uwu", 0) == 0) return text;
        else
        {
            text = text.Replace('l', 'w');
            text = text.Replace('r', 'w');
            text = text.Replace(" f", " f-f");
            text = text.Replace('L', 'W');
            text = text.Replace('R', 'W');
            text = text.Replace(" F", " F-F");
            if (UnityEngine.Random.Range(0, 10) == 0) text = text + " :3";
        }
        return text;
    }
    private string FillVars(string text) //Changes {1.name} to the name of person 1, and {0.nick} to the nickname of 0 etc
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
        comment.text = "";
        alert.text = "";
        question.text = "";
        alt1t.text = "";
        alt2t.text = "";
    }

    #endregion

    #region Questions
    private void OpenQuestion(string text, string alt1, string alt2)
    {
        fadeimage.SetActive(false);
        textdone = false;
        questionbox.SetActive(true);
        question.text = text;
        alt1t.text = alt1;
        alt2t.text = alt2;
    }
    public void AnswerQuestion(int id)
    {
        Stats.Add(Stats.List.decisionsmade);
        string[] stories = { story1, story2 };
        LoadScript(stories[id - 1]);
        #region openhouse
        if (Helper.currentStoryName == "OpenHouse") // Made to get data about a campaign for https://cybergymnasiet.se/
            Analytics.CustomEvent("program_picked", new Dictionary<string, object> { { "program", stories[id - 1] } });
        #endregion openhouse
        questionbox.SetActive(false);
    }
    private void OpenQuestionDD(string[] line) 
    {
        dropdownObject.GetComponent<Dropdown>().ClearOptions();
        dropdownObject.GetComponent<Dropdown>().AddOptions(new List<string> {" "}); //adds preselected blank
        List<string> options = new List<string>();
        for (int i = 2; i < line.Length; i = i+2) 
        {
            options.Add(line[i]);
        }
        dropdownQ.text = line[1];
        dropdownObject.GetComponent<Dropdown>().AddOptions(options);
        dropdownMenu.SetActive(true);
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
        LoadScript(options[select-1]);
        dropdownMenu.SetActive(false);
    }

    #endregion

    #region Generic
    private IEnumerator Delay(float time) //ID 7
    {
        ready = false;
        yield return new WaitForSeconds(time);
        dialoguepos++;
        ready = true;
        RunNext();
    }

    private void ChangeBackground(string bg) //ID 1
    {
        dl.Image(background, $"file://{Helper.currentStoryPath}/Backgrounds/{bg}.png");
        RunNext();
    }

    public void LoadScript(string storyx)
    {
        string path = $"{Helper.currentStoryPath}/Dialogues/{storyx}.txt";
        if (!File.Exists(path))
        {
            Debug.LogError($"Visual Novel: Tried to start non-existing story: {path}");
            return;
        }
        Debug.Log($"New story loaded: {storyx}");
        PlayerPrefs.SetString("script", storyx);
        StartCoroutine(SaveInfo());
        dialoguepos = 0;
        story = File.ReadAllLines(path);
        ready = true;
        RunNext();
    }

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
    private IEnumerator StartCredits() //Avslutar & återställer spelet och startar credits
    {
        StartCoroutine(FadeOut(music.GetComponent<AudioSource>(), 1.3f, 0));
        Credits.storypath = Helper.currentStoryPath;
        PlayerPrefs.DeleteKey("story");
        PlayerPrefs.DeleteKey("script");

        fadeimage.SetActive(true); //Open image that will fade (starts at opacity 0%)
        fadeimage.GetComponent<Animator>().Play("darken");
        yield return new WaitForSeconds(0.5f);
        Stats.Add(Stats.List.novelsfinished);
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
        StoryDesign design = Helper.GetDesign();

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