using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class CharacterCreation : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    private GameObject target;
    public bool isMouseDrag;
    private Vector3 screenPosition;

    void Start()
    {

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
        UnityWebRequest uwr = UnityWebRequestTexture.GetTexture($"file://{charpaths[Random.Range(0, charpaths.Length)]}");
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
    }
    GameObject ReturnClickedObject(out RaycastHit hit)
    {
        GameObject target = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
        {
            target = hit.collider.gameObject;
        }
        return target;
    }
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitInfo;
            target = ReturnClickedObject(out hitInfo);
            if (target != null)
            {
                isMouseDrag = true;
                Debug.Log("target position :" + target.transform.position);
                //Convert world position to screen position.
                screenPosition = Camera.main.WorldToScreenPoint(target.transform.position);
                offset = target.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseDrag = false;
        }

        if (isMouseDrag)
        {
            //track mouse position.
            Vector3 currentScreenSpace = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPosition.z);

            //convert screen position to world position with offset changes.
            Vector3 currentPosition = Camera.main.ScreenToWorldPoint(currentScreenSpace) + offset;

            //It will update target gameobject's current postion.
            target.transform.position = currentPosition;
        }

    }
}