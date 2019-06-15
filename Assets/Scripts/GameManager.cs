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


    public Canvas gameOverCanvas;
    public Canvas gameCanvas;
    public Canvas menuCanvas;
    public MeshRenderer skyBox;
    public Color colorNight;
    public Color colorMorning;
    public Color colorMidDay;
    public Color colorEvening;

    public float currentGameTime = 0;

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
    public AudioSource backgroundNoise;

    public int target = 60;
    public int starsAmount = 0;
    public int starsOldAmount = 0;

    public float fadeUISensitvity = 0.05f;

    public float timeScale;
    private float timeSinceStarPickup;

    bool hasStarted = false;

    void Awake()
    {
        sharedInstance = this;
        ApplyGraphicsSettings();
    }
    // Start is called before the first frame update
    void Start()
    {

        if (PlayerPrefs.GetInt("restart", 0) != 0)
        {
            menuCanvas.gameObject.SetActive(false);
            gameCanvas.gameObject.SetActive(true);
            StartGame();
        }
        else
        {
            SetGameStateNoTimeScale(false);
            currentGameState = GameState.paused;
        }
        
    }

    void ApplyGraphicsSettings()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target;
        Time.timeScale = timeScale;
    }

    public void InitMusic()
    {
        switch (PlayerPrefs.GetString("currentSong","song_0_0"))
        {
            case "song_0_0":
                backgroundMusic.time = 0;
                break;
            case "song_0_1":
                backgroundMusic.time = 64;
                break;
            case "song_1_0":
                backgroundMusic.time = 373;
                break;
            case "song_1_1":
                backgroundMusic.time = 548;
                break;
        }

    }

    private void SetSound(bool newState)
    {
        if (newState)
        {
            var audiosources = Resources.FindObjectsOfTypeAll<AudioSource>();
            for (int i = 0; i < audiosources.Length; i++)
            {
                if(audiosources[i] != backgroundNoise)
                    audiosources[i].UnPause();
            }
            
        }
        else
        {
            var audiosources = Resources.FindObjectsOfTypeAll<AudioSource>();
            for (int i = 0; i < audiosources.Length; i++)
            {
                if (audiosources[i] != backgroundNoise)
                    audiosources[i].Pause();
            }
        }
        
    }
    public void PauseGame()
    {
        FreezeGame();
        //SceneManager.LoadScene("PauseScene", LoadSceneMode.Additive);
        SetGamePaused();
    }
    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.inGame)
        {
            UpdateUI();
            currentGameTime += Time.deltaTime;
        }
    }

    private void UpdateUI()
    {
        ManageStarsCounter();
        textMetersTraveled.text = metersTraveled + " m";
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

    public void SetGameStateNoTimeScale(bool active)
    {
        foreach(Rigidbody2D rigidbody in Resources.FindObjectsOfTypeAll<Rigidbody2D>())
        {
            if (!active)
                PlayerController.sharedInstance.rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            else
                PlayerController.sharedInstance.rigidbody.constraints = RigidbodyConstraints2D.None;
        }
    }

    public void UnfreezeGame()
    {
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        SetGameState(GameState.inGame);
    }
    public void GameOver()
    {
        SetGameState(GameState.gameOver);
    }
    public void SetGamePaused()
    {
        SetGameState(GameState.paused);
    }
    public void SetGameState(GameState newGameState)
    {
        if(newGameState == GameState.paused)
        {
            if (backgroundNoise.isPlaying)
                backgroundNoise.Pause();

            FreezeGame();
            SetSound(false);
        }
        if (newGameState == GameState.inGame)
        {
            if (!hasStarted)
            {
                if (backgroundNoise.isPlaying)
                    backgroundNoise.Pause();

                InitMusic();
                
                PlayerController.sharedInstance.rigidbody.velocity = new Vector2(200, 0);
                PlayerController.sharedInstance.FollowPlayer();

                hasStarted = true;
                SetGameStateNoTimeScale(true);
            }

            if(!backgroundMusic.isPlaying)
                backgroundMusic.Play();
            
            UnfreezeGame();
            SetSound(true);

        }
        if (newGameState == GameState.gameOver)
        {
            SetSound(false);
            gameOverEffect.Play();
            Invoke("EndGame", gameOverEffect.clip.length);
        }
        currentGameState = newGameState;
    }
    private void EndGame()
    {
        SaveStats();
        if (!backgroundNoise.isPlaying)
            backgroundNoise.Play();
        gameCanvas.gameObject.SetActive(false);
        gameOverCanvas.gameObject.SetActive(true);
    }

    private void SaveStats()
    {
        PlayerPrefs.SetInt("starsCollected", starsAmount);
        PlayerPrefs.SetInt("starsTotal", PlayerPrefs.GetInt("starsTotal", 0) + starsAmount);
        PlayerPrefs.SetInt("metersTraveled", metersTraveled);
        if(PlayerPrefs.GetInt("metersRecord",0) < metersTraveled)
            PlayerPrefs.SetInt("metersRecord", metersTraveled);
    }
}
