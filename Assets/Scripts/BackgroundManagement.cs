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
    // Start is called before the first frame update
    void Awake()
    {
        PrepareBackgroundColors();
        PrepareBackgroundLayers();
        sharedInstance = this;
    }
    void Start()
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


        }
        else
        {


            // transition in progress
            // calculate interpolated color
            RenderSettings.skybox.SetColor("_Tint",
                Color.Lerp(backgroundColors[(int)currentBackgroundPhase], 
                backgroundColors[(int)lastBackgroundPhase], transitionTimeElapsed / TRANSITION_TIME));

            if(currentBackgroundPhase == BackgroundPhase.Night)
            {
                foreach(SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
                {
                    if (sprite.name.Contains("Cloud"))
                    {
                        sprite.color = new Color(opaqueCloud.r, opaqueCloud.g, opaqueCloud.b, transitionTimeElapsed / TRANSITION_TIME);
                    }
                    if (sprite.name.Contains("Starfield"))
                    {
                        sprite.color = new Color(opaqueStars.r, opaqueStars.g, opaqueStars.b, Math.Abs(1 - (transitionTimeElapsed / TRANSITION_TIME)));
                    }
                }
                RenderSettings.skybox.SetFloat("_Rotation", Mathf.Lerp(300, 180, Math.Abs(1 - (transitionTimeElapsed / TRANSITION_TIME))));
            }

            if (currentBackgroundPhase == BackgroundPhase.Morning)
            {
                foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
                {
                    if (sprite.name.Contains("Cloud"))
                    {
                        sprite.color = new Color(opaqueCloud.r, opaqueCloud.g, opaqueCloud.b, Math.Abs(1 - (transitionTimeElapsed / TRANSITION_TIME)));
                    }
                    if (sprite.name.Contains("Starfield"))
                    {
                        sprite.color = new Color(opaqueStars.r, opaqueStars.g, opaqueStars.b, transitionTimeElapsed / TRANSITION_TIME);
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

            // update the timer
            transitionTimeElapsed -= Time.deltaTime;
        }

    }
}
