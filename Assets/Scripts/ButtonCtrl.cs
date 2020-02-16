using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonCtrl : MonoBehaviour
{
    public GameObject OverwriteAlert;
    public GameObject Logo;
    public GameObject Settings;
    public Button ContinueButton;
    public Toggle uwu;
    public Slider Speed;
    public Slider Volume;
    public Text SpeedText;
    public Text VolumeText;
    public Text UwuText;
    public GameObject BehindSettings;


    public void Start()
    {
        if (PlayerPrefs.GetInt("hasplayed", 0) == 1) //ifall man inte har spelat tidigare kan man inte använda den knappen
            ContinueButton.interactable = true;
        else
            ContinueButton.interactable = false;

        SpeedText.text = $"{Math.Round(PlayerPrefs.GetFloat("delay", 0.04f) * 1000)}ms";
        VolumeText.text = $"{Math.Round(PlayerPrefs.GetFloat("volume", 1f) * 100)}%";

        if (PlayerPrefs.GetInt("uwu", 0) == 1)
        {
            uwu.SetIsOnWithoutNotify(true);
            UwuText.text = "(◠‿◠✿)";
        }
        
        Speed.SetValueWithoutNotify(PlayerPrefs.GetFloat("delay",0.04f));
        Volume.SetValueWithoutNotify(PlayerPrefs.GetFloat("volume", 1f));

    }
    public void StartNew()
    {
        if(PlayerPrefs.GetInt("hasplayed", 0) == 1)
        {
            OverwriteAlert.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetInt("hasplayed", 1);
            SceneManager.LoadScene("game");
        }
    }
    public void StartNewConfirmed()
    {
        SceneManager.LoadScene("game");
    }
    public void CancelNew()
    {
        OverwriteAlert.SetActive(false);
    }
    public void OpenSettings()
    {
        Settings.SetActive(true);
        Logo.SetActive(false);
        BehindSettings.SetActive(true);
    }
    public void CloseSettings()
    {
        Settings.SetActive(false);
        Logo.SetActive(true);
        BehindSettings.SetActive(false);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
    }
    public void ChangeSpeed(float value)
    {
        PlayerPrefs.SetFloat("delay", value);
        SpeedText.text = $"{Math.Round(PlayerPrefs.GetFloat("delay",0.04f)*1000)}ms";
    }
    public void ChangeVolume(float newVolume)
    {
        PlayerPrefs.SetFloat("volume", newVolume);
        AudioListener.volume = PlayerPrefs.GetFloat("volume",1f);
        VolumeText.text = $"{Math.Round(PlayerPrefs.GetFloat("volume", 1f)*100)}%";
    }
    public void UwUToggle(bool uwu)
    {
        if (uwu)
        {
            UwuText.text = "(◠‿◠✿)";
            PlayerPrefs.SetInt("uwu", 1);
        }
        else //disable / false
        {
            UwuText.text = "";
            PlayerPrefs.SetInt("uwu", 0);
        }
    }

}
