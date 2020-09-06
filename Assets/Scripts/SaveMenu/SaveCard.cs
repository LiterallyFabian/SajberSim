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
using System.Globalization;
using System.Linq;
using System.IO;
using UnityEditor;

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
        public SaveMenu Main;
        private bool working = false;
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
        public void Click()
        {
            if(working) //story exists
            {
                Manifest data = Manifest.Get(save.path + "/manifest.json");
                if (SceneManager.GetActiveScene().name != "game") //play from main
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
                    StartCoroutine(main.FadeToScene("game"));
                }
                else //play from ingame
                {

                }
            }
            else if (!working)//story doesn't exist, delete card
            {
                Debug.Log($"SaveCard/Delete: Deleting card {id}");
                if (File.Exists($"{Helper.Helper.savesPath}/{id}.png")) File.Delete($"{Helper.Helper.savesPath}/{id}.png");
                if (File.Exists($"{Helper.Helper.savesPath}/{id}.save")) File.Delete($"{Helper.Helper.savesPath}/{id}.save");
                Main.UpdateMenu();
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
            if (File.Exists(Path.Combine(save.path, "manifest.json"))) working = true;
            Download dl = GameObject.Find("Helper").GetComponent<Download>();
            Title.text = save.novelname;

            Overlay.GetComponent<Image>().color = Colors.Colors.FromRGB(save.splashcolor);
            Title.GetComponent<Text>().color = Colors.Colors.FromRGB(save.textcolor);

            string datetext = save.date.ToString("dddd, d MMMM HH:mm", Language.Culture);
            date.text = datetext.First().ToString().ToUpper() + datetext.Substring(1);
            dl.CardThumbnail(tb2, $"{Helper.Helper.savesPath}/{id}.png");
            if (!working) tb2.color = Color.red;
        }
    }
}