using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

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
        string name = gameObject.transform.parent.name;
        if (name == "Card -1") GameObject.Find("Canvas/StoryChoice").GetComponent<StartStory>().Play(-1, CreateStory.currentlyEditingName, CreateStory.currentlyEditingPath);
        else if (name.Contains("Card "))
            GameObject.Find("Canvas/StoryChoice").GetComponent<StartStory>().Play(Convert.ToInt32(name.Replace("Card ", "")));
        else if (name.Contains("Detailscard"))
            GameObject.Find("Canvas/StoryChoice").GetComponent<StartStory>().Play(Convert.ToInt32(name.Replace("Detailscard ", "")));
    }
    public void OpenDetails()
    {
        if(gameObject.transform.parent.name == "Card -1") return;
        GameObject.Find("Canvas/StoryChoice").GetComponent<StartStory>().OpenDetails(Convert.ToInt32(gameObject.transform.parent.name.Replace("Card ", "")));
    }
}