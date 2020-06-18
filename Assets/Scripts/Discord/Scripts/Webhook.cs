using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SajberSim.Web
{
    /// <summary>
    /// Sends webhooks to discord for various events. 
    /// The webhook urls are not included in the github code for security reasons, 
    /// But you can create your own using Discord or simply cancel these methods
    /// </summary>
/*
class Credentials
{
    public static Dictionary<string, string> webhooks = new Dictionary<string, string>()
    {
            {"log", "https://discordapp.com/api/webhooks/xxx" },
            {"support", "https://discordapp.com/api/webhooks/xxx" },
            {"stats", "https://discordapp.com/api/webhooks/xxx" }
    };
    public const string ftp = "ftp://";
    public const string ftpuser = "";
    public const string ftppass = "";
}
*/
    class Webhook
    {
        private static void Send(string url, string msgbase, string msg, string nameext, string avatar)
        {
            using (dWebHook dcWeb = new dWebHook())
            {
                dcWeb.profilepic = avatar;
                dcWeb.displayname = $"SajberSim {nameext}";
                dcWeb.url = url;
                dcWeb.SendMessage(msgbase + msg);
            }
        }
        public static void Log(string msg, string avatar = "http://sajber.me/account/Email/webhookpfp.png")
        {
            Send(Credentials.webhooks["log"], "**:video_game: INGAME LOG**\n\n ", msg, "Log", avatar);
        }
        public static void Support(string msg, string email, string avatar = "http://sajber.me/account/Email/webhookpfp.png")
        {
            Send(Credentials.webhooks["support"], $"**:triangular_flag_on_post: NEW SUPPORT REQUEST**\nSender: {email}\n\n ", msg, "Support", avatar);
        }
        public static void Stats(string msg, string avatar = "http://sajber.me/account/Email/webhookpfp.png")
        {
            Send(Credentials.webhooks["stats"], "**:chart_with_upwards_trend: STATS**\n\n ", msg, "Stats", avatar);
        }
    }
    public class dWebHook : IDisposable
    {
        private readonly WebClient dWebClient;
        private static NameValueCollection discordValues = new NameValueCollection();
        public string url { get; set; }
        public string displayname { get; set; }
        public string profilepic { get; set; }

        public dWebHook()
        {
            dWebClient = new WebClient();
        }


        public void SendMessage(string msgSend)
        {
            discordValues.Set("username", displayname);
            discordValues.Set("avatar_url", profilepic);
            discordValues.Set("content", msgSend);

            try
            {
                dWebClient.UploadValues(url, discordValues);
            }
            catch (WebException e)
            {
                UnityEngine.Debug.LogError($"Webhook: Tried to send message with text {msgSend} failed, error message:\n\n{e}");
            }
        }

        public void Dispose()
        {
            dWebClient.Dispose();
        }
    }
}
