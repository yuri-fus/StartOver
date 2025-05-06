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
            DontDestroyOnLoad(gameObject); // Опционально
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
    /// Сохранить запись об аудиофайле и перезаписать JSON-файл.
    /// </summary>
    public void SaveAudioFileDataToJson(int storyIndex, int pageIndex, string audioFilePath)
    {
        // Проверка, существует ли уже запись с таким storyIndex и pageIndex
        AudioFileEntry existingEntry = audioFileList.list.Find(entry =>
            entry.storyIndex == storyIndex && entry.pageIndex == pageIndex);

        if (existingEntry != null)
        {
            existingEntry.audioFilePath = audioFilePath; // Обновляем путь
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
    /// Загрузить данные из JSON-файла.
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
    /// Получить путь к аудиофайлу по индексу истории и страницы.
    /// </summary>
    public string GetAudioFilePath(int storyIndex, int pageIndex)
    {
        AudioFileEntry entry = audioFileList.list.Find(e =>
            e.storyIndex == storyIndex && e.pageIndex == pageIndex);

        return entry != null ? entry.audioFilePath : null;
    }

    /// <summary>
    /// Удалить все сохранённые данные (опционально)
    /// </summary>
    public void ClearAudioFileData()
    {
        audioFileList.list.Clear();
        if (File.Exists(jsonFilePath))
            File.Delete(jsonFilePath);
    }
}
