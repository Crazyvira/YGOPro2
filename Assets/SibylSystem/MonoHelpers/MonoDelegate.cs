using UnityEngine;
using System;
using System.Collections;
using System.IO;
public class MonoDelegate : MonoBehaviour
{
    public Action actionInMono;
    public void function()
    {
        if (actionInMono != null) actionInMono();
    }
}


public class MonoListener : MonoBehaviour
{
    public Action<GameObject> actionInMono;
    public void function()
    {
        if (actionInMono != null) actionInMono(this.gameObject);
    }
}

public class MonoListenerRMS_ized : MonoBehaviour
{
    public Action<GameObject, Servant.messageSystemValue> actionInMono;
    public Servant.messageSystemValue value;
    public void function()
    {
        UIInput input = GetComponent<UIInput>();
        if (input != null)
        {
            value = new Servant.messageSystemValue();
            value.hint = input.name;
            value.value = input.value;
        }
        if (actionInMono != null) actionInMono(this.gameObject, value);
    }
}

public class MonoDownloader : MonoBehaviour
{
    public event EventHandler DownloadForCloseUpCompleted;
    /// <summary>Occurs when [card download is completed].</summary>
    public event EventHandler DownloadCardCompleted;
    /// <summary>Occurs when [closeup download is completed].</summary>
    public event EventHandler DownloadCloseupCompleted;
    public void start(string url, string targetFile)
    {
        StartCoroutine(Download(url, targetFile));
    }
    IEnumerator Download(string url, string targetFile)
    {
        WWW request = new WWW(url);
        while (!request.isDone)
            yield return null;
        string folder = ShaCache.ToContaingFolder(targetFile);
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        if (request.error == null || request.error == "")
            File.WriteAllBytes(targetFile, request.bytes);
        if (DownloadCardCompleted != null)
        {
            DownloadCardCompleted(this, null);
        }
    }
}