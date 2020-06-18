using SajberSim.Translation;
using System.Linq;
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
        if(this.gameObject.GetComponent<Text>() == null)
        {
            Debug.LogError($"Translation: Tried to translate with key {TextId} on object {gameObject.name} which does not have a text component");
            return;
        }
        Text txt = GetComponent<Text>();
        if (txt != null || TextId != null)
            if (TextId == "ISOCode")
                txt.text = Translate.GetLanguage();
            else
                txt.text = Translate.Get(TextId);
        else
        {
            Debug.LogError($"Object {txt.name}");
        }
        
    }
}