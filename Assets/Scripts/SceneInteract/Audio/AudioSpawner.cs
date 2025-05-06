using UnityEngine;
using System.Collections;
using System.IO;

public class AudioSpawner : MonoBehaviour
{
    public string audioFileName = "mySound.mp3";  // Название файла в StreamingAssets
    public GameObject audioPrefab;                // Префаб с AudioSource

    private bool isPlaying = false;

    void OnMouseDown()
    {
        if (!isPlaying)
        {
            StartCoroutine(PlayAudioFromStreamingAssets());
        }
    }

    IEnumerator PlayAudioFromStreamingAssets()
    {
        isPlaying = true;

        string filePath = Path.Combine(Application.streamingAssetsPath, audioFileName);

#if UNITY_ANDROID && !UNITY_EDITOR
        using (var www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(filePath, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError("Ошибка загрузки аудио: " + www.error);
                isPlaying = false;
                yield break;
            }

            AudioClip clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);
            yield return StartCoroutine(SpawnAndPlayAudio(clip));
        }
#else
        string url = "file://" + filePath;
        using (var www = UnityEngine.Networking.UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Debug.LogError("Ошибка загрузки аудио: " + www.error);
                isPlaying = false;
                yield break;
            }

            AudioClip clip = UnityEngine.Networking.DownloadHandlerAudioClip.GetContent(www);
            yield return StartCoroutine(SpawnAndPlayAudio(clip));
        }
#endif
    }

    IEnumerator SpawnAndPlayAudio(AudioClip clip)
    {
        GameObject obj = Instantiate(audioPrefab);
        AudioSource source = obj.GetComponent<AudioSource>();
        if (source == null)
        {
            source = obj.AddComponent<AudioSource>();
        }

        source.clip = clip;
        source.Play();

        yield return new WaitForSeconds(clip.length + 0.1f);

        Destroy(obj);
        isPlaying = false;
    }
}
