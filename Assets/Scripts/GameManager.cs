using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static PersonClass;

public class GameManager : MonoBehaviour
{
    public int shown = -1;
    public int hidden = 1;
    public bool ready = true;
    public bool dialogdone = false;
    public static int dialogpos = 0;
    public GameObject textbox;
    public Text posobj;
    public GameObject alertbox;
    public GameObject portrait;
    public Text comment;
    public Text personname;
    public GameObject background;
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
        posobj.text = $"pos: {dialogpos}\nready = {ready}";
        if (ready)
        {
            string[] line = story[dialogpos].Split(','); //line = nuvarande raden
            if (line[0] == "0") //textbox
            {
                Character talker = people[int.Parse(line[1])];
                string mood = line[2];
                string text = line[3];
                Debug.Log($"{talker.name} says: {text}");
                ready = false;
                co = StartCoroutine(SpawnTextBox(talker, mood, text));
            }
            else if (line[0] == "1") //new background
            {
                StartCoroutine(ChangeBackground(line[1]));
                dialogpos++;
            }
            else if (line[0] == "2") //move or create character
            {
                dialogpos++;
            }
            else if (line[0] == "3") //question
            {

            }
            else if (line[0] == "4") //open new story (no question)
            {
                story = LoadStory(line[1]);
                dialogpos = 0; //återställ positionen - ny story!
            }
            else if (line[0] == "5") //thinkbox
            {
                dialogpos++;
            }
            else if (line[0] == "6") //general box
            {
                dialogpos++;
            }
        }
        else if(dialogdone && !ready)
        {
            if (Input.GetKeyUp("space"))
            {
                StopCoroutine(co);
                ready = true;
                dialogpos++;
            }
        }
    }
    IEnumerator SpawnTextBox(Character talker, string mood, string target) //ID 0
    {
        textbox.SetActive(true);
        WWW www = new WWW($"file://{Application.dataPath}/characters/{talker.name.ToLower()}port.png");
        yield return www;
        portrait.GetComponent<Image>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
        personname.text = talker.name;
        string written = target[0].ToString(); //written = det som står hittills

        for (int i = 1; i < target.Length; i++)
        {
            written = written + target[i];
            yield return new WaitForSeconds(0.04f);
            if (Input.GetKeyUp("space")) //avbryt och skriv hela
            {
                comment.text = target;
                dialogdone = true;
            }
            comment.text = written;
        }
        comment.text = target;
        dialogdone = true;
        Debug.LogError("text done");
    }

    IEnumerator ChangeBackground(string bg) //ID 1
    {
        Debug.Log($"New background loaded: {bg}");
        WWW www = new WWW($"file://{Application.dataPath}/Backgrounds/{bg}.png");
        yield return www;
        background.GetComponent<SpriteRenderer>().sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0.5f, 0.5f));
    }

    string[] LoadStory(string story) //ID 4
    {
        Debug.Log($"New story loaded: {story}");
        return File.ReadAllLines($"{Application.dataPath}/Dialogs/{story}.txt");
    }
    void ReplaceSprite(GameObject obj, string path)
    {

    }
}
