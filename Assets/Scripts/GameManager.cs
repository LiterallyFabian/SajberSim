using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using static PersonClass;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;
using System.Globalization;

public class GameManager : MonoBehaviour
{
    public static bool ready = true; //Om skriptet är redo att gå till nästa rad
    public static bool dialogdone = false; //Om någon dialog håller på att skrivas ut är denna false
    public static int dialogpos = 0;
    public static float charsize = 0.8f;
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
    public GameObject skiptutorial;
    public GameObject dropdownObject;
    public GameObject dropdownMenu;
    public static bool paused = false;
    public Text posobj; //Debug meny i canvas > dev
    public Text comment; //Texten som skrivs ut
    public Text personname; //Namntaggen i textboxar
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
    



    // Start is called before the first frame update
    void Start()
    {
        lang.NumberDecimalSeparator = "."; 

        paused = false;
        dialogpos = 0;
        ready = true;
        dialogdone = false;
        Cursor.visible = true;
        System.Random rnd = new System.Random();
        AudioListener.volume = PlayerPrefs.GetFloat("volume", 1f);
        string path = Application.dataPath;

        if (SceneManager.GetActiveScene().name == "game")
        {
            story = File.ReadAllLines($"{path}/Modding/Dialogues/{PlayerPrefs.GetString("story", "start")}.txt");
            PlayerPrefs.SetString("tempstory", PlayerPrefs.GetString("story", "start"));
        }

        if (SceneManager.GetActiveScene().name == "dev")
        {
            story = File.ReadAllLines($"{path}/Modding/Dialogues/start.txt");
            PlayerPrefs.SetString("tempstory", "start");

        }




    }

    // Update is called once per frame
    void Update()
    {
        //fixes tutorial buttonweird
        if (SceneManager.GetActiveScene().name == "game")
            if (PlayerPrefs.GetString("story", "start") == "start")
                skiptutorial.SetActive(true);
            else skiptutorial.SetActive(false);

        if (PlayerPrefs.GetInt("uwu", 0) == 1) uwuwarning.SetActive(true);
        else uwuwarning.SetActive(false);

        string[] line = story[dialogpos].Split('|'); //line = nuvarande raden
        if (ready)
        {
            #region checks & run function
            if (line[0] == "" || line[0].StartsWith("//")) //blank/comment = ignore
            {
                dialogpos++;
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
                    ready = false;
                    co = StartCoroutine(SpawnTextBox(talker, UwUTranslator(text)));
            }
            else if (line[0] == "ALERT") //general box
            {
                string text = FillVars(line[1]);
                Debug.Log($"Alert: {text}");
                ready = false;
                StartCoroutine(SpawnAlert(UwUTranslator(text)));
            }
            else if (line[0] == "BG") //new background
            {
                StartCoroutine(ChangeBackground(line[1], background));
                dialogpos++;

                if (line.Length > 2)
                    RemoveCharacters();
            }
            else if (line[0] == "CHAR") //move or create character
            {
                string name = "";
                if (int.TryParse(line[1], out int xd)) name = people[int.Parse(line[1])].name; //ID --> Name if possible, else name
                else name = line[1];

                string mood = line[2];
                float x = (float)Convert.ToDouble(line[3], lang);
                float y = (float)Convert.ToDouble(line[4], lang);
                int align = int.Parse(line[5]);
                StartCoroutine(CreateCharacter(name.ToLower(), mood, x, y, align));
                dialogpos++;
            }
            else if(line[0] == "DEL") //delete character
            {
                string name = "";
                if (int.TryParse(line[1], out int xd)) name = people[int.Parse(line[1])].name; //ID --> Name if possible, else name
                else name = line[1];

                StartCoroutine(CreateCharacter(name.ToLower(), "neutral", 0, 200, 1));
                dialogpos++;
            }
            else if (line[0] == "QUESTION") //question
            {
                ready = false;
                if(line.Length == 6) //Normal 2 alt questions
                {
                    string quest = line[1];
                    string alt1 = line[2];
                    story1 = line[3];
                    string alt2 = line[4];
                    story2 = line[5];
                    Question(quest, alt1, alt2);
                }
                else //More questions - dropdown menu
                {
                    QuestionDD(line);
                }
            }
            else if (line[0] == "LOADSTORY") //open new story (no question)
            {
                ToggleTextbox(false, 3);
                story = LoadStory(line[1]);
                dialogpos = 0; //återställ positionen - ny story!
                if (line.Length > 2)
                    RemoveCharacters();
            }
            else if (line[0] == "OPENSCENE") //delay
            {
                SceneManager.LoadScene(line[1]);
            }
            else if (line[0] == "WAIT") //delay
            {
                ToggleTextbox(false, 3);
                ready = false;
                StartCoroutine(Delay((float)Convert.ToDouble(line[1], lang)));
            }
            else if (line[0] == "PLAYMUSIC")
            {
                ToggleTextbox(false, 3);
                StartCoroutine(PlayMusic(line[1]));
                dialogpos++;
            }
            else if (line[0] == "STOPSOUNDS")
            {
                StopSounds();
                dialogpos++;
            }
            else if (line[0] == "PLAYSFX")
            {
                ToggleTextbox(false, 3);
                StartCoroutine(PlaySoundEffect(line[1]));
                dialogpos++;
            }
            else if (line[0] == "FINISHGAME")
            {
                StartCoroutine(StartCredits());
            }
            #endregion

        }
        else if (!dialogdone && !ready && (Input.GetKeyUp("space") || Input.GetKeyUp(KeyCode.Return) || Input.GetMouseButtonDown(0)))
        {
            dialogdone = true;
        }
        else if (dialogdone && !ready && (Input.GetKeyUp("space") || Input.GetKeyUp(KeyCode.Return) || Input.GetMouseButtonDown(0)) && (line[0] == "ALERT" || line[0] == "T") && !paused) //gå vidare från dialog
        {
            StopCoroutine(co);
            dialogpos++;
            ready = true;
        }

        //debug info
        posobj.text = $"line = {dialogpos}\naction = {line[0]}\nready = {ready}\ndialogdone = {dialogdone}\nstory = {PlayerPrefs.GetString("tempstory", "start")}\n\n{story[dialogpos]}";
    }
    public void QuestionDD(string[] line)
    {
        dropdownObject.GetComponent<Dropdown>().ClearOptions();
        dropdownObject.GetComponent<Dropdown>().AddOptions(new List<string> {" "});
        ToggleTextbox(false,3);
        List<string> options = new List<string>();
        for (int i = 2; i < line.Length; i = i+2) 
        {
            options.Add(line[i]);
        }
        dropdownQ.text = line[1];
        dropdownObject.GetComponent<Dropdown>().AddOptions(options);
        dropdownMenu.SetActive(true);
    }
    public void ChangeStoryFromDD(int select)
    {
        string[] line = story[dialogpos].Split('|');
        List<string> options = new List<string>();
        for (int i = 3; i < line.Length; i = i + 2)
        {
            options.Add(line[i]);
        }
        #region openhouse
        Analytics.CustomEvent("program_picked", new Dictionary<string, object>
        {
            { "program", options[select-1] }
        });
        #endregion openhouse
        story = LoadStory(options[select-1]);
        dialogpos = 0;
        dropdownMenu.SetActive(false);
        ready = true;
    }
    public void SkipTutorial()
    {
        story = LoadStory("intro");
        dialogpos = 0;
        dialogdone = false;
        ready = true;
    }
    IEnumerator Delay(float time) //ID 7
    {
        yield return new WaitForSeconds(time);
        dialogpos++;
        ready = true;
    }
    IEnumerator SpawnTextBox(Character talker, string target) //ID 0
    {
        dialogdone = false;
        ToggleTextbox(true, 1);
        ToggleTextbox(false, 0);
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{Application.dataPath}/Modding/Characters/{talker.name.ToLower()}port.png");
        yield return uwr.SendWebRequest();
        var texture = DownloadHandlerTexture.GetContent(uwr);
        portrait.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        personname.text = talker.name;

        if (PlayerPrefs.GetFloat("delay", 0.04f) > 0.001f) //ifall man stängt av typing speed är denna onödig
        {
            string written = target[0].ToString(); //written = det som står hittills

            for (int i = 1; i < target.Length; i++)
            {
                written = written + target[i];
                yield return new WaitForSeconds(PlayerPrefs.GetFloat("delay", 0.04f));
                if (dialogdone) //avbryt och skriv hela
                {
                    comment.text = target;
                    dialogdone = true;
                    break;
                }
                comment.text = written;
            }
        }

        comment.text = target;
        dialogdone = true;
    }

    void Question(string text, string alt1, string alt2)
    {
        dialogdone = false;
        ToggleTextbox(true, 2);
        ToggleTextbox(false, 1);
        ToggleTextbox(false, 0);
        question.text = text;
        alt1t.text = alt1;
        alt2t.text = alt2;
    }
    public void AnswerQuestion(int id)
    {
        DataTracker.ReportQuestion(question.text, id);
        dialogpos = 0;
        string[] stories = { story1, story2 };
        story = LoadStory(stories[id - 1]);
        #region openhouse
        Analytics.CustomEvent("program_picked", new Dictionary<string, object>
        {
            { "program", stories[id-1] }
        });
        #endregion openhouse
        ready = true;
    }
    IEnumerator SpawnAlert(string target) //ID 0
    {
        dialogdone = false;
        ToggleTextbox(false, 1);
        ToggleTextbox(true, 0);
        string written = target[0].ToString(); //written = det som står hittills

        for (int i = 1; i < target.Length; i++)
        {
            written = written + target[i];
            yield return new WaitForSeconds(PlayerPrefs.GetFloat("delay", 0.04f));
            if (dialogdone) //avbryt och skriv hela
            {
                alert.text = target;
                dialogdone = true;
                break;
            }
            alert.text = written;
        }
        alert.text = target;
        dialogdone = true;
    }
    public static void RemoveCharacters()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("character");

        for (var i = 0; i < gameObjects.Length; i++)
            Destroy(gameObjects[i]);
    }
    public string UwUTranslator(string text)
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

    IEnumerator ChangeBackground(string bg, GameObject item) //ID 1
    {
        Texture2D texture;
        ToggleTextbox(false, 3);
        ready = false;
        Debug.Log($"New background loaded: {bg}");
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{Application.dataPath}/Modding/Backgrounds/{bg}.png"))
        {
            yield return uwr.SendWebRequest();
            if (uwr.isNetworkError)
            {
                Debug.LogError("Kunde ej ladda ner bakgrund");
            }
            else
            {
                texture = DownloadHandlerTexture.GetContent(uwr);
                item.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            
        }
        
        ready = true;
    }

    string[] LoadStory(string story) //ID 4
    {
        ToggleTextbox(false, 3);
        Debug.Log($"New story loaded: {story}");
        PlayerPrefs.SetString("story", story);
        PlayerPrefs.SetString("tempstory", story);
        StartCoroutine(SaveInfo());
        return File.ReadAllLines($"{Application.dataPath}/Modding/Dialogues/{story}.txt");
    }

    IEnumerator CreateCharacter(string name, string mood, float x, float y, int align) //ID 2
    {
        if (GameObject.Find(name) == null) //karaktär finns ej
        {
            //ladda in filen som texture
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{Application.dataPath}/Modding/Characters/{name}{mood}.png");
            yield return uwr.SendWebRequest();
            var texture = DownloadHandlerTexture.GetContent(uwr);

            //skapa gameobj
            GameObject character = new GameObject(name);
            character.gameObject.tag = "character";
            SpriteRenderer renderer = character.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            //sätt size + pos
            character.transform.position = new Vector3(x, y, -1f);
            character.transform.localScale = new Vector3(charsize * align, charsize, 0.6f);
        }
        else //karaktär finns
        {
            //ändra pos
            GameObject character = GameObject.Find(name);
            character.transform.position = new Vector3(x, y, -1f);
            character.transform.localScale = new Vector3(charsize * align, charsize, 0.6f);

            //ändra mood
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{Application.dataPath}/Modding/Characters/{name}{mood}.png");
            yield return uwr.SendWebRequest();
            var texture = DownloadHandlerTexture.GetContent(uwr);
            character.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }


    }
    IEnumerator PlayMusic(string sound) //Musik ligger på "music"
    {
        if (musicplaying != sound) //spela om den inte redan körs
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip($"file://{Application.dataPath}/Modding/Audio/{sound}.ogg", AudioType.OGGVORBIS))
        {
            yield return uwr.SendWebRequest();
            music.GetComponent<AudioSource>().clip = DownloadHandlerAudioClip.GetContent(uwr);
            music.GetComponent<AudioSource>().Play();
            musicplaying = sound;
        }
    }
    IEnumerator PlaySoundEffect(string sound) //Ljudeffekter ligger på "SFX"
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip($"file://{Application.dataPath}/Modding/Audio/{sound}.ogg", AudioType.OGGVORBIS))
        {
            yield return uwr.SendWebRequest();
            SFX.GetComponent<AudioSource>().clip = DownloadHandlerAudioClip.GetContent(uwr);
            SFX.GetComponent<AudioSource>().Play();
        }
    }
    public void StopSounds()
    {
        background.GetComponent<AudioSource>().Stop();
        music.GetComponent<AudioSource>().Stop();
        musicplaying = "none";
    }
    void ToggleTextbox(bool shown, int id) //ID0 ALERT, ID1 TEXT, ID2 QUESTION, ELSE EVERYTHING
    {
        if (id == 0)
            alertbox.SetActive(shown);
        else if (id == 1)
            textbox.SetActive(shown);
        else if (id == 2)
            questionbox.SetActive(shown);
        else
        {
            questionbox.SetActive(shown);
            textbox.SetActive(shown);
            alertbox.SetActive(shown);
        }
        if (!shown)
        {
            //om man tar bort textboxen så försvinner texten
            comment.text = "";
            alert.text = "";
            question.text = "";
            alt1t.text = "";
            alt2t.text = "";
        }
    }
    public IEnumerator StartCredits() //Avslutar & återställer spelet och startar credits
    {
        StartCoroutine(FadeOut(background.GetComponent<AudioSource>(), 1.3f, 0));
        PlayerPrefs.DeleteKey("story");

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
    string FillVars(string text) //Changes {1.name} to the name of person 1, and {0.nick} to the nickname of 0 etc
    {
        MatchCollection matches = Regex.Matches(text, @"{(\d+)\.(\w+)}"); //Matches {1.name} with "1" & "name" as a group

        foreach (Match match in matches)
        {
            string replace ="";
            if (match.Groups[2].Value == "name")
                replace = people[int.Parse(match.Groups[1].Value)].name;
            if (match.Groups[2].Value == "nick")
                replace = people[int.Parse(match.Groups[1].Value)].nick;

            text = new Regex("{(\\d+)\\.(\\w+)}").Replace(text, replace,1); //Byter 
        }

        return text;
    }
    public IEnumerator SaveInfo() //just shows the user that the game got saved. simple huh?
    {
        saveinfo.SetActive(true);
        yield return new WaitForSeconds(2.5f);
        saveinfo.SetActive(false);
    }
}