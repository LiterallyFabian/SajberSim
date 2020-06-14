using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SajberSim.Web;
using System.Linq;

public class CharacterCreation : MonoBehaviour
{

    private float startPosX;
    private float startPosY;
    private bool isHeld = false;
    private string[] backgroundpaths;
    private int currentbg = 0;
    public GameObject fadeimage;
    public InputField code;

    Download dl;

    List<string> allcharsU = new List<string>(); //list with all Characters
    List<string> allchars = new List<string>(); //list with all characters
    List<string> allspawned = new List<string>(); //list with all spawned characters
    public Dropdown DDcreatechar;

    void Start()
    {
        dl = new GameObject("downloadobj").AddComponent<Download>();
        Cursor.visible = true;
        string path = $@"{Application.dataPath}/Modding/Backgrounds/";
        backgroundpaths = Directory.GetFiles(path, "*.png");

        FillLists();
    }
    public void FillLists()
    {
        if (SceneManager.GetActiveScene().name == "characterpos" && gameObject.name == "GameObject")
        {
            string charPath = $@"{Application.dataPath}/Modding/Characters/";
            string[] charPaths = Directory.GetFiles(charPath, "*neutral.png");
            allcharsU.Add("Skapa karaktär...");
            for (int i = 0; i < charPaths.Length; i++)
            {
                string name = charPaths[i].Replace(charPath, "").Replace("neutral.png", "");
                allcharsU.Add(Char.ToUpper(name[0]) + name.Remove(0, 1));
                allchars.Add(name);
            }
            allcharsU = allcharsU.Except(allspawned).ToList(); //remove already spawned characters
            DDcreatechar.ClearOptions();
            DDcreatechar.AddOptions(allcharsU);
        }
    }
    public void SubmitCharacter(int id) //input from dropdown
    {
        if (id == 0) return;
        string name = allcharsU[id];
        allspawned.Add(name);
        CreateCharacter(name);
        FillLists();
    }
    public void ClearCharacters()
    {
        GameManager.RemoveCharacters();
    }
    public void CreateCharacter(string name)
    {
        //skapa gameobj
        GameObject character = new GameObject(name);
        character.gameObject.tag = "character";
        character.AddComponent<SpriteRenderer>();
        dl.Sprite(character, $"{Application.dataPath}/Modding/Characters/{name}neutral.png");


        //sätt size + pos
        character.transform.position = new Vector3(0, 0, -1f);
        character.transform.localScale = new Vector3(GameManager.charactersize, GameManager.charactersize, 0.6f);
        character.AddComponent<BoxCollider2D>().size = new Vector2(5, 10);
        character.AddComponent<CharacterCreation>();
    }
    public void CycleMood() //todo
    {
        string name = this.gameObject.name.Split('_')[0].ToLower();
        string currentmood = this.gameObject.name.Split('_')[1];
        string[] moodpaths = Directory.GetFiles($@"{Application.dataPath}/Modding/Characters/", $"{name}*.png");
        int currentmoodID = Array.FindIndex(moodpaths, row => row.Contains($"{name}{currentmood}"));
        currentmoodID++;

        if (moodpaths.Length - 1 > currentmoodID)
        {
            dl.Sprite(this.gameObject, moodpaths[0]);
        }
        else dl.Sprite(this.gameObject, moodpaths[currentmoodID]);

    }

    public void RemoveCharacters()
    {
        allspawned.Clear();
        FillLists();
        GameManager.RemoveCharacters();
    }
    public void NextBG()
    {
        currentbg++;
        if (currentbg == backgroundpaths.Length) currentbg = 0;
        dl.Sprite(GameObject.Find("background"), $"file://{backgroundpaths[currentbg]}");
    }
    private void Update()
    {

        if (isHeld)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gameObject.transform.localPosition = new Vector3(mousePos.x - startPosX, mousePos.y - startPosY, 0);
        }

        if (gameObject.name == "GameObject")
        {
            string codetext = "";
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("character");
            foreach (GameObject character in gameObjects)
            {
                codetext += $"CHAR|{character.name}|neutral|{Math.Round(character.transform.position.x, 1)}|{Math.Round(character.transform.position.y, 1)}|1\n";
            }
            code.text = codetext;
        }
    }
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            startPosX = mousePos.x - transform.localPosition.x;
            startPosY = mousePos.y - transform.localPosition.y;
            isHeld = true;
        }
        else if (Input.GetMouseButtonDown(1))
        {

        }
    }
    private void OnMouseUp()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x < -7.8 && mousePos.y > 3.8)
        {
            GameObject.Find("GameObject").GetComponent<CharacterCreation>().allspawned.Remove(gameObject.name);
            Destroy(gameObject);
            GameObject.Find("GameObject").GetComponent<CharacterCreation>().FillLists();
        }
        isHeld = false;
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
