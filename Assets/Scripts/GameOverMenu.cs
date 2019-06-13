using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    void Start()
    {
        scoreText.text =
            "Distance: " + PlayerPrefs.GetInt("metersTraveled", 0) + "m\n"
            + "Record: " + PlayerPrefs.GetInt("metersRecord", 0) + "m\n\n"
            + "Stars collected: " + PlayerPrefs.GetInt("starsCollected", 0) + "\n"
            + "Total stars: " + PlayerPrefs.GetInt("starsTotal", 0) + "";
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("GameScene");
        PlayerPrefs.SetInt("restart", 0);
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
        PlayerPrefs.SetInt("restart", 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
