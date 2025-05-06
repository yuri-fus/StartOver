using UnityEngine;
using System.Collections;
using System.IO;

public class AudioSwitcher : MonoBehaviour
{
    public AudioSource targetSource;                // Назначенный AudioSource в инспекторе
    public string alternateClipFileName = "alt.mp3"; // Название файла в StreamingAssets

    private AudioClip originalClip;
    private AudioClip alternateClip;
    private bool isUsingAlternate = false;
    private bool isLoading = false;

    void Start()
    {
        if (targetSource == null)
        {
            Debug.LogError("AudioSource не назначен!");
        }
        else
        {
            originalClip = targetSource.clip;
        }
    }

    void OnMouseDown()
    {
        if (!isLoading)
        {
            if (!isUsingAlternate)
            {
                StartCoroutine(LoadAndSwitchToAlternate());
            }
            else
            {
                SwitchToOriginal();
            }
        }
    }

    IEnumerator LoadAndSwitchToAlternate()
    {
        isLoading = true;

        string path = Path.Combine(Application.streamingAssetsPath, alternateClipFileName);
#if UNITY_ANDROID && !UNITY_EDITOR
        using (var www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(path, AudioType.MPEG))
#else
        using (var www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
#endif
        {
            yield return www.SendWebRequest();

            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError("Ошибка загрузки аудио: " + www.error);
                isLoading = false;
                yield break;
            }

            alternateClip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);
            targetSource.clip = alternateClip;
            targetSource.Play();
            isUsingAlternate = true;
            isLoading = false;
        }
    }

    void SwitchToOriginal()
    {
        if (originalClip != null)
        {
            targetSource.clip = originalClip;
            targetSource.Play();
            isUsingAlternate = false;
        }
    }
}
