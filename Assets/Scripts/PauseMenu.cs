using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Slider volumeSlider;
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
        volumeSlider.value = PlayerPrefs.GetFloat("volume",100);
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat("volume", volumeSlider.value);

    }


}
