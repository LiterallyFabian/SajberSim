using JetBrains.Annotations;
using SajberSim.Helper;
using SajberSim.Story;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    // Fade stuff
    public GameObject fadeimage;
    public bool fadestarted;
    public GameObject music;

    // Assets in the scene
    public Text Roles;
    public Text People;
    public Text Message;
    public Text NoCredits;
    public GameObject Logo;
    public GameObject Background;
    public GameObject CreditHolder;

    // Variables
    public static string storyname = "";
    private string[] creditsraw;
    private string storypath;
    private Download dl;
    private void Start()
    {
        dl = GameObject.Find("EventSystem").GetComponent<Download>();
        storypath = $"{Application.dataPath}/Story/{storyname}";
        string creditsPath = storypath + "/credits.txt";
        
        if (!File.Exists(creditsPath))
        {
            Debug.LogError($"No credits found for {storyname}. Continuing with default.");
            NoCredits.color = new Color(0.772549f, 0.3098039f, 0.6470588f, 1);
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

        if(File.Exists($"{storypath}/logo.png")) 
            dl.Image(Logo, $"{storypath}/logo.png");

        foreach(string line in creditsraw)
        {
            if (line.ToLower().StartsWith("background|"))
                dl.Image(Background, $"{storypath}/Backgrounds/{line.ToLower().Replace("background|", "")}.png");
            else if (line.ToLower().StartsWith("music|"))
                dl.Ogg(music, $"{storypath}/Audio/{line.ToLower().Replace("music|", "")}.ogg", true);
            else if (line.ToLower().StartsWith("color|"))
            {
                Color textColor = new Color(0.772549f, 0.3098039f, 0.6470588f, 1); //sajbersim pink
                ColorUtility.TryParseHtmlString($"#{line.ToLower().Replace("color|", "").Replace("#", "")}", out textColor);
                People.color = textColor;
                Roles.color = Helper.ModifyColor(textColor, 0.8f);
            }
            else if (line.ToLower().StartsWith("message|"))
                Message.text = line.Split('|')[1];

            else if(line != "") // Roles / people found
            {
                if (line.StartsWith("-"))
                {
                    if (rolesfound)
                    {
                        roles += $"\n\n{line.Replace("-", "")}";
                        people += "\n\n";
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
                }
            }
        }
        People.text = people;
        Roles.text = roles;
    }
    private void Update()
    {
        CreditHolder.transform.position += Vector3.up * 40 * Time.deltaTime;
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
