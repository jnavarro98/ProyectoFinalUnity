using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Slider volumeSlider;
    float volumeLevel;

    void Awake()
    {
        LoadPreferences();
    }
    private void LoadPreferences()
    {
        volumeLevel = PlayerPrefs.GetFloat("volume", 100);
    }
    public void PlayGame ()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);
    }
}
