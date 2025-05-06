using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using static UnityEngine.GraphicsBuffer;
//using Platinio;

public class StoryManager_tmp : MonoBehaviour
{
[SerializeField] private GameObject[] menuPopUps;
[System.Serializable]
    public class Story
    {
        public string storyName;
        public GameObject[] storyPrefabs;
//public AudioClip\[] storySounds;
public string[] bundleUrls;


    // Fields for handling audio URLs and audio clips by page
    public List<AudioUrlEntry> audioUrlsList;
        [NonSerialized] public Dictionary<string, string[]> audioUrls;

        public List<PageAudioClipEntry> pageAudioClipsList;
        [NonSerialized] public Dictionary<int, AudioClip> pageAudioClips;
    }

    [System.Serializable]
    public class AudioUrlEntry
    {
        public string language;
        public string[] urls;
    }

    [System.Serializable]
    public class PageAudioClipEntry
    {
        public int pageIndex;
        public AudioClip clip;
    }


    public Story[] stories;
    public Transform instantiateParent;
    public Text pageCounterText;

    public GameObject topMenu;

    private GameObject currentPrefab;
    public GameObject audioObject; // Reference to the single audio GameObject for playing Audio for pages
    private AudioSource audioSource;
    private int currentStoryIndex = -1;
    private int currentPrefabIndex = 0;
    //private string jsonFileName = "AudioInfo.json";
    //private const string AssetInfoFileName = "AssetInfo.json";
    private Dictionary<int, HashSet<int>> loadedAssets = new Dictionary<int, HashSet<int>>();

    //public PurchaseManager purchaseManager;
    private DownloadManager downloadManager;
    private string language;
    private string filePath;
    private GlobalLanguageManager languageManager;
    private Dictionary<int, Dictionary<int, string>> audioPaths = new Dictionary<int, Dictionary<int, string>>();
    //private const string AudioPathsFileName = "AudioPaths.json";

    //page count
    public int currentPage;


    //Game object
    public GameObject game;


    //Not working shit for audo imfo saving ////////////////////////////////////////////////////////////////////////////////////////
    [System.Serializable]
    public class AudioPathInfo
    {
        public int storyIndex;
        public Dictionary<int, string> pageAudioPaths;
    }
    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>


    // New stuff for trying loading the  audio info in json /////////////////////////////////////////////////////////////////////////
    [System.Serializable]
    public class AudioPathEntry
    {
        public int storyIndex;
        public int pageIndex;
        public string audioFilePath;
    }

    [System.Serializable]
    public class AudioPathList
    {
        public List<AudioPathEntry> list;
    }


    [System.Serializable]
    public class SerializationHelper<T>
    {
        public List<T> list;
        public SerializationHelper(List<T> list)
        {
            this.list = list;
        }
    }

    private const string AudioInfoFileName = "AudioInfo.json";

    //private Dictionary<int, Dictionary<int, string>> audioFilePaths;
    private Dictionary<(int, int, string), string> audioFilePaths;

    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>



    void Start()
    {
        downloadManager = FindObjectOfType<DownloadManager>();
        audioSource = GetComponent<AudioSource>();

        //LoadAssetInfo();
        AutoFillStorySlots();

        //LoadAudioPaths();  // Загружаем пути к аудиофайлам из JSON
        //LoadAudioPathsFromJson();
        LoadAudioFromJson();
        //LoadLocalAudioClips(currentStoryIndex);  // Загружаем аудио при запуске

        // Initialize audio dictionaries from lists, with error handling for null references
        foreach (var story in stories)
        {
            story.audioUrls = ConvertAudioUrlsListToDict(story.audioUrlsList);
            story.pageAudioClips = ConvertPageAudioClipsListToDict(story.pageAudioClipsList);

        }

        languageManager = FindObjectOfType<GlobalLanguageManager>();
        LanguageSelect();

        Debug.Log("StoryManager initialized.");
    }

    /*
    public void PopUpMenu()
    {
        foreach (var popUpObj in menuPopUps)
        {
            Platinio.MenuPopUp popUp = popUpObj.GetComponent<Platinio.MenuPopUp>();
            if (popUp != null) popUp.Toggle();
            else Debug.LogWarning("MenuPopUp не найден на объекте " + popUpObj.name);
        }
    }
    */

    public void LanguageSelect()
    {
        if (languageManager != null)
        {
            // Access the selected language
            string selectedLanguage = languageManager.GetSelectedLanguage();
            language = selectedLanguage;
            Debug.Log("Current selected language: " + language);
        }
        else
        {
            Debug.LogWarning("GlobalLanguageManager component not found.");
        }
    }

    //Stop Sound for maim menu
    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    public void StopMainSoundTrack()
    {
        AudioSource audioSource = game.GetComponent<AudioSource>();
        //audioSource.Play(); // Начать воспроизведение
        audioSource.Pause(); // Остановить воспроизведение
    }
    /// <summary>
    /// /////////////////////////////////////////////////////////////////////////////////////////////////////////



    // Conversion helper methods
    private Dictionary<string, string[]> ConvertAudioUrlsListToDict(List<AudioUrlEntry> audioUrlsList)
    {
        var audioUrls = new Dictionary<string, string[]>();
        if (audioUrlsList != null)
        {
            foreach (var entry in audioUrlsList)
            {
                if (!audioUrls.ContainsKey(entry.language))
                {
                    audioUrls[entry.language] = entry.urls;
                }
            }
        }
        return audioUrls;
    }

    private Dictionary<int, AudioClip> ConvertPageAudioClipsListToDict(List<PageAudioClipEntry> pageAudioClipsList)
    {
        var pageAudioClips = new Dictionary<int, AudioClip>();
        if (pageAudioClipsList != null)
        {
            foreach (var entry in pageAudioClipsList)
            {
                if (!pageAudioClips.ContainsKey(entry.pageIndex))
                {
                    pageAudioClips[entry.pageIndex] = entry.clip;
                }
            }
        }
        return pageAudioClips;
    }


    public void DownloadPurchasedStoryAssets(int storyIndex)
    {
        /*
        //PopUp The top menu/////////////////////////////////////////////////////////////////
        Platinio.MenuPopUp popUpTopMenu = menuPopUps[1].GetComponent<Platinio.MenuPopUp>();
        if (popUpTopMenu != null)
        {
            popUpTopMenu.Toggle();
            Debug.LogWarning("Should load the level and should be topMenu PopUping!");
        }
        */

       
            LanguageSelect();
            LoadAudioFromJson();
            Debug.Log($"Downloading file: {filePath} for language: {language}");
            //StartCoroutine(downloadManager.DownloadStoryAssets(storyIndex, stories[storyIndex]));

            /*
            //PopUp The top menu/////////////////////////////////////////////////////////////////
            Platinio.MenuPopUp popUpTopMenu = menuPopUps[0].GetComponent<Platinio.MenuPopUp>();
            if (popUpTopMenu != null)
            {
                popUpTopMenu.Toggle();
                Debug.LogWarning("Should load the level and should be topMenu PopUping!");
            }

            else 
            {
                Debug.LogError("WHERE IS THE TOP MENU PopUping!?");
            }
            /////////////////////////////////////////////////////////////////////////////////////
            */
       
    }

    /*
    void DestroyAllChildren()
    {
        GameObject prefabHolder = GameObject.Find("PrefabHolder");

        if (prefabHolder != null)
        {
            foreach (Transform child in prefabHolder.transform)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            Debug.LogError("PrefabHolder not found in the scene!");
        }
    }
    */

    public void LoadNextPrefab()
    {
        if (currentStoryIndex == -1)
        {
            Debug.LogError("No story selected to load the next prefab.");
            return;
        }

        if (currentPrefabIndex >= stories[currentStoryIndex].storyPrefabs.Length)
        {
            Debug.Log("All prefabs in the story are loaded.");
            return;
        }

        if (stories[currentStoryIndex].storyPrefabs[currentPrefabIndex] != null)
        {
            GameObject flashlight = GameObject.Find("flashlight");
            //GameObject flash_light = GameObject.Find("flash_light");
            if (flashlight != null)
            {
                Destroy(flashlight);
            }
            else
            {
                Debug.LogWarning("flashlight not found in the scene!");
            }
            //Debug.LogWarning("flash_light local coordinates are " + flash_light.transform.localPosition);
            InstantiatePage(currentStoryIndex, currentPrefabIndex);
        }
    }

    public void LoadPreviousPrefab()
    {
        if (currentPrefabIndex > 0)
        {
            if (currentStoryIndex == -1 || currentPrefabIndex <= 1)
            {
                Debug.LogWarning("No previous page to load.");
                return;
            }
            currentPrefabIndex -= 2;
            LoadNextPrefab();
        }
    }

    public void LoadStory(int storyIndex)
    {
        if (storyIndex < 0 || storyIndex >= stories.Length)
        {
            Debug.LogError("Invalid story index: " + storyIndex);
            return;
        }

        currentStoryIndex = storyIndex;
        currentPrefabIndex = 0;
        LoadNextPrefab();
        UpdatePageCounter();
    }

    // Вызывается из DownloadManager Надо проверить загрузку ассетов после внесённых изменений в логику загрузки ассетов, если отсуттсвие функции никак не влияет, можно закоментировать
    public void SetDownloadedPrefab(int storyIndex, int pageIndex, GameObject prefab)
    {
        stories[storyIndex].storyPrefabs[pageIndex] = prefab;

        if (!loadedAssets.ContainsKey(storyIndex))
        {
            loadedAssets[storyIndex] = new HashSet<int>();
        }
        loadedAssets[storyIndex].Add(pageIndex);
        Debug.Log("wrighting downloded assets");
    }
    /// <summary>
    /// ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>


    private void AutoFillStorySlots()
    {
        for (int storyIndex = 0; storyIndex < stories.Length; storyIndex++)
        {
            string storyFolder = GetStoryFolderPath(storyIndex);
            if (!Directory.Exists(storyFolder))
            {
                Directory.CreateDirectory(storyFolder);
            }

            for (int pageIndex = 0; pageIndex < stories[storyIndex].storyPrefabs.Length; pageIndex++)
            {
                if (stories[storyIndex].storyPrefabs[pageIndex] == null)
                {
                    string bundleFile = Path.Combine(storyFolder, $"{stories[storyIndex].storyName}_{storyIndex:D2}_sc_{pageIndex:D2}.bundle");
                    if (File.Exists(bundleFile))
                    {
                        AssetBundle bundle = AssetBundle.LoadFromFile(bundleFile);
                        if (bundle != null)
                        {
                            GameObject prefab = bundle.LoadAsset<GameObject>($"{stories[storyIndex].storyName}_{storyIndex:D2}_sc_{pageIndex:D2}");
                            stories[storyIndex].storyPrefabs[pageIndex] = prefab;
                            bundle.Unload(false);
                        }
                    }
                }
            }
        }



    }



    public void LoadLocalAudioClips(int storyIndex)
    {
        if (!audioPaths.ContainsKey(storyIndex)) return;

        foreach (var pagePath in audioPaths[storyIndex])
        {
            int pageIndex = pagePath.Key;
            string audioFilePath = pagePath.Value;

            if (File.Exists(audioFilePath))
            {
                StartCoroutine(LoadAudioClip(audioFilePath, clip =>
                {
                    if (clip != null)
                    {
                        if (!stories[storyIndex].pageAudioClips.ContainsKey(pageIndex))
                        {
                            stories[storyIndex].pageAudioClips[pageIndex] = clip;
                            Debug.Log($"Loaded audio for story {storyIndex}, page {pageIndex}");
                        }
                    }
                }));
            }
        }
    }


    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>
    /// </param>



    public void LoadAudioFromJson()
    {
        string jsonPath = Path.Combine(Application.persistentDataPath, AudioInfoFileName);

        Debug.Log($"Attempting to load audio paths from JSON at path: {jsonPath}");

        if (!File.Exists(jsonPath))
        {
            Debug.LogWarning("No audio paths JSON file found at " + jsonPath);
            return;
        }

        string jsonContent = File.ReadAllText(jsonPath);
        //Debug.Log($"JSON content: {jsonContent}");

        AudioPathList audioPathList = JsonUtility.FromJson<AudioPathList>(jsonContent);

        // Initialize the dictionary if not already done
        if (audioPaths == null)
        {
            audioPaths = new Dictionary<int, Dictionary<int, string>>();
        }

        // Check for entries in the JSON file
        if (audioPathList.list == null || audioPathList.list.Count == 0)
        {
            Debug.LogWarning("No entries found in the JSON audio path list.");
            return;
        }
        else
        {
            Debug.Log($"Number of audio path entries: {audioPathList.list.Count}");
        }

        // Перебираем список и заполняем словарь
        foreach (var entry in audioPathList.list)
        {
            Debug.Log($"Processing entry - StoryIndex: {entry.storyIndex}, PageIndex: {entry.pageIndex}, AudioFilePath: {entry.audioFilePath}");

            if (!audioPaths.ContainsKey(entry.storyIndex))
            {
                audioPaths[entry.storyIndex] = new Dictionary<int, string>();
                Debug.Log($"Initialized dictionary for story index {entry.storyIndex}.");
            }

            audioPaths[entry.storyIndex][entry.pageIndex] = entry.audioFilePath;

            // Загружаем аудиоклип и добавляем его в pageAudioClips истории
            StartCoroutine(LoadAudioClip(entry.audioFilePath, clip =>
            {
                if (entry.storyIndex >= 0 && entry.storyIndex < stories.Length && stories[entry.storyIndex].pageAudioClips != null)
                {
                    stories[entry.storyIndex].pageAudioClips[entry.pageIndex] = clip;
                    Debug.Log($"Audio clip assigned for Story {entry.storyIndex}, Page {entry.pageIndex}");
                }
                else
                {
                    Debug.LogWarning($"Invalid story index {entry.storyIndex} or pageAudioClips not initialized.");
                }

            }));
        }

        Debug.Log("Audio paths successfully loaded and assigned from JSON.");
    }



    private void LoadLocalAudioFilesForStory(int storyIndex)
    {
        if (storyIndex < 0 || storyIndex >= stories.Length)
        {
            Debug.LogError("Invalid story index: " + storyIndex);
            return;
        }

        string audioFolderPath = Path.Combine(Application.persistentDataPath, storyIndex.ToString(), language);

        if (!Directory.Exists(audioFolderPath))
        {
            Debug.LogWarning($"Audio folder for story {storyIndex} does not exist at path: {audioFolderPath}");
            return;
        }

        foreach (var pagePrefab in stories[storyIndex].storyPrefabs)
        {
            if (pagePrefab == null) continue;

            string pageName = pagePrefab.name; // Имя префаба страницы используется для поиска аудиофайла
            string audioFilePath = Path.Combine(audioFolderPath, pageName + ".mp3");

            if (File.Exists(audioFilePath))
            {
                int pageIndex = Array.IndexOf(stories[storyIndex].storyPrefabs, pagePrefab);
                StartCoroutine(LoadAudioClip(audioFilePath, clip =>
                {
                    if (clip != null)
                    {
                        // Добавляем загруженный аудиоклип в словарь pageAudioClips по индексу страницы
                        stories[storyIndex].pageAudioClips[pageIndex] = clip;
                        Debug.Log($"Loaded audio for story {storyIndex}, page {pageIndex}");
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to load audio clip for story {storyIndex}, page {pageIndex}");
                    }
                }));
            }
            else
            {
                Debug.LogWarning($"Audio file not found for story {storyIndex}, page {pageName} at path: {audioFilePath}");
            }
        }
    }


    // Модифицированный метод для загрузки аудиоклипа
    private IEnumerator LoadAudioClip(string path, System.Action<AudioClip> callback)
    {
        using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip("file://" + path, AudioType.MPEG))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                callback?.Invoke(clip);
            }
            else
            {
                Debug.LogError($"Error loading audio clip from {path}: {request.error}");
                callback?.Invoke(null);
            }
        }
    }



    // Вызываем LoadLocalAudioFilesForStory перед PlayStorySound, чтобы убедиться, что аудио загружено
    private void PlayStorySound(int storyIndex, int pageIndex)
    {
        if (stories[storyIndex].pageAudioClips == null || !stories[storyIndex].pageAudioClips.ContainsKey(pageIndex))
        {
            //LoadLocalAudioFilesForStory(storyIndex); // Загрузка аудио, если его нет в словаре
            LoadLocalAudioClips(storyIndex); // Загрузка аудио, если его нет в словаре
        }

        if (stories[storyIndex].pageAudioClips.TryGetValue(pageIndex, out AudioClip clip))
        {
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning($"AudioClip for story {storyIndex} page {pageIndex} is missing.");
            }
        }
        else
        {
            Debug.LogWarning("No sound available for page: " + pageIndex);
        }
    }


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////



    public string GetStoryFolderPath(int storyIndex)
    {
        return Path.Combine(Application.persistentDataPath, storyIndex.ToString());
    }



    public void InstantiatePage(int storyIndex, int pageIndex)
    {
        if (currentPrefab != null)
        {
            Destroy(currentPrefab);
        }

        // Проверка и загрузка аудиофайлов для текущей страницы, если они отсутствуют в словаре
        if (stories[storyIndex].pageAudioClips == null || !stories[storyIndex].pageAudioClips.ContainsKey(pageIndex))
        {
            // Загрузка локальных аудиофайлов для страницы текущей истории
            //LoadLocalAudioFilesForStory(storyIndex);
            LoadLocalAudioClips(storyIndex);

        }

        // Продолжаем инстанцирование страницы
        if (stories[storyIndex].storyPrefabs[pageIndex] != null)
        {
            currentPrefab = Instantiate(stories[storyIndex].storyPrefabs[pageIndex], instantiateParent);
            currentPrefabIndex = pageIndex + 1;
            UpdatePageCounter();

            // После инстанцирования страницы, проверяем и проигрываем соответствующий аудиоклип
            PlayStorySound(storyIndex, pageIndex);
        }
        else
        {
            Debug.LogError("Prefab for page not found.");
        }
    }





    public void UpdatePageCounter()
    {
        if (pageCounterText != null && currentStoryIndex != -1)
        {
            int totalPrefabs = stories[currentStoryIndex].storyPrefabs.Length;
            currentPage = Mathf.Clamp(currentPrefabIndex, 1, totalPrefabs);
            pageCounterText.text = $"Page {currentPage} / {totalPrefabs}";
            //Debug.Log(currentPage);
        }
    }


}
