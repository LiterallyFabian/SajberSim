using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using Random = System.Random;

namespace SajberSim.Suggestions
{
    public class Menu : MonoBehaviour
    {
        public static bool success;
        private string URL = $"https://gist.githubusercontent.com/LiterallyFabian/7296b08d69f5392edc4dd1b05e0b7c56/raw/";
        private float CardHeight = 440;
        private float CardDistance = 10;
        public GameObject Content;
        public GameObject NovelSuggestionCard;
        void Start()
        {
            success = false;
            Debug.Log("Suggestions/Menu/Start: Updating suggestions card on main menu.");

            //Get all ids
            WebClient webClient = new WebClient();
            string data = webClient.DownloadString(URL);
            string[] ids = data.Split(',');
            success = true;

            //Set length
            int cardAmount = ids.Length > 5 ? 4 : ids.Length;
            Content.GetComponent<RectTransform>().sizeDelta = new Vector2(300, (CardHeight + CardDistance) * (cardAmount));
            Content.transform.localPosition = new Vector3(-10, 0);

            //Randomize list
            Random rnd = new Random();
            ids = ids.OrderBy(x => rnd.Next()).ToArray();

            //Fill with cards
            for (int i = 0; i < cardAmount; i++)
            {
                GameObject cardobject = Instantiate(NovelSuggestionCard, Content.transform);
                cardobject.transform.localPosition = new Vector3(0, CardHeight * i * -1 - 220 - (CardDistance * i));
                Card cardcomp = cardobject.GetComponent<Card>();
                cardcomp.id = ulong.Parse(ids[i]);
                cardcomp.FillData();
            }
        }
    }
}