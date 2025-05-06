using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalLanguageManager : MonoBehaviour
{
    public static GlobalLanguageManager Instance;

    [Header("Настройки языков")]
    public List<string> languages = new List<string> { "en", "ru", "es" };
    public int defaultLanguageIndex = 0;

    [Header("Кнопки выбора языка (по индексу)")]
    public List<Button> languageButtons;

    private int selectedLanguageIndex;

    private const string LanguagePrefKey = "SelectedLanguage";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Сохраняем между сценами (опционально)
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        LoadLanguage();
        AssignButtonCallbacks();
    }

    /// <summary>
    /// Назначить колбэки кнопкам для выбора языка
    /// </summary>
    private void AssignButtonCallbacks()
    {
        for (int i = 0; i < languageButtons.Count; i++)
        {
            int index = i;
            languageButtons[i].onClick.AddListener(() => OnLanguageSelected(index));
        }
    }

    /// <summary>
    /// Вызывается при выборе языка через UI
    /// </summary>
    /// <param name="index">Индекс языка в списке</param>
    public void OnLanguageSelected(int index)
    {
        if (index >= 0 && index < languages.Count)
        {
            selectedLanguageIndex = index;
            PlayerPrefs.SetInt(LanguagePrefKey, selectedLanguageIndex);
            PlayerPrefs.Save();

            Debug.Log("Язык выбран: " + GetSelectedLanguage());
        }
        else
        {
            Debug.LogWarning("Недопустимый индекс языка: " + index);
        }
    }

    /// <summary>
    /// Загрузка языка из PlayerPrefs или установка по умолчанию
    /// </summary>
    public void LoadLanguage()
    {
        selectedLanguageIndex = PlayerPrefs.GetInt(LanguagePrefKey, defaultLanguageIndex);

        if (selectedLanguageIndex < 0 || selectedLanguageIndex >= languages.Count)
        {
            selectedLanguageIndex = defaultLanguageIndex;
        }

        Debug.Log("Загружен язык: " + GetSelectedLanguage());
    }

    /// <summary>
    /// Получить текущий выбранный код языка (например, "en", "ru")
    /// </summary>
    /// <returns>Код языка</returns>
    public string GetSelectedLanguage()
    {
        if (selectedLanguageIndex >= 0 && selectedLanguageIndex < languages.Count)
            return languages[selectedLanguageIndex];
        else
            return languages[defaultLanguageIndex];
    }
}
