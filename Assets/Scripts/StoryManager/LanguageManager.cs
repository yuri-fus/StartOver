using UnityEngine;
using UnityEngine.Localization.Settings;
using System.Collections;

public class LanguageManager : MonoBehaviour
{
    public void SetLanguage(string languageCode)
    {
        StartCoroutine(SetLocale(languageCode));
    }

    private IEnumerator SetLocale(string languageCode)
    {
        yield return LocalizationSettings.InitializationOperation;

        var locales = LocalizationSettings.AvailableLocales.Locales;
        foreach (var locale in locales)
        {
            if (locale.Identifier.Code == languageCode)
            {
                LocalizationSettings.SelectedLocale = locale;
                break;
            }
        }
    }

    public string GetCurrentLanguage()
    {
        return LocalizationSettings.SelectedLocale.Identifier.Code;
    }
}
