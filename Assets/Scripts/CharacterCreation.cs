using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SajberSim.Web;

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

    void Start()
    {
        dl = (new GameObject("downloadobj")).AddComponent<Download>();
        Cursor.visible = true;
        string path = $@"{Application.dataPath}/Modding/Backgrounds/";
        backgroundpaths = Directory.GetFiles(path, "*.png");
    }
    public void ClearCharacters()
    {
        GameManager.RemoveCharacters();
    }
    public void CreateCharacterStart()
    {
        string charPath = $@"{Application.dataPath}/Modding/Characters/";
        string[] charpaths = Directory.GetFiles(charPath, "*neutral.png");

        //skapa gameobj
        GameObject character = new GameObject($"person");
        character.gameObject.tag = "character";
        SpriteRenderer renderer = character.AddComponent<SpriteRenderer>();
        dl.Sprite(character, $"file://{charpaths[UnityEngine.Random.Range(0, charpaths.Length)]}");


        //sätt size + pos
        character.transform.position = new Vector3(0, 0, -1f);
        character.transform.localScale = new Vector3(GameManager.charsize, GameManager.charsize, 0.6f);
        character.AddComponent<BoxCollider2D>();
        character.AddComponent<CharacterCreation>();
    }

    public void RemoveCharacters()
    {
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

        if(gameObject.name == "GameObject")
        {
            string codetext = "";
            GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("character");
            foreach (GameObject character in gameObjects)
            {
                codetext += $"CHAR|ID|mood|{Math.Round(character.transform.position.x, 2)}|{Math.Round(character.transform.position.y, 2)}|1\n";
            }
            code.text = codetext;
        }
    }
    private void OnMouseDown()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        startPosX = mousePos.x - transform.localPosition.x;
        startPosY = mousePos.y - transform.localPosition.y;
        isHeld = true;
    }
    private void OnMouseUp()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (mousePos.x < -7.8 && mousePos.y > 3.8) Destroy(gameObject);
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