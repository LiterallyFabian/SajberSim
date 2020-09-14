using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SajberSim.Objects
{
    public class Textbox : MonoBehaviour
    {
        public Text Comment;
        public Text Nametag;
        public RawImage Portrait;
        private Download dl;
        private void Awake()
        {
            dl = Download.Init();
        }
        public void Set(string name, string portraitPath = "", bool port = false)
        {
            Nametag.text = name;
            if (port) dl.RawImage(Portrait.gameObject, portraitPath);
            GameManager.currentPortrait = name;
        }
        public void SetActive(bool enable = true, bool clearText = true)
        {
            gameObject.SetActive(enable);
            if (clearText) ResetText();
        }
        public void ResetText()
        {
            Comment.text = "";
            Nametag.text = "";
        }
    }
}
