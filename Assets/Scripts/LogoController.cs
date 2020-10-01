using SajberSim.Helper;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class LogoController : MonoBehaviour
{
    public Image logoObj;
    public Sprite defaultlogo;
    public Sprite afklogo;
    public Sprite demologo;

    private void Start()
    {
        if (Demo.isDemo) defaultlogo = demologo;
        logoObj.sprite = defaultlogo;
    }

    private void Update()
    {
        if (!Helper.loggedin) return;
        if (SteamUtils.SecondsSinceComputerActive > 1800) logoObj.sprite = afklogo;
        else logoObj.sprite = defaultlogo;
    }
}