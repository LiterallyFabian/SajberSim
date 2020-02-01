using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PersonClass;

public class GameManager : MonoBehaviour
{
    public int shown = -1;
    public int hidden = 1;
    public bool dialogdone = true;
    public static int dialogpos = 0;
    public GameObject textbox;
    public GameObject background;
    public Sprite bgmatsal;
    public Sprite bgmain;
    public Sprite samhappy;
    public Sprite samsad;
    public Sprite samneutral;
    public Sprite fabinahappy;
    public Sprite fabinasad;
    public Sprite fabinaneutral;
    public string[] story;

    public TextAsset main;


    // Start is called before the first frame update
    void Start()
    {
        string temp = main.text;
        story = temp.Split(Environment.NewLine.ToCharArray());

        Character sam = new Character("Sam","Sammy", "favthing", "brown", "favcolor", 17, samsad, samneutral, samhappy);
        Character fabina = new Character("Fabina", "Fabi", "bread", "blonde", "purple", 18, fabinasad, fabinaneutral, fabinahappy);
        Character[] persons = { sam, fabina };
    }

    // Update is called once per frame
    void Update()
    {
        if (dialogdone)
        {
            string[] line = story[dialogpos].Split(',');
            if(line[0] == "0") //textbox
            {

            }
            else if (line[0] == "1") //new background
            {
                ChangeBackground(line[1]);
            }
            else if (line[0] == "2") //move or create character
            {
                 
            }
            else if (line[0] == "3") //question
            {

            }
            else if (line[0] == "4") //open script (no question)
            {

            }
            else if (line[0] == "5") //thinkbox
            {

            }
        }
    }
    IEnumerator SpawnTextBox(Character talker, string mood, string text)
    {
        textbox.transform.position = new Vector3(0, -3.5f, shown); 
        dialogdone = false; //ifall funktionen är klar. är denna false kan inte en ny startas
        dialogpos++; //vilken rad vi är på i dialogen
        string written = text[0].ToString(); //written = det som står, text = target
        for (int i = 1; i < text.Length; i++)
        {
            written = written + text[i];
            yield return new WaitForSeconds(0.05f);
            if (Input.GetKeyDown("space")) //avbryt och skriv hela
            {
                written = text;
                break;
            }
        }

        Debug.Log(written);
    }
    void ChangeBackground(string changeto)
    {
        if (changeto == "matsal")
        {
            background.GetComponent<SpriteRenderer>().sprite = bgmatsal;
        } 
        else if (changeto == "main")
        {
            background.GetComponent<SpriteRenderer>().sprite = bgmain;
        }
    }
}
