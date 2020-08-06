using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AfkLogo : MonoBehaviour
{
    public Image logoObj;
    private Sprite logo;
    public Sprite afklogo;
    private void Start()
    {
        logo = logoObj.sprite;
    }
    private void Update()
    {
        if (SteamUtils.SecondsSinceComputerActive > 1800) logoObj.sprite = afklogo;
        else logoObj.sprite = logo;
    }
}
