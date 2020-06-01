using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public void Set(float a)
    {
        GetComponent<Image>().color = new Color(0, 0, 0, a);
    }
    public void SetDark(bool d)
    {
        if (d)
            GetComponent<Image>().color = new Color(0.92f, 0.92f, 0.92f);
        else
            GetComponent<Image>().color = Color.white;
    }
}
