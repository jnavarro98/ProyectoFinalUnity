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

    void Awake()
    {
        LoadPreferences();
    }
    private void LoadPreferences()
    {
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

    }
}
