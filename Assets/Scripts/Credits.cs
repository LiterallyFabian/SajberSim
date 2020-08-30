using JetBrains.Annotations;
using SajberSim.Colors;
using SajberSim.Helper;
using SajberSim.Steam;
using SajberSim.Story;
using SajberSim.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    private Camera cam;
    private Transform canvas;
    // Fade stuff
    public GameObject fadeimage;
    public bool fadestarted;
    public GameObject music;

    // Assets in the scene
    public Text Roles;
    public Text People;
    public GameObject Message;
    public GameObject Logo;
    public GameObject Background;
    public GameObject CreditHolder;
    public GameObject EscButton;

    // Variables
    private string[] creditsraw;
    public static string storypath;
    private Download dl;
    private float speed = 50;
    private bool messagefound = false;
    private void Start()
    {
        Time.timeScale = 1;
        speed = PlayerPrefs.GetFloat("creditspeed", 50);
        canvas = GameObject.Find("Canvas").transform;
        cam = Camera.main;
        dl = Download.Init();
        string creditsPath = Path.Combine(storypath, "credits.txt");
        
        if (!File.Exists(creditsPath))
        {
            Debug.LogError($"No credits found for {storypath}. Continuing with default.");
            return;
        }
        creditsraw = File.ReadAllLines(creditsPath);
        SetCredits();
    }
    private void SetCredits()
    {
        bool rolesfound = false;
        string roles = "";
        string people = "";
        int lines = 0;

        if(File.Exists(Path.Combine(storypath, "logo.png"))) 
            dl.Image(Logo, Path.Combine(storypath, "logo.png"));

        foreach(string line in creditsraw)
        {
            if (line.ToLower().StartsWith("background|"))
                dl.Image(Background, Path.Combine(storypath, "Backgrounds", $"{line.ToLower().Replace("background|", "")}.png"));


            else if (line.ToLower().StartsWith("music|") && line.Split('|').Length > 1)//$"{storypath}/Audio/{line.Split('|')[1]}.ogg"
                dl.Ogg(music, Path.Combine(storypath, "Audio", line.Split('|')[1] + ".ogg"), true);

            else if (line.ToLower().StartsWith("color|"))
            {
                Color textColor = Colors.FromRGB($"#{line.ToLower().Replace("color|", "").Replace("#", "")}");
                People.color = textColor;
                Message.GetComponent<Text>().color = textColor;
                Roles.color = Helper.ModifyColor(textColor, 0.8f);
            }
            else if (line.ToLower().StartsWith("message|") && line.Split('|').Length > 1)
            {
                messagefound = true;
                Message.GetComponent<Text>().text = line.Split('|')[1];
            }

            else if (line != "") // Roles / people found
            {
                if (line.StartsWith("-"))
                {
                    if (rolesfound)
                    {
                        roles += $"\n{line.Replace("-", "")}";
                        people += "\n";
                        lines++;
                    }
                    else
                    {
                        roles = line.Replace("-", "");
                    }
                    rolesfound = true;
                }
                else
                {
                    roles += "\n";
                    people += line + "\n";
                    lines++;
                }
            }
        }
        Message.transform.localPosition = new Vector3(0, lines * -40 - 50, 0);
        People.text = people;
        Roles.text = roles;
        if (!messagefound) Message.GetComponent<Text>().text = "";
    }
    private void Update()
    {
        CreditHolder.transform.position += Vector3.up * speed * Time.deltaTime;
        if (cam.ScreenToWorldPoint(Message.transform.position).y > 0.7f)
        {
            Message.transform.SetParent(canvas);
            EscButton.GetComponent<Animator>().Play("esc key");
            if (!messagefound) StartCoroutine(LeaveCredits());
        }
        if (Input.GetKeyDown("escape"))
            StartCoroutine(LeaveCredits());
    }
    public IEnumerator LeaveCredits()
    {
        if (!fadestarted)
        {
            StartCoroutine(AudioFadeOut.FadeOut(music.GetComponent<AudioSource>(), 1.6f));
            fadestarted = true;
            fadeimage.SetActive(true); //Open image that will fade (starts at opacity 0%)

            for (float i = 0; i <= 1; i += Time.deltaTime / 1.5f) //Starts fade, load scene when done
            {
                fadeimage.GetComponent<Image>().color = new Color(0, 0, 0, i);
                yield return null;
            }
            SceneManager.LoadScene("menu");
        }
    }
}
