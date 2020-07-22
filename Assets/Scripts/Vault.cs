using SajberSim.Colors;
using SajberSim.Helper;
using SajberSim.Translation;
using Steamworks;
using System;
using System.Collections;
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
        Debug.Log($"VAULT: {input.text} wasn't a code, how sad!");
        StartCoroutine(ResetField());
        Lore.color = Colors.NsfwRed;
        Lore.text = Translate.Get($"vaulterror{UnityEngine.Random.Range(0, 7)}");
    }
    private void Correct(string hash)
    {
        Lore.color = Colors.SfwGreen;
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
                GetComponent<Animator>().enabled = true;
                GetComponent<Animator>().Play("vaultDrop");
                break;
        }
    }
    private IEnumerator ResetField()
    {
        input.GetComponent<AudioSource>().Play();
        float time = 0.3f;
        for (float t = 0f; t < time; t += Time.deltaTime)
        {
            input.transform.localScale = new Vector3(1, Mathf.Lerp(1, 0, t / time), 1);
            yield return null;
        }
        input.transform.localScale = new Vector3(1, 0, 1);
        input.text = "";
        for (float t = 0f; t < time; t += Time.deltaTime)
        {
            input.transform.localScale = new Vector3(1, Mathf.Lerp(0, 1, t / time), 1);
            yield return null;
        }
        input.transform.localScale = new Vector3(1, 1, 1);
    }
}
