using SajberSim.Web;
using System;
using System.Collections.Generic;
using UnityEngine;
using Prefs = SajberSim.Helper.Helper.Prefs;

namespace SajberSim.Translation
{
    internal class Translate
    {
        public static string lang = Get2LetterISOCodeFromSystemLanguage().ToLower();

        /// <summary>
        /// Languages supported in the game. can be selected from settings
        /// </summary>
        public static string[] languages = { "en", "sv" };

        /// <summary>
        /// Dictionary with words on selected language
        /// Example: Translate.Fields["word"]
        /// </summary>
        public static Dictionary<String, String> Fields { get; private set; }

        /// <summary>
        /// Fall-back ictionary with words on english
        /// </summary>
        public static Dictionary<String, String> EnglishFields { get; private set; }

        /// <summary>
        /// Init on first use
        /// </summary>
        static Translate()
        {
            LoadLanguage();
        }

        public static string Get(string id, string location = "a script")
        {
            if (Fields.ContainsKey(id))
                return Fields[id];
            else if (EnglishFields.ContainsKey(id))
            {
                Debug.LogWarning($"Translation/Get: Could not find translation for \"{id}\" in language {lang.ToUpper()}, falling back on English.");
                return EnglishFields[id];
            }
            else
            {
                Debug.LogError($"Translation/Get: Translation for \"{id}\" does not exist in {lang.ToUpper()} or EN, returning ID. This request was called from {location}");
                Webhook.Log($"<@&743158406948585593>\n\nTranslation for \"{id}\" does not exist in {lang.ToUpper()} or EN. This was called from game version v{Application.version}");
                return $"TRANSLATION MISSING: {id}";
            }
        }

        /// <summary>
        /// Load language files
        /// </summary>
        private static void LoadLanguage()
        {
            if (Fields == null) Fields = new Dictionary<string, string>();
            if (EnglishFields == null) EnglishFields = new Dictionary<string, string>();

            if (PlayerPrefs.GetString(Prefs.language.ToString(), "none") != "none") lang = PlayerPrefs.GetString(Prefs.language.ToString());

            var textAsset = Resources.Load($@"Languages/{lang}");
            if (textAsset == null)
            {
                textAsset = Resources.Load(@"Languages/en") as TextAsset;
                Debug.LogWarning($"Translation: Could not find a translation file for {lang} - using English instead");
            }

            string allTexts = (textAsset as TextAsset).text;
            string[] lines = allTexts.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            string key, value;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].IndexOf("=") >= 0 && !lines[i].StartsWith("#") && lines[i] != "")
                {
                    key = lines[i].Substring(0, lines[i].IndexOf("="));
                    value = lines[i].Substring(lines[i].IndexOf("=") + 1,
                            lines[i].Length - lines[i].IndexOf("=") - 1).Replace("\\n", Environment.NewLine);
                    Fields.Add(key, value);
                }
            }
            if (lang != "en") LoadFallback();
        }

        private static void LoadFallback()
        {
            string allTexts = (Resources.Load(@"Languages/en") as TextAsset).text;
            string[] lines = allTexts.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            string key, value;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].IndexOf("=") >= 0 && !lines[i].StartsWith("#") && lines[i] != "")
                {
                    key = lines[i].Substring(0, lines[i].IndexOf("="));
                    value = lines[i].Substring(lines[i].IndexOf("=") + 1,
                            lines[i].Length - lines[i].IndexOf("=") - 1).Replace("\\n", Environment.NewLine);
                    EnglishFields.Add(key, value);
                }
            }
        }

        /// <summary>
        /// get the current language
        /// </summary>
        /// <returns></returns>
        public static string GetLanguage()
        {
            return Get2LetterISOCodeFromSystemLanguage().ToLower();
        }

        /// <returns>The 2-letter ISO code from system language.</returns>
        public static string Get2LetterISOCodeFromSystemLanguage()
        {
            SystemLanguage lang = Application.systemLanguage;
            string res = "EN";
            switch (lang)
            {
                case SystemLanguage.Afrikaans: res = "AF"; break;
                case SystemLanguage.Arabic: res = "AR"; break;
                case SystemLanguage.Basque: res = "EU"; break;
                case SystemLanguage.Belarusian: res = "BY"; break;
                case SystemLanguage.Bulgarian: res = "BG"; break;
                case SystemLanguage.Catalan: res = "CA"; break;
                case SystemLanguage.Chinese: res = "ZH"; break;
                case SystemLanguage.ChineseSimplified: res = "ZH"; break;
                case SystemLanguage.ChineseTraditional: res = "ZH"; break;
                case SystemLanguage.Czech: res = "CS"; break;
                case SystemLanguage.Danish: res = "DA"; break;
                case SystemLanguage.Dutch: res = "NL"; break;
                case SystemLanguage.English: res = "EN"; break;
                case SystemLanguage.Estonian: res = "ET"; break;
                case SystemLanguage.Faroese: res = "FO"; break;
                case SystemLanguage.Finnish: res = "FI"; break;
                case SystemLanguage.French: res = "FR"; break;
                case SystemLanguage.German: res = "DE"; break;
                case SystemLanguage.Greek: res = "EL"; break;
                case SystemLanguage.Hebrew: res = "IW"; break;
                case SystemLanguage.Hungarian: res = "HU"; break;
                case SystemLanguage.Icelandic: res = "IS"; break;
                case SystemLanguage.Indonesian: res = "IN"; break;
                case SystemLanguage.Italian: res = "IT"; break;
                case SystemLanguage.Japanese: res = "JA"; break;
                case SystemLanguage.Korean: res = "KO"; break;
                case SystemLanguage.Latvian: res = "LV"; break;
                case SystemLanguage.Lithuanian: res = "LT"; break;
                case SystemLanguage.Norwegian: res = "NO"; break;
                case SystemLanguage.Polish: res = "PL"; break;
                case SystemLanguage.Portuguese: res = "PT"; break;
                case SystemLanguage.Romanian: res = "RO"; break;
                case SystemLanguage.Russian: res = "RU"; break;
                case SystemLanguage.SerboCroatian: res = "SH"; break;
                case SystemLanguage.Slovak: res = "SK"; break;
                case SystemLanguage.Slovenian: res = "SL"; break;
                case SystemLanguage.Spanish: res = "ES"; break;
                case SystemLanguage.Swedish: res = "SV"; break;
                case SystemLanguage.Thai: res = "TH"; break;
                case SystemLanguage.Turkish: res = "TR"; break;
                case SystemLanguage.Ukrainian: res = "UK"; break;
                case SystemLanguage.Unknown: res = "EN"; break;
                case SystemLanguage.Vietnamese: res = "VI"; break;
            }
            //		Debug.Log ("Lang: " + res);
            return res;
        }
    }
}