using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class DownloadManager_tmp : MonoBehaviour
{
    [SerializeField] private GameObject[] menuPopUps;
    public FadeMenu fadeOut_LoginUI;

    public GameObject downloadProgress;
    [SerializeField] private GameObject assetBundleProgressPrefab;
    [SerializeField] private GameObject audioProgressPrefab;
    [SerializeField] private GameObject noInternetPopup;

    [SerializeField]
    private bool isActive = true;
    public bool IsActive
    {
        get { return isActive; }
        set { isActive = value; }
    }

    private Text assetBundleProgressText;
    private Text audioProgressText;
    private Image progressBarImage;

    private int totalAssetBundles;
    private int downloadedAssetBundles;

    private int totalAudioFiles;
    private int downloadedAudioFiles;

    private StoryManager storyManager;
    private AudioDataManager audioDataManager;
    private GlobalLanguageManager languageManager;
    private string languageCode;

    public Canvas uiCanvas;
    public GameObject pauseMenu;
    public GameObject langMenu;

    public GameObject menuScroll;
    public GameObject tap_Button;
    public GameObject prefabHolder;

    void Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            isActive = false;
        }

        GameObject assetBundleProgress = Instantiate(assetBundleProgressPrefab, uiCanvas.transform);
        assetBundleProgressText = assetBundleProgress.GetComponent<Text>();

        GameObject audioProgress = Instantiate(audioProgressPrefab, uiCanvas.transform);
        audioProgressText = audioProgress.GetComponent<Text>();

        GameObject progressBar = Instantiate(downloadProgress, uiCanvas.transform);
        progressBarImage = progressBar.GetComponent<Image>();
        progressBarImage.fillAmount = 0f;

        storyManager = FindObjectOfType<StoryManager>();
        audioDataManager = FindObjectOfType<AudioDataManager>();
        languageManager = FindObjectOfType<GlobalLanguageManager>();
    }

    public void DownloadNewLanguage()
    {
        if (languageManager != null)
        {
            languageCode = languageManager.GetSelectedLanguage();
            languageManager.LoadLanguage();
        }
        else
        {
            Debug.LogWarning("GlobalLanguageManager component not found.");
        }
    }

    public IEnumerator DownloadStoryAssets(int storyIndex, StoryManager.Story story)
    {
        DownloadNewLanguage();
        Debug.Log("Current selected language: " + languageCode);
        totalAssetBundles = story.bundleUrls.Length;
        totalAudioFiles = story.audioUrls[languageCode].Length;

        downloadedAssetBundles = 0;
        downloadedAudioFiles = 0;

        int totalFiles = totalAssetBundles + totalAudioFiles;
        int downloadedFiles = 0;

        bool allAssetBundlesExist = true;
        bool allAudioFilesExist = true;

        // Проверка наличия всех локальных AssetBundle-файлов
        for (int i = 0; i < totalAssetBundles; i++)
        {
            string assetName = $"{story.storyName}_{storyIndex:D2}_sc_{i:D2}";
            string filePath = Path.Combine(storyManager.GetStoryFolderPath(storyIndex), $"{assetName}.bundle");

            if (!File.Exists(filePath))
            {
                allAssetBundlesExist = false;
                break;
            }
        }

        // Проверка наличия всех локальных аудиофайлов
        for (int i = 0; i < totalAudioFiles; i++)
        {
            string languagePath = Path.Combine(storyManager.GetStoryFolderPath(storyIndex), languageCode);
            string audioFileName = $"{story.storyName}_{storyIndex:D2}_{languageCode}_sc_{i:D2}.mp3";
            string audioFilePath = Path.Combine(languagePath, audioFileName);

            if (!File.Exists(audioFilePath))
            {
                allAudioFilesExist = false;
                break;
            }
        }

        // Проверка интернет-соединения
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            Debug.LogWarning("Нет подключения к интернету.");

            if (allAssetBundlesExist && allAudioFilesExist)
            {
                Debug.Log("Все ассеты найдены локально. Загружаем из локального хранилища.");
                isActive = false;
                LoadStory(storyIndex);
                yield break;
            }
            else
            {
                isActive = false;
                if (noInternetPopup != null)
                {
                    noInternetPopup.SetActive(true);
                }
                yield break;
            }
        }
        else
        {
            isActive = true;
        }

        // Показ прогресса
        assetBundleProgressText.gameObject.SetActive(true);
        audioProgressText.gameObject.SetActive(true);
        progressBarImage.gameObject.SetActive(true);

        // Загрузка AssetBundles
        for (int pageIndex = 0; pageIndex < totalAssetBundles; pageIndex++)
        {
            string bundleUrl = story.bundleUrls[pageIndex];
            string assetName = $"{story.storyName}_{storyIndex:D2}_sc_{pageIndex:D2}";
            string filePath = Path.Combine(storyManager.GetStoryFolderPath(storyIndex), $"{assetName}.bundle");

            if (!File.Exists(filePath))
            {
                using (UnityWebRequest request = UnityWebRequest.Get(bundleUrl))
                {
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                        float currentProgress = (float)(downloadedFiles + request.downloadProgress) / totalFiles;
                        UpdateCombinedProgress(currentProgress);
                        yield return null;
                    }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        File.WriteAllBytes(filePath, request.downloadHandler.data);
                        AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
                        if (bundle != null)
                        {
                            GameObject prefab = bundle.LoadAsset<GameObject>(assetName);
                            if (prefab != null)
                            {
                                storyManager.SetDownloadedPrefab(storyIndex, pageIndex, prefab);
                            }
                            bundle.Unload(false);
                        }
                    }
                    else
                    {
                        Debug.LogError("Error downloading AssetBundle: " + request.error);
                    }
                }
            }

            downloadedAssetBundles++;
            downloadedFiles++;
        }

        // Загрузка аудиофайлов
        for (int pageIndex = 0; pageIndex < totalAudioFiles; pageIndex++)
        {
            string languagePath = Path.Combine(storyManager.GetStoryFolderPath(storyIndex), languageCode);
            string audioFileName = $"{story.storyName}_{storyIndex:D2}_{languageCode}_sc_{pageIndex:D2}.mp3";
            string audioUrl = story.audioUrls[languageCode][pageIndex];

            if (!Directory.Exists(languagePath))
            {
                Directory.CreateDirectory(languagePath);
            }

            string audioFilePath = Path.Combine(languagePath, audioFileName);

            if (!File.Exists(audioFilePath))
            {
                using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.MPEG))
                {
                    request.SendWebRequest();
                    while (!request.isDone)
                    {
                        float currentProgress = (float)(downloadedFiles + request.downloadProgress) / totalFiles;
                        UpdateCombinedProgress(currentProgress);
                        yield return null;
                    }

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        File.WriteAllBytes(audioFilePath, request.downloadHandler.data);
                    }
                    else
                    {
                        Debug.LogError($"Ошибка скачивания аудио: {request.error}");
                    }
                }
            }

            SaveAudioFileEntry(storyIndex, pageIndex, audioFilePath);
            downloadedAudioFiles++;
            downloadedFiles++;
        }

        UpdateCombinedProgress(1f);
        yield return new WaitForSeconds(1f);

        assetBundleProgressText.gameObject.SetActive(false);
        audioProgressText.gameObject.SetActive(false);
        progressBarImage.gameObject.SetActive(false);

        Platinio.MenuPopUp popUpLang = menuPopUps[3].GetComponent<Platinio.MenuPopUp>();
        langMenu.gameObject.SetActive(false);
        if (popUpLang != null) popUpLang.Toggle();

        yield return new WaitForSeconds(1.5f);

        LoadStory(storyIndex);
        pauseMenu.gameObject.SetActive(true);
    }

    private void UpdateCombinedProgress(float progress)
    {
        int percent = Mathf.RoundToInt(progress * 100f);
        assetBundleProgressText.text = percent + "%";
        audioProgressText.text = percent + "%";
        progressBarImage.fillAmount = progress;
    }

    private void SaveAudioFileEntry(int storyIndex, int pageIndex, string path)
    {
        if (audioDataManager != null)
        {
            audioDataManager.SaveAudioFileDataToJson(storyIndex, pageIndex, path);
        }
    }

    public void SaveAllAudioFileDataToJson(int storyIndex, StoryManager.Story story)
    {
        if (audioDataManager == null || story.audioUrls == null || !story.audioUrls.ContainsKey(languageCode))
        {
            Debug.LogWarning("Не удалось сохранить аудиоданные в JSON — нет данных или компонента.");
            return;
        }

        int totalAudioFiles = story.audioUrls[languageCode].Length;

        for (int pageIndex = 0; pageIndex < totalAudioFiles; pageIndex++)
        {
            string languagePath = Path.Combine(storyManager.GetStoryFolderPath(storyIndex), languageCode);
            string audioFileName = $"{story.storyName}_{storyIndex:D2}_{languageCode}_sc_{pageIndex:D2}.mp3";
            string audioFilePath = Path.Combine(languagePath, audioFileName);

            if (File.Exists(audioFilePath))
            {
                SaveAudioFileEntry(storyIndex, pageIndex, audioFilePath);
            }
        }

        Debug.Log("JSON обновлён для всех аудиофайлов.");
    }

    public void LoadStory(int index)
    {
        storyManager.LoadStory(index);
        storyManager.LoadLocalAudioClips(index);
        fadeOut_LoginUI.FadeOut();
        PopUpMenu();
    }

    public void PopUpMenu()
    {
        foreach (var popUpObj in menuPopUps)
        {
            Platinio.MenuPopUp popUp = popUpObj.GetComponent<Platinio.MenuPopUp>();
            if (popUp != null) popUp.Toggle();
            else Debug.LogWarning("MenuPopUp не найден на объекте " + popUpObj.name);
        }
    }
}
