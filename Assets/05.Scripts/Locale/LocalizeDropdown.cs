// LocalizeDropdown.cs

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace UVH.Localization
{
    [RequireComponent(typeof(TMP_Dropdown))]
    [AddComponentMenu("Localization/Localize TMP Dropdown")]
    public class LocalizeDropdown : MonoBehaviour
    {

        [Serializable]
        public class LocalizedDropdownOption
        {
            public LocalizedString text;

            public LocalizedSprite sprite; //not implemented yet!
        }

        public List<LocalizedDropdownOption> options;
        public int selectedOptionIndex = 0;
        private Locale currentLocale = null;
        private TMP_Dropdown Dropdown => GetComponent<TMP_Dropdown>();


        private void Start()
        {
            getLocale();
            UpdateDropdown(currentLocale);
            LocalizationSettings.SelectedLocaleChanged += UpdateDropdown;
        }


        private void OnEnable() => LocalizationSettings.SelectedLocaleChanged += UpdateDropdown;
        private void OnDisable() => LocalizationSettings.SelectedLocaleChanged -= UpdateDropdown;
        void OnDestroy() => LocalizationSettings.SelectedLocaleChanged -= UpdateDropdown;



        private void getLocale()
        {
            var locale = LocalizationSettings.SelectedLocale;
            if (currentLocale != null && locale != currentLocale)
            {
                currentLocale = locale;
            }
        }


        private void UpdateDropdown(Locale locale)
        {
            selectedOptionIndex = Dropdown.value;
            Dropdown.ClearOptions();

            for (int i = 0; i < options.Count; i++)
            {
                //sprite functionality isn't ready yet.
                Sprite localizedSprite = null;

                String localizedText = options[i].text.GetLocalizedString();
                Dropdown.options.Add(new TMP_Dropdown.OptionData(localizedText, localizedSprite));
            }

            Dropdown.value = selectedOptionIndex;
            Dropdown.RefreshShownValue();
        }

    }
}