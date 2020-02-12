using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonCtrl : MonoBehaviour
{
    public GameObject OverwriteAlert;
    public Button continuebutton;
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
    public void Continue()
    {

    }
    public void OpenSettings()
    {
        SceneManager.LoadScene("settings");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void Start()
    {
        if (PlayerPrefs.GetInt("hasplayed", 0) == 1) //ifall man inte har spelat tidigare kan man inte använda den knappen
            continuebutton.interactable = true;
        else
            continuebutton.interactable = false;
    }
    public void ResetAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
