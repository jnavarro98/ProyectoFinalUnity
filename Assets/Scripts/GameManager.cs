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

    public const float TRANSITION_TIME = 30;

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
    public int metersTraveled;
    float volumeMultiplicator;
    const string settingsDir = @".\preferences";
    const string settings = "preferences.txt";
    public AudioSource gameOverEffect;
    public AudioSource backgroundMusic;
    public float timeSinceLastPowerUP;
    public float transitionTimeElapsed = TRANSITION_TIME;

    public Color[] backgroundColors;
    public int colorIndex;
    public int lastColorIndex;

    public int target = 60;

    public float timeScale;

    void Awake()
    {
        
        timeSinceLastPowerUP = 0;
        sharedInstance = this;
        QualitySettings.vSyncCount = 0;
        PrepareBackgroundColors();
        InitMusic();
    }
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = target;
        Time.timeScale = timeScale;
        StartGame();
    }

    public void InitMusic()
    {

        switch (lastColorIndex)
        {
            case 1: //Morning
                backgroundMusic.time = 15;
                break;
            case 2://MidDay
                backgroundMusic.time = 31;
                break;
            case 3://Afternoon
                backgroundMusic.time = 64;
                break;
        }
        //backgroundMusic.Play();

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
            UpdateBackground();
        }
    }

    public void FreezeGame()
    {
        Time.timeScale = 0f;
    }

    public void PrepareBackgroundColors()
    {

        backgroundColors = new Color[4];

        backgroundColors[0] = colorNight;
        backgroundColors[1] = colorMorning;
        backgroundColors[2] = colorMidDay;
        backgroundColors[3] = colorEvening;
        colorIndex = Random.Range(0, 3);

        if (colorIndex == 0)
            lastColorIndex = 3;
        else
            lastColorIndex = colorIndex - 1;

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
            UnfreezePlayer();
            //LoadPreferences();
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

    void InitColorPalette()
    {



    }

    private void UpdateBackground()
    {

        if (transitionTimeElapsed <= Time.deltaTime)
        {
            // start a new transition
            transitionTimeElapsed = TRANSITION_TIME;
            lastColorIndex = colorIndex;
            colorIndex++;
            
            if (colorIndex > backgroundColors.Length - 1)
            {
                colorIndex = 0;
            }

        }
        else
        {

            // transition in progress
            // calculate interpolated color
            RenderSettings.skybox.SetColor("_SkyTint",
                Color.Lerp(backgroundColors[colorIndex], backgroundColors[lastColorIndex],
                transitionTimeElapsed / TRANSITION_TIME));

            // update the timer
            transitionTimeElapsed -= Time.deltaTime;
        }

    }
}
