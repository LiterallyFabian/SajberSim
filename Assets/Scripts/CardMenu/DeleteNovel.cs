using SajberSim.Translation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DeleteNovel : MonoBehaviour
{
    public Text GotIt;
    public Text DeleteText;
    public string path;
    public Manifest data;
    private void Start()
    {
        DeleteText.text = string.Format(Translate.Get("deletenovel"), data.name);
    }
    public void Delete()
    {
        Debug.Log(path);
        StartCoroutine(ActuallyDelete());
        GotIt.text = Translate.Get("gotit");
    }
    public void Cancel()
    {
        this.gameObject.transform.localScale = Vector3.zero;
    }
    IEnumerator ActuallyDelete()
    {
        DeleteText.text = string.Format(Translate.Get("deletingnovel"), data.name);
        yield return new WaitForSeconds(60);
        Debug.Log($"DeleteNovel/ActuallyDelete: Deleted {path}.");
        Directory.Delete(path, true);
        Destroy(this.gameObject, 1);
    }
}

