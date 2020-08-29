using SajberSim.Translation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Policy;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SajberSim.SaveSystem
{
    public class SaveMenu : MonoBehaviour
    {
        // Start is called before the first frame update
        public int page = 0;
        public Text pageinfo;
        public GameObject SaveCardTemplate;
        public GameObject EmptyCardTemplate;
        public GameObject RightClickInfo;
        public GameObject LeftClickInfo;
        public Text nosavesnotice;
        public GameObject CloseButtonBehind;
        public bool menuopen = false;
        public void Start()
        {
            if(SceneManager.GetActiveScene().name != "game")
            {
                LeftClickInfo.transform.position = RightClickInfo.transform.position;
                RightClickInfo.SetActive(false);
            }
            UpdateMenu();
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && menuopen) ToggleMenu(false);
        }
        public void ToggleMenu(bool open)
        {
            if (menuopen == open) return;
            if (open)
            {
                menuopen = true;
                GetComponent<Animator>().Play("openStorymenu");
                GameManager.savemenuopen = true;
                CloseButtonBehind.SetActive(true);
            }
            else
            {
                GameManager.savemenuopen = false;
                menuopen = false;
                GetComponent<Animator>().Play("closeStorymenu");
                CloseButtonBehind.SetActive(false);
            }
        }
        public void UpdateMenu()
        {
            Debug.Log("SaveMenu/Update: Requesting to update save menu...");
            if (!Directory.Exists(Helper.Helper.savesPath)) Directory.CreateDirectory(Helper.Helper.savesPath);
            string[] paths = Directory.GetFiles(Helper.Helper.savesPath, "*.save");
            nosavesnotice.text = paths.Length == 0 ? Translate.Get("nosavesfound") : "";
            int cardPages = GetPages(paths.Length);
            ClearSaveCards();
            pageinfo.GetComponent<Text>().text = string.Format(Translate.Get("page"), page + 1, cardPages + 1);
            for (int i = page * 9; i < page * 9 + 9; i++)
            {
                if (paths.Length == i)
                {
                    if (SceneManager.GetActiveScene().name != "game") return;
                    GameObject card = Instantiate(EmptyCardTemplate, Vector3.zero, new Quaternion(0, 0, 0, 0), GetComponent<Transform>()) as GameObject;
                    card.transform.localPosition = Helper.Helper.SavePositions[Helper.Helper.SavePositions.Keys.ElementAt(i - (page * 9))];
                    return;
                }
                Vector3 position = Helper.Helper.SavePositions[Helper.Helper.SavePositions.Keys.ElementAt(i - (page * 9))];
                int cardno = StringToInt(Path.GetFileName(paths[i]).Replace(".save", ""));
                if (cardno != -1)
                {
                    Save save = Save.Get(paths[i]);

                    CreateCard(save, position, i - (page * 9), cardno);
                }
                else
                {
                    Helper.Helper.Alert(Translate.Get("donteditsaves"));
                }
            }
        }
        public GameObject CreateCard(Save data, Vector3 pos, int id, int cardno)
        {
            //spawn, place and resize
            GameObject card = Instantiate(SaveCardTemplate, Vector3.zero, new Quaternion(0, 0, 0, 0), GetComponent<Transform>()) as GameObject;
            card.transform.localPosition = pos;
            card.name = $"Card {cardno}";
            //fill with data
            SaveCard cardDetails = card.GetComponent<SaveCard>();
            cardDetails.save = data;
            cardDetails.id = cardno;
            cardDetails.Main = GetComponent<SaveMenu>();
            cardDetails.Fill();
            return card;
        }
        public void ClearSaveCards()
        {
            Debug.Log("SaveMenu/Clear: Request to delete save cards");
            GameObject[] cards = GameObject.FindGameObjectsWithTag("SaveCard");
            foreach (GameObject card in cards)
                GameObject.Destroy(card);
        }
        public int GetPages(int length)
        {
            if (SceneManager.GetActiveScene().name == "game") length++;
            if (length <= 9) return 0;
            else if (length % 9 == 0) return length / 9 - 1;
            return (length - (length % 9)) / 9;
        }
        public void ChangePage(int change)
        {
            int pages = GetPages(Directory.GetFiles(Helper.Helper.savesPath, "*.save").Length);
            if (pages == 0)
            {
                pageinfo.gameObject.GetComponent<Animator>().Play("storycard_pageinfojump", 0, 0);
                return;
            }

            ClearSaveCards();
            if (page + change > pages) page = 0;
            else if (page + change < 0) page = pages;
            else page += change;
            UpdateMenu();
        }
        public static int StringToInt(string n)
        {
            try
            {
                return Convert.ToInt32(n);
            }
            catch
            {
                return -1;
            }
        }
    }
}