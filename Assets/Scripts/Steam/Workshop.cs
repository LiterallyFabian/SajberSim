using Steamworks.Data;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SajberSim.Helper;
using SajberSim.Translation;
using Steamworks;
using SajberSim.Web;

namespace SajberSim.Steam
{
    public class Workshop
    {
        public enum Privacy
        {
            Public,
            FriendsOnly,
            Private
        }
        public enum Rating
        {
            Everyone,
            Questionable,
            Mature
        }
        public async static void Upload(string title, string description, int tag, string dataPath, string lang, Privacy privacy, Rating rating)
        {
            Editor item = Steamworks.Ugc.Editor.NewCommunityFile
                    .WithTitle(title)
                    .WithDescription(description)
                    .WithTag(Helper.Helper.genresSteam[tag])
                    .WithTag(rating.ToString())
                    .WithContent(dataPath)
                    .InLanguage(lang)
                    .WithPreviewFile(dataPath + "/steam.png")
                    .WithChangeLog("First upload");

            switch (privacy)
            {
                case Privacy.Public:
                    item.WithPublicVisibility(); break;
                case Privacy.FriendsOnly:
                    item.WithFriendsOnlyVisibility(); break;
                case Privacy.Private:
                    item.WithPrivateVisibility(); break;
            }

            PublishResult result = await item.SubmitAsync(new ProgressClass());
            UnityEngine.Debug.Log($"Steam: Tried to upload a new workshop item with title {title}, path {dataPath}. \nResult from Steam: {result.Result}");
            if(result.Success)
            {
                Helper.Helper.Alert(string.Format(Translate.Get("publishsuccess"), SteamClient.Name, title, result.FileId));
                Process.Start($@"steam://openurl/https://steamcommunity.com/sharedfiles/filedetails/?id={result.FileId}"); 
                Stats.Add(Stats.List.novelspublished);
                if (privacy == Privacy.Public) Webhook.Stats($"{SteamClient.Name} uploaded a new visual novel: \"{title}\"!\nhttps://steamcommunity.com/sharedfiles/filedetails/?id={result.FileId}");
            }
            else
            {
                Helper.Helper.Alert(string.Format(Translate.Get("publishfail"), result.Result));
            }
        }
        /// <summary>
        /// Verifies if a folder is ready for upload.
        /// </summary>
        /// <param name="dataPath">Path to story folder</param>
        /// <returns>Whether it's ready or not.</returns>
        public static bool Verify(string dataPath)
        {
            string[] folders = { "Audio", "Backgrounds", "Characters", "Dialogues" };
            if (!File.Exists($@"{dataPath}/steam.png"))
            {
                Helper.Helper.Alert(Translate.Get("picnotfound")); // Thumbnail for steam was not found.
                return false;
            }
            else
            {
                FileInfo thumbnail = new FileInfo($@"{dataPath}/steam.png");
                if (thumbnail.Length > 1000000)
                {
                    Helper.Helper.Alert(Translate.Get("pictoolarge")); // Thumbnail for steam was too large.
                    return false;
                }
            }
            foreach(string folder in folders)
            {
                if (!Directory.Exists($"{dataPath}/{folder}"))
                {
                    Helper.Helper.Alert(string.Format(Translate.Get("nodirectory"), folder, folder.ToLower())); //Directory not found
                    return false;
                }
                else if(Directory.GetFileSystemEntries($"{dataPath}/{folder}").Length == 0)
                {
                    Helper.Helper.Alert(string.Format(Translate.Get("nodirectory"), folder, folder.ToLower())); //Directory empty
                    return false;
                }
            }
            return true;
        }
	}
    class ProgressClass : IProgress<float>
    {
        float lastvalue = 0;

        public void Report(float value)
        {
            if (lastvalue >= value) return;

            lastvalue = value;

            UnityEngine.Debug.Log(value);
        }
    }
}
