using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SajberSim.Colors;
using System;
using SajberSim.Web;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Unity.VectorGraphics;
using UnityEngine.SceneManagement;
using SajberSim.Steam;

namespace SajberSim.SaveSystem
{
    public class SaveCard : MonoBehaviour, IPointerClickHandler
    {
        public UnityEvent leftClick;
        public UnityEvent rightClick;
        public int id;
        public Image thumbnail;
        public Image Overlay;
        public RawImage tb2;
        public Text Title;
        public Text date;
        public Save save;
        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                leftClick.Invoke();
            else if (eventData.button == PointerEventData.InputButton.Right)
                rightClick.Invoke();
        }
        // Start is called before the first frame update
        public SaveCard(Save s, int id)
        {
            save = s;
            this.id = id;
        }
        public void Play()
        {
            Manifest data = Manifest.Get(save.path + "/manifest.json");
            if(SceneManager.GetActiveScene().name != "game")
            {
                Helper.Helper.currentStoryPath = save.path;
                Helper.Helper.currentStoryName = data.name;
                Debug.Log($"Attempting to start the novel \"{data.name}\" with path {save.path}");
                ButtonCtrl main = GameObject.Find("ButtonCtrl").GetComponent<ButtonCtrl>();
                ButtonCtrl.CreateCharacters();
                StartStory.storymenuOpen = false;
                GameManager.storyAuthor = data.author;
                GameManager.storyName = data.name;
                GameManager.save = save;
                Achievements.Grant(Achievements.List.ACHIEVEMENT_play1);
                Stats.Add(Stats.List.novelsstarted);
                StartCoroutine(main.FadeToScene("game"));
            }
            else //ingame already
            {
                
            }
        }
        public void Overwrite()
        {
            if (SceneManager.GetActiveScene().name != "game") return;
            GameManager game = GameObject.Find("GameManagerObj").GetComponent<GameManager>();
            save = game.Save(id);
            Fill();
        }
        public void Fill()
        {
            Download dl = GameObject.Find("Helper").GetComponent<Download>();
            Manifest data = Manifest.Get(save.path + "/manifest.json");
            Title.text = save.novelname;
            Color splashColor = Color.white;
            ColorUtility.TryParseHtmlString($"#{data.overlaycolor.Replace("#", "")}", out splashColor);
            Overlay.GetComponent<Image>().color = splashColor;

            Color textColor = Colors.Colors.UnityGray;
            ColorUtility.TryParseHtmlString($"#{data.textcolor.Replace("#", "")}", out textColor);
            Title.GetComponent<Text>().color = textColor;
            date.text = save.date.ToString("dddd, d MMMM HH:mm");
            dl.CardThumbnail(tb2, $"{Helper.Helper.savesPath}/{id}.png");
        }
    }
}