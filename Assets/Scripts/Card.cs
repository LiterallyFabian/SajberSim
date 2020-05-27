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
}
