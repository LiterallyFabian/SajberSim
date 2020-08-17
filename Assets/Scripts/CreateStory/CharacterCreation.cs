using SajberSim.Translation;
using SajberSim.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour
{
    private string[] backgroundpaths;
    private int currentbg = 0;
    public GameObject fadeimage;
    public InputField code;
    private CultureInfo customCulture;

    private Download dl;

    private List<string> allchars = new List<string>(); //list with all Characters
    public List<string> allspawned = new List<string>(); //list with all spawned characters
    private List<string> allbacks = new List<string>();
    public Dropdown DDcreatechar;
    public Dropdown DDsetback;

    private void Start()
    {
        customCulture = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
        customCulture.NumberFormat.NumberDecimalSeparator = ".";

        Thread.CurrentThread.CurrentCulture = customCulture;
        dl = Download.Init();
        Cursor.visible = true;
        string path = $@"{CreateStory.currentlyEditingPath}/Backgrounds/";
        backgroundpaths = Directory.GetFiles(path, "*.png");
        allbacks = backgroundpaths.ToList();
        for (int i = 0; i < allbacks.Count; i++)
        {
            allbacks[i] = allbacks[i].Replace(path, "").Replace(".png", "");
        }
        DDsetback.AddOptions(allbacks);
        FillLists();
        SetBG(0);
    }

    public void FillLists()
    {
        string charPath = $@"{CreateStory.currentlyEditingPath}/Characters/";
        List<string> charPaths = Directory.GetFiles(charPath, "*.png").ToList();
        foreach (string subpath in Directory.GetDirectories(charPath))
            charPaths.AddRange(Directory.GetFiles(subpath, "*.png"));
        for (int i = 0; i < charPaths.Count; i++)
        {
            allchars.Add(charPaths[i].Replace(charPath, "").Replace("\\", "/").Replace(".png", ""));
        }
        allchars = allchars.Except(allspawned).ToList(); //remove already spawned characters
        allchars.RemoveAll(u => u.EndsWith("port"));
        allchars.Sort();
        allchars.Insert(0, Translate.Get("createchar"));
        DDcreatechar.ClearOptions();
        DDcreatechar.AddOptions(allchars);
    }

    public void SubmitCharacter(int id) //input from dropdown
    {
        if (id == 0) return;
        string name = allchars[id];
        allspawned.Add(name);
        CreateCharacter(name);
        FillLists();
    }

    public void CreateCharacter(string name)
    {
        //skapa gameobj
        GameObject character = new GameObject(name);
        character.gameObject.tag = "character";
        character.AddComponent<SpriteRenderer>();
        dl.Sprite(character, $"{CreateStory.currentlyEditingPath}/Characters/{name}.png");

        //sätt size + pos
        character.transform.position = new Vector3(0, 0, -1f);
        character.transform.localScale = new Vector3(GameManager.charactersize, GameManager.charactersize, 0.6f);
        character.AddComponent<BoxCollider2D>().size = new Vector2(5, 10);
        CharacterMovement script = character.AddComponent<CharacterMovement>();
        script.Main = GetComponent<CharacterCreation>();
    }

    public void RemoveCharacters()
    {
        allspawned.Clear();
        FillLists();
        GameManager.RemoveCharacters();
    }

    public void SetBG(int i)
    {
        dl.Sprite(GameObject.Find("background"), $"file://{backgroundpaths[i]}");
    }

    private void Update()
    {
        string codetext = "";
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("character");
        foreach (GameObject character in gameObjects)
        {
            bool flipped = false;
            if (character.transform.localScale.x < 0) flipped = true;
            if (!character.name.Contains("/"))
                codetext += $"CHAR|{character.name}|[MOOD]|{Math.Round(character.transform.position.x, 1)}|{Math.Round(character.transform.position.y, 1)}|{Math.Round(character.transform.localScale.y / GameManager.charactersize, 2)}|{flipped}\n";
            else
                codetext += $"CHAR|{character.name.Split('/')[0]}|{character.name.Split('/')[1]}|{Math.Round(character.transform.position.x, 1)}|{Math.Round(character.transform.position.y, 1)}|{Math.Round(character.transform.localScale.y / GameManager.charactersize, 2)}|{flipped}\n";
        }
        code.text = codetext;
    }

    public void ReturnToMain()
    {
        StartCoroutine(FadeToScene("menu"));
    }

    public IEnumerator FadeToScene(string scene)
    {
        fadeimage.SetActive(true); //Open image that will fade (starts at opacity 0%)

        for (float i = 0; i <= 1; i += Time.deltaTime / 1.5f) //Starts fade, load scene when done
        {
            fadeimage.GetComponent<Image>().color = new Color(0, 0, 0, i);
            if (i > 0.5f) Cursor.visible = false;
            yield return null;
        }
        SceneManager.LoadScene(scene);
    }
}