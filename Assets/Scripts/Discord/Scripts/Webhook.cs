using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SajberSim.Web
{
    /// <summary>
    /// Sends webhooks to discord for various events. 
    /// The WebhookUrls.url is not included in the github code for security reasons, but can be created with the folloding code:
    /// </summary>
    /// 
/*
class WebhookUrls
{
    public static Dictionary<string, string> url = new Dictionary<string, string>()
    {
            {"log", "https://discordapp.com/api/webhooks/XXX" },
            {"support", "https://discordapp.com/api/webhooks/XXX" },
            {"stats", "https://discordapp.com/api/webhooks/XXX" }
    };
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
        Send(WebhookUrls.url["log"], "**:video_game: INGAME LOG**\n\n ", msg, "Log", avatar);
    }
    public static void Support(string msg, string email, string avatar = "http://sajber.me/account/Email/webhookpfp.png")
    {
        Send(WebhookUrls.url["support"], $"**:triangular_flag_on_post: NEW SUPPORT REQUEST**\nSender: {email}\n\n ", msg, "Support", avatar);
    }
    public static void Stats(string msg, string avatar = "http://sajber.me/account/Email/webhookpfp.png")
    {
        Send(WebhookUrls.url["stats"], "**:chart_with_upwards_trend: STATS**\n\n ", msg, "Stats", avatar);
    }
}
public class dWebHook : IDisposable
{
    private readonly WebClient dWebClient;
    private static NameValueCollection discordValues = new NameValueCollection();
    public string url { get; set; }
    public string displayname { get; set; }
    public string profilepic { get; set; }
    public string[] embeds { get; set; }

    public dWebHook()
    {
        dWebClient = new WebClient();
    }


    public void SendMessage(string msgSend)
    {
        discordValues.Add("username", displayname);
        discordValues.Add("avatar_url", profilepic);
        discordValues.Add("content", msgSend);
        if(embeds != null) discordValues.Add("embeds", embeds[0]);

        dWebClient.UploadValues(url, discordValues);
    }

    public void Dispose()
    {
        dWebClient.Dispose();
    }
}
}
