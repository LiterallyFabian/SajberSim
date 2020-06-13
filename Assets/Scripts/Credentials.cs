using System.Collections.Generic;

class Credentials
{
    public static Dictionary<string, string> webhooks = new Dictionary<string, string>()
    {
            {"log", "https://discordapp.com/api/webhooks/xxx" },
            {"support", "https://discordapp.com/api/webhooks/xxx" },
            {"stats", "https://discordapp.com/api/webhooks/xxx" }
    };
    readonly static string ftp = "ftp";
    readonly static string ftpuser = "user";
    readonly static string ftppass = "pass";
}
