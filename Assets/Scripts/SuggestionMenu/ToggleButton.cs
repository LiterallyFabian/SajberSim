using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    public Animator Menu;
    public GameObject Icon;
    private bool isOpen = false;
    public void Toggle()
    {
        isOpen = !isOpen;
        Icon.transform.rotation = isOpen ? new Quaternion(0,0,0,0) : new Quaternion(90, 0, 0, 0);
        Menu.Play(isOpen ? "popup" : "popdown");
    }
}
