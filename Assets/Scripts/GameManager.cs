using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    paused,
    inGame,
    gameOver
}
public class GameManager : MonoBehaviour
{
    public GameState currentGameState = GameState.paused;
    //Singleton
    public AudioSource playerSoundGrass;
    public static GameManager sharedInstance;
    public Text textMetersTraveled;
    public int metersTraveled;
    float volumeMultiplicator;
    const string settingsDir = @".\preferences";
    const string settings = "preferences.txt";
    public AudioSource gameOverEffect;
    public AudioSource backgroundMusic;
    public float timeSinceLastPowerUP;

    public int target = 60;

    void Awake()
    {
        timeSinceLastPowerUP = 0;
        sharedInstance = this;
        QualitySettings.vSyncCount = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = target;
        StartGame();
    }

    private void LoadPreferences()
    {
        if (Directory.Exists(settingsDir))
        {
            string preferences = File.ReadAllText(Path.Combine(settingsDir, settings));
            volumeMultiplicator = float.Parse(preferences.TrimEnd('\n').Split('\n')[0].
                Split('=')[1], CultureInfo.InvariantCulture.NumberFormat);
        }
        else
        {
            volumeMultiplicator = 100;
        }
        ApplySettings();
    }
    public void PauseGame()
    {
        FreezeGame();
        SceneManager.LoadScene("PauseScene", LoadSceneMode.Additive);
        BackToMenu();
    }
    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.inGame)
        {
            textMetersTraveled.text = metersTraveled + " m";
            timeSinceLastPowerUP += Time.deltaTime;
        }
    }

    public void FreezeGame()
    {
        Time.timeScale = 0f;
    }

    public void UnfreezePlayer()
    {
        Time.timeScale = 1f;
    }

    private void ApplySettings()
    {
        foreach(AudioSource a in Resources.FindObjectsOfTypeAll(typeof(AudioSource)))
        {
            a.volume = volumeMultiplicator;
        }
    }

    public void StartGame()
    {
        SetGameState(GameState.inGame);
    }
    public void GameOver()
    {
        SetGameState(GameState.gameOver);
    }
    public void BackToMenu()
    {
        SetGameState(GameState.paused);
    }
    public void SetGameState(GameState newGameState)
    {
        if(newGameState == GameState.paused)
        {
            foreach (AudioSource a in Resources.FindObjectsOfTypeAll(typeof(AudioSource)))
            {
                a.Pause();
            }
        }
        if (newGameState == GameState.inGame)
        {
            LoadPreferences();
            UnfreezePlayer();
            LoadPreferences();
            GetComponent<AudioSource>().Play();
        }
        if (newGameState == GameState.gameOver)
        {
            foreach (AudioSource a in Resources.FindObjectsOfTypeAll(typeof(AudioSource)))
            {
                a.Stop();
            }
            gameOverEffect.Play();
            Invoke("EndGame", gameOverEffect.clip.length);
        }
        currentGameState = newGameState;
    }
    private void EndGame()
    {
        SceneManager.LoadScene("GameOverScene");
    }
}
