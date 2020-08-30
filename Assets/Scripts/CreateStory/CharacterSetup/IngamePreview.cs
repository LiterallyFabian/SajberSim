using SajberSim.CardMenu;
using SajberSim.Colors;
using SajberSim.Helper;
using SajberSim.Web;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class IngamePreview : MonoBehaviour
{
    public Toggle ShowIngame;
    public GameObject Textbox;
    public GameObject Portrait;
    public Text PortName;
    public Text comment;
    public GameObject UI;

    private Download dl;
    private void Start()
    {
        dl = Download.Init();
        
        UpdateChar();
        UpdateDesign();
    }
    public void ToggleIngame(bool showingame)
    {
        UI.SetActive(!showingame);
        Textbox.SetActive(showingame);
        if (!showingame)
        {
            UpdateChar();
        }
    }
    private void UpdateChar()
    {
        string[] ports = Stories.GetStoryAssetPaths("ports", CreateStory.currentlyEditingPath);
        if (ports.Length != 0 && ports != null)
        {
            string port = ports[Random.Range(0, ports.Length)];
            dl.Image(Portrait, port);
            string name = Path.GetFileName(port).Replace("port.png", "");
            if (Path.GetFileName(port) == "port.png") name = Path.GetFileName(Path.GetDirectoryName(port));
            
            PortName.text = name.FirstCharToUpper();
        }
    }
    private void UpdateDesign()
    {
        Helper.currentStoryPath = CreateStory.currentlyEditingPath;
        StoryDesign design = StoryDesign.Get();

        if (File.Exists($"{CreateStory.currentlyEditingPath}/textbox.png"))
        {
            Debug.Log($"Found textbox at path {CreateStory.currentlyEditingPath}/textbox.png and will try to update...");
            dl.Image(Textbox, $"{CreateStory.currentlyEditingPath}/textbox.png");
        }

        comment.color = Colors.FromRGB(design.textcolor);
        PortName.color = Colors.FromRGB(design.namecolor);
    }
}
