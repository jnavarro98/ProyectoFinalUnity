using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    void Start()
    {

    }

    // Start is called before the first frame update
    public void LoadSettings()
    {
        GetComponentInChildren<Toggle>().isOn = PlayerPrefs.GetInt("switchedControls", 0) != 0;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("switchedControls", GetComponentInChildren<Toggle>().isOn ? 1 : 0);
    }
}
