using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : Singleton<GameManager>
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    // Start is called before the first frame update
    void Start()
    {
        bgmSource.volume = PlayerPrefs.GetFloat("BGMVolume", 1);
        sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1);
    }

    public void ChangeMusicVolume(float value)
    {
        bgmSource.volume = value;
        PlayerPrefs.SetFloat("BGMVolume", value);
    }

    public void ChangeSFXVolume(float value)
    {
        sfxSource.volume = value;
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}
