using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum GameState
{
    paused,
    inGame,
    gameOver
}
public class GameManager : MonoBehaviour
{


    public MeshRenderer skyBox;
    public Color colorNight;
    public Color colorMorning;
    public Color colorMidDay;
    public Color colorEvening;

    public GameState currentGameState = GameState.paused;
    //Singleton
    public AudioSource playerSoundGrass;
    public static GameManager sharedInstance;
    public Text textMetersTraveled;
    public Text starsCollected;
    public Image starIcon;
    public int metersTraveled;
    float volumeMultiplicator;
    const string settingsDir = @".\preferences";
    const string settings = "preferences.txt";
    public AudioSource gameOverEffect;
    public AudioSource backgroundMusic;
    public float timeSinceLastPowerUP = 0;

    public int target = 60;
    public int starsAmount = 0;
    public int starsOldAmount = 0;

    public float fadeUISensitvity = 0.05f;

    public float timeScale;
    private float timeSinceStarPickup;

    void Awake()
    {
        sharedInstance = this;
        
        
    }
    // Start is called before the first frame update
    void Start()
    {
        ApplyGraphicsSettings();
        InitMusic();
        StartGame();
    }

    void ApplyGraphicsSettings()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target;
        Time.timeScale = timeScale;
    }

    public void InitMusic()
    {

        switch (BackgroundManagement.sharedInstance.currentBackgroundPhase)
        {
            case BackgroundManagement.BackgroundPhase.MidDay:
                backgroundMusic.time = 15;
                break;
            case BackgroundManagement.BackgroundPhase.Evening:
                backgroundMusic.time = 31;
                break;
            case BackgroundManagement.BackgroundPhase.Night:
                backgroundMusic.time = 64;
                break;
        }

    }

    private void LoadPreferences()
    {
        volumeMultiplicator = PlayerPrefs.GetFloat("volume", 100);
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
            ManageStarsCounter();
            textMetersTraveled.text = metersTraveled / 2 + " m";
            timeSinceLastPowerUP += Time.deltaTime;
        }
    }

    void ManageStarsCounter()
    {
        if(starsAmount > starsOldAmount)
        {
            EnableStarsCounter();
            starsCollected.text = starsAmount.ToString();
            timeSinceStarPickup = 0;
            starsOldAmount = starsAmount;
        }

        if (timeSinceStarPickup >= 3)
        {
            DisableStarsCounter();
        }

        if(timeSinceStarPickup <= 3)
            timeSinceStarPickup += Time.deltaTime;

        Debug.Log(timeSinceStarPickup);
    }

    void DisableStarsCounter()
    {
        starsCollected.color = new Color(starsCollected.color.r, starsCollected.color.g,
                starsCollected.color.b, 0);
        starIcon.color = new Color(starIcon.color.r, starIcon.color.g,
            starIcon.color.b, 0);
    }

    void EnableStarsCounter()
    {
        starsCollected.color = new Color(starsCollected.color.r, starsCollected.color.g,
                starsCollected.color.b, 1);
        starIcon.color = new Color(starIcon.color.r, starIcon.color.g,
            starIcon.color.b, 1);
    }

    public void FreezeGame()
    {
        Time.timeScale = 0f;
    }

    public void UnfreezeGame()
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
            UnfreezeGame();
            LoadPreferences();
            backgroundMusic.Play();
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
