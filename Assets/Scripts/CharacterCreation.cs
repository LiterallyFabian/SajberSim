using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCreation : MonoBehaviour
{

    private float startPosX;
    private float startPosY;
    private bool isHeld = false;
    private string[] backgroundpaths;
    private int currentbg = 0;
    public GameObject fadeimage;
    public InputField code;
    void Start()
    {
        Cursor.visible = true;
        string path = $@"{Application.dataPath}/Modding/Backgrounds/".Replace("/", "\\");
        backgroundpaths = Directory.GetFiles(path, "*.png");
        NextBG();
    }
    public void ClearCharacters()
    {
        GameManager.RemoveCharacters();
    }
    public void CreateCharacterStart()
    {
        StartCoroutine(CreateCharacter());
    }
    IEnumerator CreateCharacter() //ID 2
    {
        string charPath = $@"{Application.dataPath}/Modding/Characters/".Replace("/", "\\");
        string[] charpaths = Directory.GetFiles(charPath, "*neutral.png");
        //ladda in filen som texture
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{charpaths[UnityEngine.Random.Range(0, charpaths.Length)]}");
        yield return uwr.SendWebRequest();
        var texture = DownloadHandlerTexture.GetContent(uwr);

        //skapa gameobj
        GameObject character = new GameObject($"person");
        character.gameObject.tag = "character";
        SpriteRenderer renderer = character.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        //sätt size + pos
        character.transform.position = new Vector3(0, 0, -1f);
        character.transform.localScale = new Vector3(0.58f, 0.58f, 0.6f);
        character.AddComponent<BoxCollider2D>();
        character.AddComponent<CharacterCreation>();
    }
    public void RemoveCharacters()
    {
        GameManager.RemoveCharacters();
    }
    IEnumerator ChangeBackground(string bg) //ID 1
    {
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{bg}");
        yield return uwr.SendWebRequest();
        var texture = DownloadHandlerTexture.GetContent(uwr);
        GameObject.Find("background").GetComponent<SpriteRenderer>().sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
    public void NextBG()
    {
        if (currentbg == backgroundpaths.Length) currentbg = 0;
        StartCoroutine(ChangeBackground(backgroundpaths[currentbg]));
        currentbg++;
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
                codetext += $"2|ID|mood|{Math.Round(character.transform.position.x, 2)}|{Math.Round(character.transform.position.y, 2)}|1\n";
            }
            code.text = (codetext);
        }
    }
    private void OnMouseDown()
    {
        if (Input.GetMouseButton(1)) Debug.LogError("ye");
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