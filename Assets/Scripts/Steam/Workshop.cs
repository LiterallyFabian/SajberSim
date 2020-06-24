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
                    .WithTag(Helper.Helper.genresid[tag])
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
            }
        }
        public static bool Verify(string dataPath)
        {
            if (!File.Exists($@"{dataPath}/steam.png"))
            {
                Helper.Helper.Alert("h");
                return true;
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
