using SajberSim.Helper;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

class Vault : MonoBehaviour
{
    public InputField input;
    public Text Lore;
    public void RunCode()
    {
        string code = input.text.ToLower().Hash();
        string[] correctHashes = { "50DF67917FFEEE1506C3E7619A02E794CD965320C7412A12708D09266F12BC4F3E1564DDF53AB9E943A93C648C726F3A14BA4032C3A49922E4B264FC5EC88F28" };
        if (correctHashes.Contains(code)) Correct(code);
        else Error();
    }
    public void CloseVault()
    {
        Destroy(gameObject);
    }
    private void Error()
    {
        Debug.Log("no");
    }
    private void Correct(string hash)
    {
        switch (hash)
        {
            case "50DF67917FFEEE1506C3E7619A02E794CD965320C7412A12708D09266F12BC4F3E1564DDF53AB9E943A93C648C726F3A14BA4032C3A49922E4B264FC5EC88F28":
                if (GameObject.Find("Eastereggs").GetComponent<GameSpin>() == null)
                {
                    Error(); 
                    return;
                }
                GameObject.Find("Eastereggs").GetComponent<GameSpin>().StartSpin();
                Lore.text = $"Jeez{(Helper.loggedin ? " " + SteamClient.Name : "")}...";
                break;
        }
    }
}
