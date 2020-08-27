using Newtonsoft.Json;
using System;
using System.Net;

namespace SajberSim.Steam
{
    internal class SteamAPI
    {
        public static PlayerData GetProfile(string id)
        {
            string URL = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=6576B2E2BEA62204491CB89D75FAF90D&steamids={id}";
            WebClient webClient = new WebClient();
            string data = webClient.DownloadString(URL);
            data = data.Replace("{\"response\":{\"players\":[", "").Replace("}]}}", "}");
            try
            {
                PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(data);
                return playerData;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Could not deserialize data from url {URL}\nError: {e}");
                return null;
            };
        }
    }

    public class PlayerData
    {
        [JsonProperty("steamid")]
        public string Steamid { get; set; }

        [JsonProperty("communityvisibilitystate")]
        public long Communityvisibilitystate { get; set; }

        [JsonProperty("profilestate")]
        public long Profilestate { get; set; }

        [JsonProperty("personaname")]
        public string Personaname { get; set; }

        [JsonProperty("lastlogoff")]
        public long Lastlogoff { get; set; }

        [JsonProperty("commentpermission")]
        public long Commentpermission { get; set; }

        [JsonProperty("profileurl")]
        public string Profileurl { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("avatarmedium")]
        public string Avatarmedium { get; set; }

        [JsonProperty("avatarfull")]
        public string Avatarfull { get; set; }

        [JsonProperty("personastate")]
        public long Personastate { get; set; }

        [JsonProperty("realname")]
        public string Realname { get; set; }

        [JsonProperty("primaryclanid")]
        public string Primaryclanid { get; set; }

        [JsonProperty("timecreated")]
        public long Timecreated { get; set; }

        [JsonProperty("personastateflags")]
        public long Personastateflags { get; set; }
    }
}