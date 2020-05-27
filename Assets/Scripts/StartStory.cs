using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using SajberSim.Helper;
using System.Linq;
using System.IO;
using UnityEngine.UI;
using SajberSim.Web;

/// <summary>
/// Puts all downloaded thumbnails in the story menu
/// </summary>
public class StartStory : MonoBehaviour
{
    private Helper shelper = new Helper();
    private int page = 0;
    public GameObject StoryCardTemplate;
    Download dl;




    // Start is called before the first frame update
    void Start()
    {
        dl = new GameObject("downloadobj").AddComponent<Download>();
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void UpdatePreviewCards()
    {
        ClearPreviewCards();

        string[] storyPaths = shelper.GetAllStoryPaths();
        string[] manifests = shelper.GetAllManifests();
        for (int i = page*6; i < page*6+6; i++)
        {
            if (manifests.Length == i) return; //cancel if story doesn't exist, else set all variables
            Debug.Log($"Importing manifest {manifests[i]}");
            Manifest storydata = JsonConvert.DeserializeObject<Manifest>(File.ReadAllText(manifests[i]));
            string name = storydata.name;
            string language = storydata.language.ToUpper();
            string overlaycolor = storydata.overlaycolor.Replace("#","");
            string textcolor = storydata.textcolor.Replace("#", "");
            bool nsfw = storydata.nsfw;
            int playtime = storydata.playtime;

            //spawn, place and resize
            GameObject menu = Instantiate(StoryCardTemplate, Vector3.zero, new Quaternion(0,0,0,0), GameObject.Find("Canvas/StoryChoice").GetComponent<Transform>()) as GameObject;
            menu.transform.localPosition = Helper.CardPositions[Helper.CardPositions.Keys.ElementAt(i)];
            menu.transform.localScale = Vector3.one;
            menu.name = $"Story card {i}";

            //fill with data
            if(File.Exists($"{storyPaths[i]}/thumbnail.png"))
            dl.Image(GameObject.Find($"Canvas/StoryChoice/{menu.name}/Thumbnail"), $"{storyPaths[i]}/thumbnail.png");

            Color splashColor = Color.white;
            ColorUtility.TryParseHtmlString($"#{overlaycolor}", out splashColor);
            GameObject.Find($"Canvas/StoryChoice/{menu.name}/Overlay").GetComponent<Image>().color = splashColor;

            Color textColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 1); //standard gray
            ColorUtility.TryParseHtmlString($"#{textcolor}", out textColor);
            GameObject.Find($"Canvas/StoryChoice/{menu.name}/Title").GetComponent<Text>().color = textColor;

            GameObject.Find($"Canvas/StoryChoice/{menu.name}/Title").GetComponent<Text>().text = name;

            GameObject.Find($"Canvas/StoryChoice/{menu.name}/Flag").GetComponent<Image>().sprite = dl.Flag(language);
        }
    }
    public void PageUp()
    {

    }
    public void PageDown()
    {

    }
    public void ClearPreviewCards()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("PreviewCard");

        for (int i = 0; i < gameObjects.Length; i++)
            Destroy(gameObjects[i]);


    }
    public void OpenMenu()
    {
        GameObject.Find("Canvas/StoryChoice").GetComponent<Transform>().localScale = Vector3.one;
    }
    public void CloseMenu()
    {
        GameObject.Find("Canvas/StoryChoice").GetComponent<Transform>().localScale = Vector3.zero;
    }
    
}
