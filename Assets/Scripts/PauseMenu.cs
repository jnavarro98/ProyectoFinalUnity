﻿using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Slider volumeSlider;
    const string settingsDir = @".\preferences";
    const string settings = "preferences.txt";
    float volumeLevel;

    void Awake()
    {
        LoadPreferences();
    }

    // Start is called before the first frame update
    public void PlayGame()
    {
        GameManager.sharedInstance.StartGame();
        SceneManager.UnloadSceneAsync("PauseScene");
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    private void LoadPreferences()
    {
        if (Directory.Exists(settingsDir))
        {
            string preferences = File.ReadAllText(Path.Combine(settingsDir, settings));
            Debug.Log(preferences.TrimEnd('\n').Split('\n')[0].Split('=')[1]);
            volumeLevel = float.Parse(preferences.TrimEnd('\n').Split('\n')[0].
                Split('=')[1], CultureInfo.InvariantCulture.NumberFormat);
            volumeSlider.value = volumeLevel;
        }
    }

    public void SaveSettings()
    {
        if (!Directory.Exists(settingsDir))  // if it doesn't exist, create
            Directory.CreateDirectory(settingsDir);

        // use Path.Combine to combine 2 strings to a path
        File.WriteAllText(Path.Combine(settingsDir, settings), "volume=" + volumeSlider.value + "\n");
    }

}