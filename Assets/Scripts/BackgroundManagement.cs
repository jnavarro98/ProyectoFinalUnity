using DigitalRuby.RainMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManagement : MonoBehaviour
{
    public enum BackgroundPhase
    {
        Night = 0, Morning = 1, MidDay = 2, Evening = 3
    }
    

    public BackgroundPhase currentBackgroundPhase;
    public BackgroundPhase lastBackgroundPhase;
    public Color colorNight;
    public Color colorMorning;
    public Color colorMidDay;
    public Color colorEvening;

    public int rainChance = 2;

    public SpriteRenderer cloudBackground;
    public SpriteRenderer starsBackground;
    Color opaqueCloud;
    Color opaqueStars;

    public static BackgroundManagement sharedInstance;

    float transitionTimeElapsed = TRANSITION_TIME;
    public const float TRANSITION_TIME = 33;
    public Color[] backgroundColors;
    public int color;
    public int nextColor;

    public RainScript2D rainmaker;
    float lastRainCap = 0;
    float rainCap = 0;
    // Start is called before the first frame update
    void Awake()
    {
        CheckRain();
        PrepareBackgroundColors();
        PrepareBackgroundLayers();
        PrepareComponents();
        sharedInstance = this;
    }
    void Start()
    {
        
    }
    void PrepareComponents()
    {

    }
    // Update is called once per frame
    void Update()
    {
        UpdateBackground();
    }

    void PrepareBackgroundLayers()
    {
        opaqueCloud = cloudBackground.color;
        opaqueStars = starsBackground.color;
        starsBackground.color = new Color(starsBackground.color.r, starsBackground.color.g, starsBackground.color.b, 0);
    }

    public void PrepareBackgroundColors()
    {

        backgroundColors = new Color[4];

        backgroundColors[(int)BackgroundPhase.Night] = colorNight;
        backgroundColors[(int)BackgroundPhase.Morning] = colorMorning;
        backgroundColors[(int)BackgroundPhase.MidDay] = colorMidDay;
        backgroundColors[(int)BackgroundPhase.Evening] = colorEvening;
        //Gets a random phase
        currentBackgroundPhase = (BackgroundPhase)UnityEngine.Random.Range((int)BackgroundPhase.Night, 
            System.Enum.GetNames(typeof(BackgroundPhase)).Length);

        if (currentBackgroundPhase == BackgroundPhase.Night)
            lastBackgroundPhase = BackgroundPhase.Evening;
        else
            lastBackgroundPhase = currentBackgroundPhase - 1;

    }

    private void UpdateBackground()
    {


        if (transitionTimeElapsed <= Time.deltaTime)
        {
            // start a new transition
            transitionTimeElapsed = TRANSITION_TIME;
            lastBackgroundPhase = currentBackgroundPhase;

            if (currentBackgroundPhase == BackgroundPhase.Evening)
                currentBackgroundPhase = BackgroundPhase.Night;
            else
                currentBackgroundPhase++;

            CheckRain();
            
            
        }
        else
        {


            // transition in progress
            // calculate interpolated color
            RenderSettings.skybox.SetColor("_Tint",
                Color.Lerp(backgroundColors[(int)currentBackgroundPhase], 
                backgroundColors[(int)lastBackgroundPhase], transitionTimeElapsed / TRANSITION_TIME));

            var sprites = GetComponentsInChildren<SpriteRenderer>();
            if (currentBackgroundPhase == BackgroundPhase.Night)
            {
                
                for (int i = 0; i < sprites.Length; i++)
                {
                    if (sprites[i].name.Contains("Cloud"))
                    {
                        sprites[i].color = new Color(opaqueCloud.r, opaqueCloud.g, opaqueCloud.b, transitionTimeElapsed / TRANSITION_TIME);
                    }
                    if (sprites[i].name.Contains("Starfield"))
                    {
                        sprites[i].color = new Color(opaqueStars.r, opaqueStars.g, opaqueStars.b, Math.Abs(1 - (transitionTimeElapsed / TRANSITION_TIME)));
                    }
                }
                RenderSettings.skybox.SetFloat("_Rotation", Mathf.Lerp(300, 180, Math.Abs(1 - (transitionTimeElapsed / TRANSITION_TIME))));
            }

            if (currentBackgroundPhase == BackgroundPhase.Morning)
            {
                for (int i = 0; i < sprites.Length; i++)
                {
                    if (sprites[i].name.Contains("Cloud"))
                    {
                        sprites[i].color = new Color(opaqueCloud.r, opaqueCloud.g, opaqueCloud.b, Math.Abs(1 - (transitionTimeElapsed / TRANSITION_TIME)));
                    }
                    if (sprites[i].name.Contains("Starfield"))
                    {
                        sprites[i].color = new Color(opaqueStars.r, opaqueStars.g, opaqueStars.b, transitionTimeElapsed / TRANSITION_TIME);
                    }
                }
                RenderSettings.skybox.SetFloat("_Rotation", Mathf.Lerp(180, 60, Math.Abs(1 - (transitionTimeElapsed / TRANSITION_TIME))));
            }

            if (currentBackgroundPhase == BackgroundPhase.MidDay)
            {
                RenderSettings.skybox.SetFloat("_Rotation", Mathf.Lerp(60, 0, Math.Abs(1 - (transitionTimeElapsed / TRANSITION_TIME))));
            }

            if (currentBackgroundPhase == BackgroundPhase.Evening)
            {
                RenderSettings.skybox.SetFloat("_Rotation", Mathf.Lerp(360, 300, Math.Abs(1 - (transitionTimeElapsed / TRANSITION_TIME))));
            }
            
            

            ManageRain();
            // update the timer
            transitionTimeElapsed -= Time.deltaTime;
        }

        

    }

    void CheckRain()
    {
        if (rainCap == 0)
        {
            rainmaker.RainIntensity = 0;
        }

        if (UnityEngine.Random.Range(0, rainChance) == 0)
        {
            if (rainmaker.RainIntensity > 0)
            {
                DeactivateRain();
            }
            else
            {
                ActivateRain();
            }
        }
        else
        {
            lastRainCap = rainCap;
        }
    }

    private void ManageRain()
    {
        if (lastRainCap != rainCap)
            rainmaker.RainIntensity = Mathf.Lerp(lastRainCap, rainCap,
                Math.Abs(1 - (transitionTimeElapsed / TRANSITION_TIME)));
    }

    void ActivateRain()
    {
        lastRainCap = rainCap;
        rainCap = UnityEngine.Random.Range(0.4f, 1f);
        Debug.Log(rainCap);
    }

    void DeactivateRain()
    {
        lastRainCap = rainCap;
        rainCap = 0;
        Debug.Log("RainStop");
    }
}
