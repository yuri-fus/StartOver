using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalLanguageManager : MonoBehaviour
{
    public static GlobalLanguageManager Instance;

    [Header("��������� ������")]
    public List<string> languages = new List<string> { "en", "ru", "es" };
    public int defaultLanguageIndex = 0;

    [Header("������ ������ ����� (�� �������)")]
    public List<Button> languageButtons;

    private int selectedLanguageIndex;

    private const string LanguagePrefKey = "SelectedLanguage";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ��������� ����� ������� (�����������)
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
    /// ��������� ������� ������� ��� ������ �����
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
    /// ���������� ��� ������ ����� ����� UI
    /// </summary>
    /// <param name="index">������ ����� � ������</param>
    public void OnLanguageSelected(int index)
    {
        if (index >= 0 && index < languages.Count)
        {
            selectedLanguageIndex = index;
            PlayerPrefs.SetInt(LanguagePrefKey, selectedLanguageIndex);
            PlayerPrefs.Save();

            Debug.Log("���� ������: " + GetSelectedLanguage());
        }
        else
        {
            Debug.LogWarning("������������ ������ �����: " + index);
        }
    }

    /// <summary>
    /// �������� ����� �� PlayerPrefs ��� ��������� �� ���������
    /// </summary>
    public void LoadLanguage()
    {
        selectedLanguageIndex = PlayerPrefs.GetInt(LanguagePrefKey, defaultLanguageIndex);

        if (selectedLanguageIndex < 0 || selectedLanguageIndex >= languages.Count)
        {
            selectedLanguageIndex = defaultLanguageIndex;
        }

        Debug.Log("�������� ����: " + GetSelectedLanguage());
    }

    /// <summary>
    /// �������� ������� ��������� ��� ����� (��������, "en", "ru")
    /// </summary>
    /// <returns>��� �����</returns>
    public string GetSelectedLanguage()
    {
        if (selectedLanguageIndex >= 0 && selectedLanguageIndex < languages.Count)
            return languages[selectedLanguageIndex];
        else
            return languages[defaultLanguageIndex];
    }
}
