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
        private IEnumerator UpdateItem(GameObject item, string path, string type)
        {
            Debug.Log($"Trying to download image {path}...");
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(path))
            {
                uwr.timeout = 1;
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError) 
                    Debug.LogError($"Could not download {path}\n{uwr.error}");
                else
                {
                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    if(type == "img")
                        item.GetComponent<Image>().sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    else if (type == "sprite")
                        item.GetComponent<SpriteRenderer>().sprite = UnityEngine.Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    Debug.Log($"Successfully downloaded {path}");
                }
            }
        }
        private IEnumerator Setogg(GameObject item, string path, bool play)
        {
            Debug.Log($"Trying to download audio {path}...");
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.OGGVORBIS))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError) 
                    Debug.LogError($"Could not download {path}\n{uwr.error}");
                else
                {
                    item.GetComponent<AudioSource>().clip = DownloadHandlerAudioClip.GetContent(uwr);
                    Debug.Log($"Successfully downloaded {path}");
                    if (play) item.GetComponent<AudioSource>().Play();
                }
            }
        }
        public void Image(GameObject item, string path)
        {
            StartCoroutine(UpdateItem(item, path, "img"));
        }
        public void Sprite(GameObject item, string path)
        {
            StartCoroutine(UpdateItem(item, path, "sprite"));
        }
        public void Ogg(GameObject item, string path, bool play = false)
        {
            StartCoroutine(Setogg(item, path, play));
        }
        /// <summary>
        /// Returns flag from given country code, or a placeholder if the code is invalid
        /// </summary>
        /// <param name="code">2 letter country code</param>
        public Sprite Flag(string code)
        {
            Sprite flag = Resources.Load<Sprite>($"Flags/{code.ToUpper()}@3x");
            if (flag != null) return flag;
            return Resources.Load<Sprite>($"Flags/unknown");
        }
    }
}
