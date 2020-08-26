using SajberSim.Helper;
using SajberSim.Web;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PublishMenu : MonoBehaviour
{
    public InputField P_Title;
    public InputField P_Description;
    public RawImage P_Thumbnail;
    public Dropdown P_Privacy;
    private Download dl;
    public GameObject P_NoThumbnailText;
    public Button P_Publish;
    private void Start()
    {
        dl = Download.Init();
        
    }
    public void FillData()
    {
        P_Title.text = CreateStory.currentlyEditingName;
        P_Description.text = Manifest.Get($"{CreateStory.currentlyEditingPath}/manifest.json").description;
        if (File.Exists($"{CreateStory.currentlyEditingPath}/steam.png"))
        {
            dl.CardThumbnail(P_Thumbnail, $"{CreateStory.currentlyEditingPath}/steam.png");
            P_Publish.interactable = true;
            P_NoThumbnailText.SetActive(false);
        }
        else
        {
            P_Publish.interactable = false;
            P_NoThumbnailText.SetActive(true);
        }
        
        P_Privacy.ClearOptions();
        P_Privacy.AddOptions(Helper.privacysettings.ToList());
        GetFiles(CreateStory.currentlyEditingPath);
    }
    public void TryPublish()
    {
        Debug.Log($"PublishMenu/Publish: Started verifying {CreateStory.currentlyEditingPath} for publishing on Steam.");

    }
    private string CopyNovelToTemp(string path)
    {
        string tempDirectory = $"{Application.temporaryCachePath}/sbrupload/{UnityEngine.Random.Range(10000, 99999)}";
        //Copy all directories from template
        foreach (string dirPath in Directory.GetDirectories(CreateStory.currentlyEditingPath, "*", SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(CreateStory.currentlyEditingPath, tempDirectory));

        //Copy all files from template
        foreach (string newPath in Directory.GetFiles(CreateStory.currentlyEditingPath, "*.*", SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(CreateStory.currentlyEditingPath, tempDirectory), true);

        return tempDirectory;
    }
    private void DeleteUnusedFiles(string path)
    {

    }
    public void GetFiles(string path)
    {
        string[] extensions = new string[] { ".png", ".txt", ".ogg", ".json" };
        List<string> allFiles = new List<string>();
        foreach(string subpath in Directory.GetDirectories(path))
        {
            DirectoryInfo di = new DirectoryInfo(subpath);
            var files = di.GetFiles();

            files.AsParallel().Where(f => !extensions.Contains(f.Extension)).ForAll((f) => allFiles.Add(f.FullName));
        }
        DirectoryInfo dir = new DirectoryInfo(path);
        var files2 = dir.GetFiles();

        files2.AsParallel().Where(f => !extensions.Contains(f.Extension)).ForAll((f) => allFiles.Add(f.FullName));
        Debug.Log(string.Join("\n", allFiles));
    }
}