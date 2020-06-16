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
    public GameObject skiptutorial;
    public GameObject dropdownObject;
    public GameObject dropdownMenu;
    public GameObject SettingsMenu;
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
    private string storyPath;

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
        storyPath = $"{Application.dataPath}/Story/{PlayerPrefs.GetString("story")}";

        story = File.ReadAllLines($"{storyPath}/Dialogues/{PlayerPrefs.GetString("script", "start")}.txt");
        PlayerPrefs.SetString("tempstory", PlayerPrefs.GetString("story", "start"));

        if (File.Exists($"{storyPath}/textbox.png"))
        {
            Debug.Log($"Found textbox at path {storyPath}/textbox.png and will try to update...");
            dl.Image(textbox, $"{storyPath}/textbox.png");
        }

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
        //fixes tutorial buttonweird
        if (SceneManager.GetActiveScene().name == "game")
            if (PlayerPrefs.GetString("story", "start") == "start")
                skiptutorial.SetActive(true);
            else skiptutorial.SetActive(false);

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

        if (line[0] == "" || line[0].StartsWith("//")) //blank/comment = ignore
        {
            dialoguepos++;
            RunNext();
        }

        else if (line[0] == "T") //textbox
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
        else if (line[0] == "ALERT") //general box
        {
            string text = FillVars(line[1]);
            Debug.Log($"Alert: {text}");
            StartCoroutine(SpawnAlert(UwUTranslator(text)));
        }
        else if (line[0] == "BG") //new background
        {
            if (line.Length > 2) RemoveCharacters();
            dialoguepos++;
            ChangeBackground(line[1]);
        }
        else if (line[0] == "CHAR") //move or create character
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
        else if (line[0] == "DEL") //delete character
        {
            string name = "";
            if (int.TryParse(line[1], out int xd)) name = people[int.Parse(line[1])].name; //ID if possible, else name
            else name = line[1];
            dialoguepos++;
            Destroy(UnityEngine.GameObject.Find(name.ToLower()));
            RunNext();
        }
        else if (line[0] == "QUESTION") //question
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
        else if (line[0] == "LOADSTORY") //open new story (no question)
        {
            LoadScript(line[1]);
            if (line.Length > 2)
                RemoveCharacters();
        }
        else if (line[0] == "OPENSCENE") //delay
        {
            SceneManager.LoadScene(line[1]);
        }
        else if (line[0] == "WAIT") //delay
        {
            StartCoroutine(Delay((float)Convert.ToDouble(line[1], lang)));
        }
        else if (line[0] == "PLAYMUSIC")
        {
            string arg = line[1];
            dialoguepos++;
            if(arg != musicplaying) dl.Ogg(music, $"file://{storyPath}/Audio/{arg}.ogg", true);
            musicplaying = arg;
            RunNext();
        }
        else if (line[0] == "STOPSOUNDS")
        {
            dialoguepos++;
            StopSounds();
        }
        else if (line[0] == "PLAYSFX")
        {
            dialoguepos++;
            dl.Ogg(SFX, $"file://{storyPath}/Audio/{line[1]}.ogg", true);
            RunNext();
        }
        else if (line[0] == "FINISHGAME")
        {
            StartCoroutine(StartCredits());
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
            dl.Sprite(character, $"file://{storyPath}/Characters/{name}{mood}.png");

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
            dl.Sprite(character, $"file://{storyPath}/Characters/{name}{mood}.png");
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
        dl.Image(portrait, $"file://{storyPath}/Characters/{talker.name.ToLower()}port.png");
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
        textdone = false;
        questionbox.SetActive(true);
        question.text = text;
        alt1t.text = alt1;
        alt2t.text = alt2;
    }
    public void AnswerQuestion(int id)
    {
        DataTracker.ReportQuestion(question.text, id);
        string[] stories = { story1, story2 };
        LoadScript(stories[id - 1]);
        #region openhouse
        if (PlayerPrefs.GetString("story") == "OpenHouse")
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
        string[] line = story[dialoguepos].Split('|');
        List<string> options = new List<string>();
        for (int i = 3; i < line.Length; i = i + 2)
        {
            options.Add(line[i]);
        }
        #region openhouse
        if(PlayerPrefs.GetString("story") == "OpenHouse")
        Analytics.CustomEvent("program_picked", new Dictionary<string, object> {{ "program", options[select-1] }});
        #endregion openhouse
        LoadScript(options[select-1]);
        dropdownMenu.SetActive(false);
    }

    #endregion

    #region Generic
    public void SkipTutorial()
    {
        LoadScript("intro");
        textdone = false;
    }
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
        dl.Image(background, $"file://{storyPath}/Backgrounds/{bg}.png");
        RunNext();
    }

    public void LoadScript(string storyx)
    {
        string path = $"{storyPath}/Dialogues/{storyx}.txt";
        if (!File.Exists(path))
        {
            Debug.LogError($"Tried to start non-existing story: {path}");
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
        StartCoroutine(FadeOut(background.GetComponent<AudioSource>(), 1.3f, 0));
        PlayerPrefs.DeleteKey("story");
        PlayerPrefs.DeleteKey("script");

        fadeimage.SetActive(true); //Open image that will fade (starts at opacity 0%)

        for (float i = 0; i <= 1; i += Time.deltaTime / 1.5f) //Starts fade, load scene when done
        {
            fadeimage.GetComponent<Image>().color = new Color(0, 0, 0, i);
            if (i > 0.5f) Cursor.visible = false;
            yield return null;
        }

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