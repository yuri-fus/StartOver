using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleManager : MonoBehaviour
{
    public string baseURL;
    public string[] assetBundleNames;
    private AssetBundle[] loadedBundles;

    public void PrepareForDownload(string language)
    {
        baseURL = $"https://yourserver.com/AssetBundles/{language}/";
    }

    public void StartDownload()
    {
        StartCoroutine(DownloadBundles());
    }

    private IEnumerator DownloadBundles()
    {
        loadedBundles = new AssetBundle[assetBundleNames.Length];

        for (int i = 0; i < assetBundleNames.Length; i++)
        {
            string url = baseURL + assetBundleNames[i];
            UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download {assetBundleNames[i]}: {www.error}");
            }
            else
            {
                loadedBundles[i] = DownloadHandlerAssetBundle.GetContent(www);
            }
        }
    }

    public bool IsDownloadComplete()
    {
        foreach (var bundle in loadedBundles)
        {
            if (bundle == null)
                return false;
        }
        return true;
    }

    public float GetProgress()
    {
        int count = 0;
        foreach (var bundle in loadedBundles)
        {
            if (bundle != null)
                count++;
        }
        return (float)count / assetBundleNames.Length;
    }

    public AssetBundle GetBundle(string name)
    {
        for (int i = 0; i < assetBundleNames.Length; i++)
        {
            if (assetBundleNames[i] == name)
                return loadedBundles[i];
        }
        return null;
    }
}
