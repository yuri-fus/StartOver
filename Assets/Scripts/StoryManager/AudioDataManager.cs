using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class AudioFileEntry
{
    public int storyIndex;
    public int pageIndex;
    public string audioFilePath;
}

[System.Serializable]
public class AudioFileList
{
    public List<AudioFileEntry> list = new List<AudioFileEntry>();
}

public class AudioDataManager : MonoBehaviour
{
    public static AudioDataManager Instance;

    private string jsonFilePath;
    private AudioFileList audioFileList = new AudioFileList();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �����������
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        jsonFilePath = Path.Combine(Application.persistentDataPath, "AudioInfo.json");
        LoadAudioFileDataFromJson();
    }

    /// <summary>
    /// ��������� ������ �� ���������� � ������������ JSON-����.
    /// </summary>
    public void SaveAudioFileDataToJson(int storyIndex, int pageIndex, string audioFilePath)
    {
        // ��������, ���������� �� ��� ������ � ����� storyIndex � pageIndex
        AudioFileEntry existingEntry = audioFileList.list.Find(entry =>
            entry.storyIndex == storyIndex && entry.pageIndex == pageIndex);

        if (existingEntry != null)
        {
            existingEntry.audioFilePath = audioFilePath; // ��������� ����
        }
        else
        {
            AudioFileEntry newEntry = new AudioFileEntry
            {
                storyIndex = storyIndex,
                pageIndex = pageIndex,
                audioFilePath = audioFilePath
            };
            audioFileList.list.Add(newEntry);
        }

        string json = JsonUtility.ToJson(audioFileList, true);
        File.WriteAllText(jsonFilePath, json);
    }

    /// <summary>
    /// ��������� ������ �� JSON-�����.
    /// </summary>
    public void LoadAudioFileDataFromJson()
    {
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            audioFileList = JsonUtility.FromJson<AudioFileList>(json);
        }
        else
        {
            audioFileList = new AudioFileList();
        }
    }

    /// <summary>
    /// �������� ���� � ���������� �� ������� ������� � ��������.
    /// </summary>
    public string GetAudioFilePath(int storyIndex, int pageIndex)
    {
        AudioFileEntry entry = audioFileList.list.Find(e =>
            e.storyIndex == storyIndex && e.pageIndex == pageIndex);

        return entry != null ? entry.audioFilePath : null;
    }

    /// <summary>
    /// ������� ��� ���������� ������ (�����������)
    /// </summary>
    public void ClearAudioFileData()
    {
        audioFileList.list.Clear();
        if (File.Exists(jsonFilePath))
            File.Delete(jsonFilePath);
    }
}
