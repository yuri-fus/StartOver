using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioDownloadManager : MonoBehaviour
{
    public string baseURL;
    public string[] audioFileNames;
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    public void PrepareForDownload(string language)
    {
        baseURL = $"https://yourserver.com/Audio/{language}/";
    }

    public void StartDownload()
    {
        StartCoroutine(DownloadAudioFiles());
    }

    private IEnumerator DownloadAudioFiles()
    {
        foreach (var fileName in audioFileNames)
        {
            string url = baseURL + fileName;
            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to download {fileName}: {www.error}");
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                audioClips[fileName] = clip;
            }
        }
    }

    public bool IsDownloadComplete()
    {
        return audioClips.Count == audioFileNames.Length;
    }

    public float GetProgress()
    {
        return (float)audioClips.Count / audioFileNames.Length;
    }

    public AudioClip GetAudioClip(string name)
    {
        if (audioClips.ContainsKey(name))
            return audioClips[name];
        return null;
    }
}
