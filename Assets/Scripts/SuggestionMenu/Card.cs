using SajberSim.Steam;
using SajberSim.Web;
using Steamworks;
using Steamworks.Ugc;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace SajberSim.Suggestions
{
    public class Card : MonoBehaviour
    {
        public RawImage Thumbnail;
        public Text Description;
        public Text Title;
        public ulong id;
        private Download dl;
        private void Start()
        {
            dl = Download.Init();
        }
        private void Update()
        {
            transform.localPosition = new Vector3(0, transform.localPosition.y);
        }
        public async void FillData()
        {
            var itemInfo = await SteamUGC.QueryFileAsync(id);
            Title.text = itemInfo?.Title;
            Description.text = itemInfo?.Description;
            dl.RawImage(Thumbnail.gameObject, itemInfo?.PreviewImageUrl);
        }
        public void OpenInSteam()
        {
            Process.Start($@"steam://openurl/https://steamcommunity.com/sharedfiles/filedetails/?id={id}");
        }
    }

}
