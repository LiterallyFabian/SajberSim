using SajberSim.Colors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardOverlay : MonoBehaviour, IPointerClickHandler
{
    public StoryCard card;
    public UnityEvent leftClick;
    public UnityEvent rightClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            leftClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            rightClick.Invoke();
    }
    public void Set(float a)
    {
        GetComponent<Image>().color = new Color(0, 0, 0, a);
    }
    public void SetDark(bool d)
    {
        if (d)
            GetComponent<Image>().color = Colors.AlmostTransparent;
        else
            GetComponent<Image>().color = Colors.Transparent;
    }
}
