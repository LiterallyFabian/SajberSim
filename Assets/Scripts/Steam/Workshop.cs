using SajberSim.Translation;
using SajberSim.Web;
using Steamworks;
using Steamworks.Ugc;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

namespace SajberSim.Steam
{
    public class Workshop
    {
        public static float publishProgress;

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

        public async static void Upload(WorkshopData wdata)
        {
            Debug.Log(wdata.id);
            Editor item;
            if (wdata.id == -1) item = Steamworks.Ugc.Editor.NewCommunityFile;
            else item = new Editor((ulong)wdata.id);

            if (wdata.changenotes == "") wdata.changenotes = "No change notes provided.";

            item = item.WithTitle(wdata.title)
            .WithDescription(wdata.description)
            .WithTag(wdata.genre)
            .WithTag("Mature")
            .WithContent(wdata.dataPath)
            .InLanguage(wdata.lang)
            .WithPreviewFile(Path.Combine(wdata.dataPath, "steam.png"))
            .WithChangeLog(wdata.changenotes);

            switch (wdata.privacy)
            {
                case Privacy.Public:
                    item.WithPublicVisibility(); break;
                case Privacy.FriendsOnly:
                    item.WithFriendsOnlyVisibility(); break;
                case Privacy.Private:
                    item.WithPrivateVisibility(); break;
            }
            PublishResult result = await item.SubmitAsync(new ProgressClass());
            if (wdata.id == -1)
                Debug.Log($"Steam: Tried to upload a new workshop item with title {wdata.title}, path {wdata.dataPath}. \nResult from Steam: {result.Result}");
            else
                Debug.Log($"Steam: Tried to update workshop item {wdata.id} with title {wdata.title}, path {wdata.dataPath}. \nResult from Steam: {result.Result}");
            if (result.Success)
            {
                wdata.st.Stop();
                if (wdata.id == -1)
                    Helper.Helper.Alert(string.Format(Translate.Get("publishsuccess"), SteamClient.Name, wdata.title, wdata.st.ElapsedMilliseconds, result.FileId));
                else
                    Helper.Helper.Alert(string.Format(Translate.Get("updatesuccess"), SteamClient.Name, wdata.title, wdata.st.ElapsedMilliseconds));
                Process.Start($@"steam://openurl/https://steamcommunity.com/sharedfiles/filedetails/?id={result.FileId}");

                Stats.Add(Stats.List.novelspublished);

                //send the link on discord if it's public
                if (wdata.privacy == Privacy.Public && wdata.id == -1) Webhook.Stats($"{SteamClient.Name} uploaded a new visual novel: \"{wdata.title}\"!\nhttps://steamcommunity.com/sharedfiles/filedetails/?id={result.FileId}");

                string manifestPath = Path.Combine(wdata.originalPath, "manifest.json");
                Manifest data = Manifest.Get(manifestPath);
                data.authorid = $"{SteamClient.SteamId}";
                data.author = SteamClient.Name;
                data.id = result.FileId.ToString();
                Manifest.Write(manifestPath, data);
            }
            else
            {
                Helper.Helper.Alert(string.Format(Translate.Get("publishfail"), result.Result));
            }
        }

        public static bool Download(ulong id)
        {
            return SteamUGC.Download(id);
        }
        public async static Task<Item?> GetInfo(ulong id)
        {
            var itemInfo = await SteamUGC.QueryFileAsync(id);
            return itemInfo;
        }
    }

    public class ProgressClass : IProgress<float>
    {
        private float lastvalue = 0;

        public void Report(float value)
        {
            if (lastvalue >= value) return;

            lastvalue = value;

            Workshop.publishProgress = value;
            Debug.Log($"Uploading visual novel to Steam, progress: {value * 100}%");
        }
    }
}