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

public class GameManager : MonoBehaviour
{
    public static bool ready = true; //Om skriptet är redo att gå till nästa rad
    public static bool dialogdone = false; //Om någon dialog håller på att skrivas ut är denna false
    public static int dialogpos = 0;
    public GameObject textbox;
    public GameObject alertbox;
    public GameObject portrait;
    public GameObject background;
    public GameObject music;
    public GameObject uwuwarning;
    public GameObject questionbox;
    public Text posobj; //Debug meny i canvas > dev
    public Text comment; //Texten som skrivs ut
    public Text personname; //Namntaggen i textboxar
    public Text alert;
    public Text question;
    public Text alt1t;
    public Text alt2t;
    public string story1;
    public string story2;
    public static string[] story;
    public Coroutine co;
    public Character[] people = ButtonCtrl.people;
    


    // Start is called before the first frame update
    void Start()
    {
        System.Random rnd = new System.Random();
        AudioListener.volume = PlayerPrefs.GetFloat("volume", 1f);
        string path = Application.dataPath;
        
        if(SceneManager.GetActiveScene().name == "game")
        {
            story = File.ReadAllLines($"{path}/Dialogues/{PlayerPrefs.GetString("story", "start")}.txt");
            PlayerPrefs.SetString("tempstory", PlayerPrefs.GetString("story", "start"));
        }
            
        if (SceneManager.GetActiveScene().name == "dev")
        {
            story = File.ReadAllLines($"{path}/Dialogues/start.txt");
            PlayerPrefs.SetString("tempstory", "start");

        }
            



    }

    // Update is called once per frame
    void Update()
    {

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

            else if (line[0] == "0") //textbox
            {
                Character talker = people[int.Parse(line[1])];
                string text = line[2].Replace("#", ",");
                text = FillVars(text);
                UnityEngine.Debug.Log($"{talker.name} says: {text}");
                ready = false;
                co = StartCoroutine(SpawnTextBox(talker, UwUTranslator(text)));
            }
            else if (line[0] == "1") //new background
            {
                StartCoroutine(ChangeBackground(line[1]));
                dialogpos++;

                if (line.Length > 2)
                    RemoveCharacters();
            }
            else if (line[0] == "2") //move or create character
            {
                int id = int.Parse(line[1]);
                string mood = line[2];
                float x = (float)Convert.ToDouble(line[3]);
                float y = (float)Convert.ToDouble(line[4]);
                int align = int.Parse(line[5]);
                StartCoroutine(CreateCharacter(id, mood, x, y, align));
                dialogpos++;
            }
            else if (line[0] == "3") //question
            {
                ready = false;
                string quest = line[1];
                string alt1 = line[2];
                story1 = line[3];
                string alt2 = line[4];
                story2 = line[5];
                Question(quest, alt1, alt2);
            }
            else if (line[0] == "4") //open new story (no question)
            {
                ToggleTextbox(false, 3);
                story = LoadStory(line[1]);
                dialogpos = 0; //återställ positionen - ny story!
                if (line.Length > 2)
                    RemoveCharacters();
            }
            else if (line[0] == "5") //general box
            {
                string text = line[1].Replace("#", ",");
                Debug.Log($"Alert: {text}");
                ready = false;
                StartCoroutine(SpawnAlert(UwUTranslator(text)));
            }
            else if (line[0] == "WAIT") //delay
            {
                ToggleTextbox(false, 3);
                ready = false;
                StartCoroutine(Delay(float.Parse(line[1])));
            }
            else if (line[0] == "PLAYMUSIC")
            {
                ToggleTextbox(false, 3);
                StartCoroutine(PlayMusic(line[1]));
                dialogpos++;
            }
            else if(line[0] == "STOPSOUNDS")
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
            #endregion

        }
        else if (!dialogdone && !ready && Input.GetKeyUp("space"))
        {
            dialogdone = true;
        }
        else if (dialogdone && !ready && Input.GetKeyUp("space")) //gå vidare från dialog
        {
            StopCoroutine(co);
            dialogpos++;
            ready = true;
        }
            
        //debug info
        posobj.text = $"line = {dialogpos}\naction = {line[0]}\nready = {ready}\ndialogdone = {dialogdone}\nstory = {PlayerPrefs.GetString("tempstory","start")}\n\n{story[dialogpos]}";
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
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{Application.dataPath}/Characters/{talker.name.ToLower()}port.png");
        yield return uwr.SendWebRequest();
        var texture = DownloadHandlerTexture.GetContent(uwr);
        portrait.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        personname.text = talker.name;

        if(PlayerPrefs.GetFloat("delay",0.04f) > 0.001f) //ifall man stängt av typing speed är denna onödig
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
        dialogpos = 0;
        string[] stories = { story1, story2 };
        story = LoadStory(stories[id-1]);
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

    IEnumerator ChangeBackground(string bg) //ID 1
    {
        ToggleTextbox(false,3);
        ready = false;
        Debug.Log($"New background loaded: {bg}");
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{Application.dataPath}/Backgrounds/{bg}.png");
        yield return uwr.SendWebRequest();
        var texture = DownloadHandlerTexture.GetContent(uwr);
        background.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        ready = true;
    }

    string[] LoadStory(string story) //ID 4
    {
        ToggleTextbox(false,3);
        Debug.Log($"New story loaded: {story}");
        PlayerPrefs.SetString("story", story);
        PlayerPrefs.SetString("tempstory", story);
        return File.ReadAllLines($"{Application.dataPath}/Dialogues/{story}.txt");
    }

    IEnumerator CreateCharacter(int id, string mood, float x, float y, int align) //ID 2
    {
        if(GameObject.Find($"{people[id].name.ToLower()}") == null) //karaktär finns ej
        {
            //ladda in filen som texture
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{Application.dataPath}/Characters/{people[id].name.ToLower()}{mood}.png");
            yield return uwr.SendWebRequest();
            var texture = DownloadHandlerTexture.GetContent(uwr);

            //skapa gameobj
            GameObject character = new GameObject($"{people[id].name.ToLower()}");
            character.gameObject.tag = "character";
            SpriteRenderer renderer = character.AddComponent<SpriteRenderer>();
            renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            //sätt size + pos
            character.transform.position = new Vector3(x, y, -1f);
            character.transform.localScale = new Vector3(0.58f*align, 0.58f, 0.6f);
        }
        else //karaktär finns
        {
            //ändra pos
            GameObject character = GameObject.Find($"{people[id].name.ToLower()}");
            character.transform.position = new Vector3(x, y, -1f);
            character.transform.localScale = new Vector3(0.58f*align, 0.58f, 0.6f);

            //ändra mood
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{Application.dataPath}/Characters/{people[id].name.ToLower()}{mood}.png");
            yield return uwr.SendWebRequest();
            var texture = DownloadHandlerTexture.GetContent(uwr);
            character.GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
        

    }
    IEnumerator PlayMusic(string sound) //Musik ligger på bakgrunden
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip($"file://{Application.dataPath}/Audio/{sound}.ogg", AudioType.OGGVORBIS))
        {
            yield return uwr.SendWebRequest();
            background.GetComponent<AudioSource>().clip = DownloadHandlerAudioClip.GetContent(uwr);
            background.GetComponent<AudioSource>().Play();
        }
    }
    IEnumerator PlaySoundEffect(string sound) //Ljudeffekter ligger på kameran
    {
        using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip($"file://{Application.dataPath}/Audio/{sound}.ogg", AudioType.OGGVORBIS))
        {
            yield return uwr.SendWebRequest();
            music.GetComponent<AudioSource>().clip = DownloadHandlerAudioClip.GetContent(uwr);
            music.GetComponent<AudioSource>().Play();
        }
    }
    public void StopSounds()
    {
        background.GetComponent<AudioSource>().Stop();
        music.GetComponent<AudioSource>().Stop();
    }
    void ToggleTextbox(bool shown, int id) //ID0 ALERT, ID1 TEXT, ELSE EVERYTHING
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
    string FillVars(string text)
    {
        MatchCollection matches = Regex.Matches(text, @"{(\d+)\.(\w+)}");

        foreach (Match match in matches)
        {
            Debug.Log(match);

        }

        return text;
    }
}