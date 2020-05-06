using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace SajberSim.Web
{
    public class Download : MonoBehaviour
    {
        public IEnumerator UpdateItem(GameObject item, string path, string type)
        {
            Debug.Log($"Trying to download file {path}...");
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
            {
                yield return uwr.SendWebRequest();
                if (uwr.isNetworkError) Debug.LogError($"Could not download {path}\n{uwr.error}");
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    if(type == "img")
                        item.GetComponent<Image>().sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    else if (type == "sprite")
                        item.GetComponent<SpriteRenderer>().sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    Debug.Log($"{path} downloaded successfully.");
                }
            }
        }
        public void Image(GameObject items, string path)
        {
            StartCoroutine(UpdateItem(items, path, "img"));
        }
        public void Sprite(GameObject items, string path)
        {
            StartCoroutine(UpdateItem(items, path, "sprite"));
        }
    }
}
