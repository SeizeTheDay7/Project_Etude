// LocaleDropdown.cs

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

public class LocaleDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    IEnumerator Start()
    {
        // Wait for the localization system to initialize, loading Locales, preloading etc.
        yield return LocalizationSettings.InitializationOperation;

        // Generate list of available Locales
        var options = new List<TMP_Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            string displayName = GetLocaleDisplayName(locale.Identifier.Code);
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
            options.Add(new TMP_Dropdown.OptionData(displayName));
        }
        dropdown.options = options;

        dropdown.value = selected;
        dropdown.onValueChanged.AddListener(LocaleSelected);
    }

    static void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    string GetLocaleDisplayName(string localeCode)
    {
        switch (localeCode)
        {
            case "ko":
                return "한국어";
            case "en":
                return "English";
            case "zh-Hans":
                return "简体中文";
            case "zh-Hant":
                return "繁體中文";
            case "ja":
                return "日本語";
            case "ru":
                return "Русский";
            // Add more cases as needed
            default:
                return localeCode;
        }
    }
}