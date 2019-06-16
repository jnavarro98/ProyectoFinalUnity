using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Toggle switchedControlsToggle;
    public Toggle slowDownMusicToggle;
    void Start()
    {
        
        LoadSettingsFirstTime();
    }

    void LoadSettingsFirstTime()
    {
        PlayerPrefs.SetInt("switchedControls", PlayerPrefs.GetInt("switchedControls", 0));
        PlayerPrefs.SetInt("slowDownMusic", PlayerPrefs.GetInt("slowDownMusic", 1));
    }
    // Start is called before the first frame update
    public void LoadSettings()
    {
        switchedControlsToggle.isOn = PlayerPrefs.GetInt("switchedControls", 0) != 0;
        slowDownMusicToggle.isOn = PlayerPrefs.GetInt("slowDownMusic", 1) != 0;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("switchedControls", switchedControlsToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("slowDownMusic", slowDownMusicToggle.isOn ? 1 : 0);
    }
}
