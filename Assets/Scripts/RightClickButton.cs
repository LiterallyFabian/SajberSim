using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class RightClickButton : MonoBehaviour, IPointerClickHandler
{

    public UnityEvent leftClick;
    public UnityEvent middleClick;
    public UnityEvent rightClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            leftClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Middle)
            middleClick.Invoke();
        else if (eventData.button == PointerEventData.InputButton.Right)
            rightClick.Invoke();
    }
    public void PlayStory()
    {
        GameObject.Find("Canvas/StoryChoice").GetComponent<StartStory>().Play(Convert.ToInt32(gameObject.transform.parent.name.Replace("Card ","")));
    }
    public void OpenDetails()
    {
        GameObject.Find("Canvas/StoryChoice").GetComponent<StartStory>().OpenDetails(Convert.ToInt32(gameObject.transform.parent.name.Replace("Card ", "")));
    }
}