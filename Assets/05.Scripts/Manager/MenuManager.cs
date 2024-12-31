using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown hitSoundDropdown;
    [SerializeField] private TextMeshProUGUI debugText;

    void OnEnable()
    {
        bgmSlider.value = PlayerPrefs.GetFloat("BGMVolume", 1);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1);
        // languageDropdown.value = PlayerPrefs.GetInt("Language", 0);
        hitSoundDropdown.value = PlayerPrefs.GetInt("HitSound", 0);
    }

    // public void SetDebugMode(bool isDebug)
    // {
    //     debugText.gameObject.SetActive(isDebug);
    // }
}
