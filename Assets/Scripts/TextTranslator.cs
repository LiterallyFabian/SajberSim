using SajberSim.Translation;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Drop on a text object and write the ID to translate the object to appropriate language
/// </summary>
public class TextTranslator : MonoBehaviour
{
    public string TextId;

    // Use this for initialization
    void Start()
    {
        var text = GetComponent<Text>();
        if (text != null || TextId != null)
            if (TextId == "ISOCode")
                text.text = Translate.GetLanguage();
            else
                text.text = Translate.Get(TextId);
        
    }
}