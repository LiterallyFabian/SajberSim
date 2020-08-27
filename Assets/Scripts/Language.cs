using SajberSim.Translation;
using SajberSim.Web;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
public class Language
{
    public static NumberFormatInfo Format = new NumberFormatInfo();
    public static CultureInfo Culture = new CultureInfo("en-us");

    static Language()
    {
        Format.NumberDecimalSeparator = ".";
        Culture = new CultureInfo(Languages[Translate.lang.ToUpper()].LCID_code);
    }

    public Language(string code, string name, string formal, string localized, string lcid, string key)
    {
        iso_code = code;
        language_code = name;
        formal_name = formal;
        localized_name = localized;
        LCID_code = lcid;
        windows_key = key;
    }

    public string iso_code; //DK, the code used for flags
    public string language_code; //danish, the code used for steam
    public string formal_name; //Danish, formal name in english
    public string localized_name; //Dansk, formal name in original language
    public string LCID_code; //da
    public string windows_key; //DA

    public static Dictionary<string, Language> Languages = new Dictionary<string, Language>()
    {
        {"EN", new Language("US", "english", "English", "English", "en-us", "EN")},
        {"AR", new Language("ARAB_LEAGUE", "arabic", "Arabic", "عربى", "ar-ae", "AR")},
        {"BG", new Language("BG", "bulgarian", "Bulgarian", "Български", "bg", "BG")},
        {"ZH", new Language("CN", "schinese", "Chinese", "中文", "zh-cn", "ZH")},
        {"CS", new Language("CZ", "czech", "Czech", "čeština", "cs", "CS")},
        {"DA", new Language("DK", "danish", "Danish", "Dansk", "da", "DA")},
        {"NL", new Language("NL", "dutch", "Dutch", "Nederlands", "nl-nl", "NL")},
        {"FI", new Language("FI", "finnish", "Finnish", "Soumi", "fi", "FI")},
        {"FR", new Language("FR", "french", "French", "Français", "fr-fr", "FR")},
        {"DE", new Language("DE", "german", "German", "Deutsch", "de-de", "DE")},
        {"EL", new Language("GR", "greek", "Greek", "Ελληνικά", "el", "EL")},
        {"HU", new Language("HU", "hungarian", "Hungarian", "Magyar", "hu", "HU")},
        {"IT", new Language("IT", "italian", "Italian", "Italiano", "it", "IT")},
        {"JA", new Language("JP", "japanese", "Japanese", "日本語", "ja", "JA")},
        {"KO", new Language("KR", "koreana", "Korean", "한국어", "ko", "KO")},
        {"NO", new Language("NO", "norwegian", "Norwegian", "Norsk", "no-no", "NO")},
        {"PL", new Language("PL", "polish", "Polish", "Polski", "pl", "PL")},
        {"PT", new Language("PT", "portuguese", "Portuguese", "Português", "pt-pt", "PT")},
        {"BR", new Language("BR", "brazilian", "Portuguese (Brazil)", "Português-Brasil", "pt-br", "BR")},
        {"RO", new Language("RO", "romanian", "Romanian", "Limba română", "ro", "RO")},
        {"RU", new Language("RU", "russian", "Russian", "русский", "ru", "RU")},
        {"ES", new Language("ES", "spanish", "Spanish", "Español", "es-es", "ES")},
        {"SV", new Language("SE", "swedish", "Swedish", "Svenska", "sv-se", "SV")},
        {"TH", new Language("TH", "thai", "Thai", "ไทย", "th", "TH")},
        {"TR", new Language("TR", "turkish", "Turkish", "Türkçe", "tr", "TR")},
        {"UK", new Language("UA", "ukrainian", "Ukrainian", "українська мова", "uk", "UK")},
        {"VI", new Language("VN", "vietnamese", "Vietnamese", "Tiếng Việt", "vi", "VI") }
    };

    public static List<string> ListEnglishName()
    {
        
        List<string> lang = new List<string>();
        foreach (Language language in Languages.Values)
            lang.Add(language.formal_name);
        return lang;
    }

    public static List<string> ListLocalizedName()
    {
        List<string> lang = new List<string>();
        foreach (Language language in Languages.Values)
            lang.Add(language.localized_name);
        return lang;
    }

    public static List<string> ListFlagCode()
    {
        List<string> lang = new List<string>();
        foreach (Language language in Languages.Values)
            lang.Add(language.iso_code);
        return lang;
    }

    public static List<string> ListSteamName()
    {
        List<string> lang = new List<string>();
        foreach (Language language in Languages.Values)
            lang.Add(language.language_code);
        return lang;
    }

    public static List<string> ListLCIDCode()
    {
        List<string> lang = new List<string>();
        foreach (Language language in Languages.Values)
            lang.Add(language.LCID_code);
        return lang;
    }
    public static List<string> ListWindowsKeys()
    {
        List<string> lang = new List<string>();
        foreach (Language language in Languages.Values)
            lang.Add(language.windows_key);
        return lang;
    }
}