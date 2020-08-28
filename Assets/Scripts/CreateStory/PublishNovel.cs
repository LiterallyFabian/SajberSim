using SajberSim.CardMenu;
using SajberSim.Helper;
using SajberSim.Steam;
using SajberSim.Translation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class PublishNovel : MonoBehaviour
{
    public NovelDebugger ND;
    public PublishMenu Menu_Publish;
    public void TryPublish(string novelPath)
    {
        Stopwatch st = new Stopwatch();
        st.Start();
        Debug.Log($"PublishNovel/Publish: Started verifying {novelPath} for publishing on Steam.");
        string tempPath = $"{Application.temporaryCachePath}/upload/{UnityEngine.Random.Range(100000, 999999)}";
        Helper.CopyDirectory(novelPath, tempPath);
        DeleteUnusedFiles(tempPath);
        Debug.Log("Step 1: STARTED. Running debugger.");
        if (ND.DebugNovel(tempPath).ErrorActions != 0)
        {
            Helper.Alert(string.Format(Translate.Get("containserrors"), ND.ErrorActions));
            Debug.Log($"Step 1: FAILED. Novel contains {ND.ErrorActions} errors and can therefore not be published. Aborting.");
            Directory.Delete(tempPath);
            return;
        }
        else
        {
            Debug.Log($"Step 1: PASSED. Found {ND.ErrorActions} errors.");
        }
        Debug.Log("Step 2: STARTED. Checking file size.");
        long filesize = Helper.DirSize(new DirectoryInfo(tempPath));
        double size = Math.Round(Helper.BytesTo(filesize, Helper.DataSize.Megabyte), 1);
        long thumbnailsize = new FileInfo(tempPath + "/steam.png").Length;
        if (filesize > 256000000)
        {
            Helper.Alert(string.Format(Translate.Get("dirtoolarge"), size));
            Debug.Log($"Step 2: FAILED. Novel takes up {size} MB of space and can therefore not be published. Aborting.");
            Directory.Delete(tempPath);
            return;
        }
        else if (thumbnailsize > 1000000)
        {
            Helper.Alert(string.Format(Translate.Get("thumbtoolarge"), Math.Round(Helper.BytesTo(thumbnailsize, Helper.DataSize.Megabyte), 1)));
            Debug.Log($"Step 2: FAILED. Thumbnail size is too large. Aborting.");
            Directory.Delete(tempPath);
            return;
        }
        else
        {
            Debug.Log($"Step 2: PASSED. Total size is {size} MB, thumbnail size is {Math.Round(Helper.BytesTo(thumbnailsize, Helper.DataSize.Megabyte), 1)} MB.");
        }
        Debug.Log("Step 3: STARTED. Generating uploading item.");

        SendPublish(tempPath, novelPath, st);
    }
    private void SendPublish(string path, string defaultPath, Stopwatch st)
    {
        try
        {

            Manifest data = Manifest.Get(path + "/manifest.json");
            WorkshopData wdata = new WorkshopData
            {
                title = Menu_Publish.P_Title.text,
                description = Menu_Publish.P_Description.text,
                lang = Language.Languages[data.language].language_code,
                genre = Helper.genresSteam[Array.IndexOf(Helper.genresid, data.genre)],
                st = st,
                id = Convert.ToInt64(data.id),
                originalPath = defaultPath,
                changenotes = Menu_Publish.P_Notes.text,
                dataPath = path
            };
            switch (data.rating)
            {
                case "mature": wdata.rating = Workshop.Rating.Mature; break;
                case "questionable": wdata.rating = Workshop.Rating.Questionable; break;
                default: wdata.rating = Workshop.Rating.Everyone; break;
            }
            switch (Menu_Publish.P_Privacy.value)
            {
                case 0: wdata.privacy = Workshop.Privacy.Public; break;
                case 1: wdata.privacy = Workshop.Privacy.FriendsOnly; break;
                case 2: wdata.privacy = Workshop.Privacy.Private; break;
            }
            Debug.Log("Step 3: PASSED. Uploading item generated and sent.");
            Workshop.Upload(wdata);
        }
        catch (Exception e)
        {
            Debug.Log($"Step 3: FAILED. Something went wrong when trying to generate uploading item.\n{e}");
            Helper.Alert(string.Format(Translate.Get("unknownpublisherror"), Translate.Get("helpcontact"), e));
        }
    }
    private void DeleteUnusedFiles(string path)
    {
        foreach(string filepath in GetFiles(path))
        {
            Debug.Log(filepath);
            try
            {
                File.Delete(filepath);
            }
            catch (Exception e)
            {
                Debug.Log($"PublishNovel/DeleteUnusedFiles: Could not delete file {filepath}.\n{e}");
            }
        }
    }
    public List<string> GetFiles(string path)
    {
        string[] extensions = { ".png", ".txt", ".ogg", ".json" };
        List<string> badFiles = new List<string>();
        foreach (string subpath in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
        {
            if (!extensions.Any(subpath.EndsWith))
                badFiles.Add(subpath);
        }
        return badFiles;
    }
}
