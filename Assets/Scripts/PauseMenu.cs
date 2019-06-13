using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    void Awake()
    {

    }

    // Start is called before the first frame update
    public void PlayGame()
    {
        GameManager.sharedInstance.StartGame();
        //SceneManager.UnloadSceneAsync("PauseScene");
    }

    public void GoToMenu()
    {
        PlayerPrefs.SetInt("restart", 0);
        SceneManager.LoadScene("GameScene");
    }


}
