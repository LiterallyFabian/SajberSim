using SajberSim.Translation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AboutDialog : MonoBehaviour, IPointerClickHandler
{
    private TextMeshProUGUI m_TextMeshPro;
    private Camera m_Camera;
    private Canvas m_Canvas;

    void Awake()
    {
        m_TextMeshPro = gameObject.GetComponent<TextMeshProUGUI>();
        m_Canvas = gameObject.GetComponentInParent<Canvas>();
        if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            m_Camera = null;
        else
            m_Camera = m_Canvas.worldCamera;

        //set about info
        m_TextMeshPro.text =
            string.Format(Translate.Get("aboutversion"), Application.version)
            + "\n"
            + Translate.Get("aboutcopyright")
            + "\n\n"
            + string.Format(Translate.Get("aboutwebsite"), "<link=\"website\"><color=blue>https://sajber.me/</link></color>")
            + "\n"
            + string.Format(Translate.Get("aboutsupport"), "<link=\"helpsite\"><color=blue>https://help.sajber.me/</link></color>")
            + "\n"
            + Translate.Get("aboutcollab");

        //set title
        gameObject.transform.parent.Find("Title").GetComponent<Text>().text = string.Format(Translate.Get("abouttitle"), Application.version);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        try
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, m_Camera);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];
                int linkHashCode = linkInfo.hashCode;

                //Debug.Log(TMP_TextUtilities.GetSimpleHashCode("website"));

                switch (linkHashCode)
                {
                    case -959088902: // helpsite
                        Application.OpenURL("https://help.sajber.me");
                        break;
                    case 1799556155: // mainsite
                        Application.OpenURL("https://sajber.me");
                        break;
                }
            }
        }
        catch { }
    }

    void Start()
    {
        //Title.text = string.Format(Translate.Get("abouttitle"), Application.version);   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
