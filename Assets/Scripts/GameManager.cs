﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using static PersonClass;

public class GameManager : MonoBehaviour
{
    public bool ready = true; //Om skriptet är redo att gå till nästa rad
    public bool dialogdone = false; //Om någon dialog håller på att skrivas ut är denna false
    public static int dialogpos = 0;
    public GameObject textbox; 
    public GameObject alertbox;
    public GameObject portrait;
    public GameObject background;
    public GameObject camitem;
    public Text posobj; //Debug meny i canvas > dev
    public Text comment; //Texten som skrivs ut
    public Text personname; //Namntaggen i textboxar
    public string[] story; 
    public Coroutine co;
    public static Character[] people = new Character[2];



    // Start is called before the first frame update
    void Start()
    {
        string path = Application.dataPath;
        Character sam = new Character("Sam", "Sammy", "favthing", "brown", "favcolor", 17);
        Character fabina = new Character("Fabina", "Fabi", "bread", "blonde", "purple", 18);
        story = File.ReadAllLines($"{path}/Dialogs/start.txt");
        people[0] = sam;
        people[1] = fabina;
        
    }

    // Update is called once per frame
    void Update()
    {
        string[] line = story[dialogpos].Split(','); //line = nuvarande raden
        if (ready)
        {
            if (line[0] == "0") //textbox
            {
                Character talker = people[int.Parse(line[1])];
                string text = line[2].Replace("#", ",");
                Debug.Log($"{talker.name} says: {text}");
                ready = false;
                co = StartCoroutine(SpawnTextBox(talker, text));
            }
            else if (line[0] == "1") //new background
            {
                StartCoroutine(ChangeBackground(line[1]));
                dialogpos++;

                if (line.Length>2)
                    RemoveCharacters();
                Debug.LogError(line.Length);
            }
            else if (line[0] == "2") //move or create character
            {
                Debug.Log("yes");
                int id = int.Parse(line[1]);
                string mood = line[2];
                float x = (float)Convert.ToDouble(line[3]);
                float y = (float)Convert.ToDouble(line[4]);
                int align = int.Parse(line[5]);
                StartCoroutine(CreateCharacter(id,mood,x,y,align));
                dialogpos++;
            }
            else if (line[0] == "3") //question
            {
                dialogpos++;
            }
            else if (line[0] == "4") //open new story (no question)
            {
                story = LoadStory(line[1]);
                dialogpos = 0; //återställ positionen - ny story!
                if (line.Length > 2)
                    RemoveCharacters();
            }
            else if (line[0] == "5") //thinkbox
            {
                dialogpos++;
            }
            else if (line[0] == "6") //general box
            {
                dialogpos++;
            }
            else if (line[0] == "WAIT") //delay
            {
                ToggleTextbox(false);
                ready = false;
                StartCoroutine(Delay(float.Parse(line[1])));
            }
            else if (line[0] == "PLAYMUSIC")
            {
                StartCoroutine(PlayMusic(line[1]));
                dialogpos++;
            }
            else if (line[0] == "PLAYSFX")
            {
                StartCoroutine(PlaySoundEffect(line[1]));
                dialogpos++;
            }
        }
        else if (!dialogdone && !ready && Input.GetKeyUp("space"))
        {
            dialogdone = true;
        }
        else if (dialogdone && !ready && Input.GetKeyUp("space"))
        {
            StopCoroutine(co);
            dialogpos++;
            ready = true;
        }
            
        //debug info
        posobj.text = $"line = {dialogpos}\naction = {line[0]}\nready = {ready}\ndialogdone = {dialogdone}\n\n{story[dialogpos]}";
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
        ToggleTextbox(true);
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{Application.dataPath}/characters/{talker.name.ToLower()}port.png");
        yield return uwr.SendWebRequest();
        var texture = DownloadHandlerTexture.GetContent(uwr);
        portrait.GetComponent<Image>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        personname.text = talker.name;
        string written = target[0].ToString(); //written = det som står hittills

        for (int i = 1; i < target.Length; i++)
        {
            written = written + target[i];
            yield return new WaitForSeconds(0.04f);
            if (dialogdone) //avbryt och skriv hela
            {
                comment.text = target;
                dialogdone = true;
                break;
            }
            comment.text = written;
        }
        comment.text = target;
        dialogdone = true;
    }
    void RemoveCharacters()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("character");

        for (var i = 0; i < gameObjects.Length; i++)
            Destroy(gameObjects[i]);
        Debug.LogError("sh");
    }

    IEnumerator ChangeBackground(string bg) //ID 1
    {
        ToggleTextbox(false);
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
        ToggleTextbox(false);
        Debug.Log($"New story loaded: {story}");
        return File.ReadAllLines($"{Application.dataPath}/Dialogs/{story}.txt");
    }
    IEnumerator CreateCharacter(int id, string mood, float x, float y, int align) //ID 2
    {
        if(GameObject.Find($"{people[id].name.ToLower()}") == null) //karaktär finns ej
        {
            //ladda in filen som texture
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{Application.dataPath}/characters/{people[id].name.ToLower()}{mood}.png");
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
            UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{Application.dataPath}/characters/{people[id].name.ToLower()}{mood}.png");
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
            camitem.GetComponent<AudioSource>().clip = DownloadHandlerAudioClip.GetContent(uwr);
            camitem.GetComponent<AudioSource>().Play();
        }
    }
    void ToggleTextbox(bool shown)
    {
        textbox.SetActive(shown);
        if (!shown) //om man tar bort textboxen så försvinner texten
            comment.text = "";
    }
}
