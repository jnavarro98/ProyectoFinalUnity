using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController sharedInstance;

    [Header("Physics")]
    [Space(10)]

    const int RGBMAX = 255;

    public float bounceForce;
    public float maxChargeForce = 2;
    public float minChargeForce = 0.5f;
    public Rigidbody2D rigidbody;
    public float groundThreshold = 0.5f;
    public float forceMultiplier = 1f;
    public float speedBonusMultiplier = 3;
    public float maxVelocity;
    public float maxXAceleration = 40f;
    public float maxYAceleration = 10f;
    public float xAcceleration = 4.6f;
    public float yAcceleration = 1f;
    public float descendingForce = 1f;
    public float baseForceJump = 10;
    public float jumpForce = 100f;
    public float minVelocity = 5;
    public float secondsToCharge = 4;

    [Header("Logic")]
    [Space(10)]

    public float minDeltaTime = 0.8f;
    public int buffDuration;
    public LayerMask whatIsGround;

    [Header("Camera")]
    [Space(10)]
    public CinemachineVirtualCamera gameVCam;
    public CinemachineVirtualCamera staticVCam;
    public float minFOV = 10f;
    public float maxFOV = 40f;
    public float maxYOffset = 40f;
    public float minYOffset = 40f;
    public float camSensivity = 0.01f;
    public float offsetYSensivity = 0.01f;
    public float cameraDelay;
    public float descensionThreshold;

    [Header("Graphics")]
    [Space(10)]
    public Color fireSpriteColor;
    Color originalTrailColor;
    public Color chargeFireColor;
    TrailRenderer trailRenderer;
    Sprite originalSprite;
    SpriteRenderer spriteRenderer;
    ParticleSystem explosion;
    public ParticleSystem bounceEffect;
    List<TrailRenderer> trails;

    [Header("Audio")]
    [Space(10)]
    public AudioSource rockEffect;
    public AudioSource bounceSoundEffect;
    public AudioSource hitGroundSound;

    float timeSinceLastJump = 0;
    float trailCap;
    float lastYPosition;
    float yMovement;
    float radius;
    float timeSinceLastFirePowerUp;

    float screenWidth;
    float initialXPosition;
    bool onGround;
    bool ascending;
    bool jump = false;
    bool descend = false;
    bool isDead = false;
    bool switchedControls;
    public bool isPlaying = false;
    
    float chargeForce = 0f;

    bool TrailState {
        get
        {
            return trails[0].enabled;
        }
        set
        {
            for(int i = 0; i < trails.Count; i++)
            {
                trails[i].enabled = value;
            }
        }
    }

    float TrailTime
    {
        get
        {
            return trails[0].time;
        }
        set
        {
            for (int i = 0; i < trails.Count; i++)
            {
                trails[i].time = value;
            }
        }
    }

    Color TrailColor
    {
        get
        {
            return trails[0].endColor;
        }
        set
        {
            for (int i = 0; i < trails.Count; i++)
            {
                trails[i].endColor = value;
            }
        }
    }

    



    // Start is called before the first frame update
    void Start()
    {
        InitValues();
        InitTrails();
        GetComponents();
    }

    public void FollowPlayer()
    {
        Invoke("GetFollowedByCamera", 2f);
        switchedControls = PlayerPrefs.GetInt("switchedControls", 0) != 0;
    }

    public void CheckOptions()
    {
        switchedControls = PlayerPrefs.GetInt("switchedControls", 0) != 0;
    }

    public void GetFollowedByCamera()
    {
        initialXPosition = transform.position.x;
        isPlaying = true;
        gameVCam.gameObject.SetActive(true);
    }

    void GetComponents()
    {
        explosion = GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = explosion.main;
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSprite = GetComponent<SpriteRenderer>().sprite;
        originalTrailColor = TrailColor;
        TrailColor = originalTrailColor;

    }

    void InitValues()
    {
        timeSinceLastFirePowerUp = buffDuration;
        screenWidth = Screen.width;
        radius = GetComponent<CircleCollider2D>().radius + groundThreshold;
    }

    void InitTrails()
    {
        trails = new List<TrailRenderer>();
        trails.Add(GetComponent<TrailRenderer>());
        foreach (TrailRenderer t in GetComponentsInChildren<TrailRenderer>())
        {
            trails.Add(t);
        }
        trailCap = TrailTime;
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if(GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("FireEnemy"))
            {

                if (collider.gameObject.CompareTag("FireballBlue") || collider.gameObject.CompareTag("FireballRed"))
                {
                    Bounce(Vector2.up);
                }
                else
                {
                    Bounce(ClosestPointToCollider(collider) - new Vector2(transform.position.x, transform.position.y));
                }

            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.gameObject.layer == LayerMask.NameToLayer("PowerUPs"))
        {
            switch (collider.gameObject.tag)
            {
                case "FirePowerUp":
                    ActivateFirePowerUp();
                    break;
                case "SpeedPowerUp":
                    rigidbody.AddForce(new Vector2(rigidbody.velocity.normalized.x * speedBonusMultiplier,
                        rigidbody.velocity.normalized.y * speedBonusMultiplier), ForceMode2D.Impulse);
                    break;
            }
            
        }

        if (collider.gameObject.layer == LayerMask.NameToLayer("OutOfBounds"))
        {
            GameOverOut();
        }

        if (collider.gameObject.layer == LayerMask.NameToLayer("FireEnemy"))
        {
            if (!TrailState)
            {
                Color deathColor = Color.white;

                if (collider.gameObject.tag.Contains("Red"))
                {
                    deathColor = Color.red;
                }

                if (collider.gameObject.tag.Contains("Blue"))
                {
                    deathColor = new Color(0.2235294f, 0.9333334f, 1f);
                }

                GameOverFire(deathColor);
            }
            else
            {
                if (collider.gameObject.CompareTag("FireballBlue") || collider.gameObject.CompareTag("FireballRed"))
                {
                    Bounce(Vector2.up);
                } else
                {
                    Bounce(ClosestPointToCollider(collider) - new Vector2(transform.position.x, transform.position.y));
                }
            }

        }
        if (collider.gameObject.layer == LayerMask.NameToLayer("Spikes"))
        {
            GameOverSpikes();
        }
    }

    Vector2 ClosestPointToCollider(Collider2D col)
    {
        GameObject go = new GameObject("tempCollider");
        go.transform.position = transform.position;
        CircleCollider2D c = go.AddComponent<CircleCollider2D>();
        c.radius = 0.1f;
        ColliderDistance2D dist = col.Distance(c);
        Destroy(go);
        return dist.pointA;
    }

    void Awake()
    {
        sharedInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
            StartCoroutine(GroundCheck());
            UpdateAscension();
            CheckFirePowerUp();

            InputManagement();
            AcceleratePlayer();
            Jump();
            if (GameManager.sharedInstance.currentGameTime > 3)
                Descend();

            ManageSound();
            UpdateMeters();
            ManageCounters();
        }
        if (GameManager.sharedInstance.currentGameState == GameState.paused)
        {
            if(!rockEffect.mute)
                rockEffect.mute = true;
        }
        }

    void ManageCounters()
    {
        timeSinceLastFirePowerUp += Time.deltaTime;
    }

    private void UpdateMeters()
    {
        if (isPlaying)
        {
            GameManager.sharedInstance.metersTraveled =
            Math.Abs((int)((0.3) * (transform.position.x - initialXPosition))) / 3;
        }
        
    }
    private void ManageSound()
    {
        rockEffect.mute = !onGround;
    }

    private void InputManagement()
    {
        
        int i = 0;
        while(i < Input.touchCount)
        {
            if (switchedControls)
            {
                if (Input.GetTouch(i).position.x > screenWidth / 2)
                {
                    descend = true;
                }
                if (Input.GetTouch(i).position.x < screenWidth / 2 &&
                Input.GetTouch(i).phase == TouchPhase.Began && onGround)
                {

                    jump = true;
                }
                if (Input.GetTouch(i).position.x < screenWidth / 2 &&
                Input.GetTouch(i).phase == TouchPhase.Stationary)
                {
                    ChargeBounce();
                }

            } else
            {
                if (Input.GetTouch(i).position.x > screenWidth / 2 &&
                Input.GetTouch(i).phase == TouchPhase.Began && onGround)
                {
                    jump = true;
                }
                if (Input.GetTouch(i).position.x < screenWidth / 2)
                {
                    descend = true;
                }
                if (Input.GetTouch(i).position.x > screenWidth / 2 &&
                Input.GetTouch(i).phase == TouchPhase.Stationary)
                {
                    ChargeBounce();
                }
            }
            
            i++;
        }
        if (Input.GetButtonDown("Jump") && onGround)
        {
            jump = true;
        }
        if (Input.GetKey(KeyCode.Z))
        {
            descend = true;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            ChargeBounce();
        }
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene("GameScene");
        }


    }

    private void CheckFirePowerUp()
    {
        if (timeSinceLastFirePowerUp > buffDuration &&
            TrailState)
        {
                DeactivateFirePowerUp();
        }
        if (timeSinceLastFirePowerUp < buffDuration)
        {
            TrailTime = Mathf.Lerp(trailCap, 0, timeSinceLastFirePowerUp / buffDuration);
        }

    }
    void ActivateFirePowerUp()
    {
        chargeForce = 0;
        timeSinceLastFirePowerUp = 0;
        UpdateChargeState();
        TrailState = true;
    }
    private void DeactivateFirePowerUp()
    {
        TrailState = false;
        chargeForce = 0;
        UpdateChargeState();
        spriteRenderer.color = Color.white;
    }

    void FixedUpdate()
    {
        if (GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
            if(GameManager.sharedInstance.currentGameTime > 3)
                DirectCamera();
        }
    }
    void UpdateAscension()
    {
        //Updating ascension
        yMovement = transform.position.y - lastYPosition;
        lastYPosition = transform.position.y;
        ascending = yMovement > 0;
    }
    IEnumerator GroundCheck()
    {
        //Checking wether the player is on ground or not
        bool wasGrounded = onGround;
        onGround = false;
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                onGround = true;
                if (!wasGrounded)
                {
                    
                    hitGroundSound.Play();
                    if(Time.timeScale != 1)
                    {
                        chargeForce = 0;
                        UpdateChargeState();
                        if(GameManager.sharedInstance.slowDownMusic)
                            GameManager.sharedInstance.backgroundMusic.pitch = 1;
                    }
                }
            }
        }
        yield return null;
    }

    private void GameOverFire(Color fireColor)
    {
        if (!isDead)
        {
            gameVCam.Follow = null;
            rigidbody.drag = 3;
            ParticleSystem.MainModule main = explosion.main;
            main.startColor = fireColor;
            explosion.Play();
            spriteRenderer.enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            TrailState = false;
            GameManager.sharedInstance.SetGameState(GameState.gameOver);
            isDead = true;
        }
    }

    private void GameOverOut()
    {
        if (!isDead)
        {
            gameVCam.Follow = null;
            GameManager.sharedInstance.SetGameState(GameState.gameOver);
            isDead = true;
        }
    }

    private void GameOverSpikes()
    {
        if (!isDead)
        {
            gameVCam.Follow = null;
            rigidbody.drag = 3;
            ParticleSystem.MainModule main = GetComponent<ParticleSystem>().main;
            main.startColor = Color.white;
            GetComponent<ParticleSystem>().Play();
            spriteRenderer.enabled = false;
            rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            TrailState = false;
            GameManager.sharedInstance.SetGameState(GameState.gameOver);
            isDead = true;
        }
        
    }
    void Jump()
    {

        if (jump && onGround && rigidbody.velocity.x > 0 && timeSinceLastJump > 1)
        {
            
            rigidbody.AddForce(new Vector2(0, (jumpForce * forceMultiplier * rigidbody.velocity.x) + baseForceJump), ForceMode2D.Impulse);
            jump = false;
            timeSinceLastJump = 0;
            if(TrailState)
                UpdateChargeState();
        }
        
        timeSinceLastJump += Time.deltaTime;

    }


    void ChargeBounce()
    {

        if (timeSinceLastFirePowerUp < buffDuration && !onGround && chargeForce < maxChargeForce && timeSinceLastJump > 0.2f)
        {

            UpdateChargeState();
            chargeForce += Time.deltaTime / secondsToCharge;
        }
        

    }

    public void UpdateChargeState()
    {

        spriteRenderer.color = Color.Lerp(fireSpriteColor, chargeFireColor, chargeForce / maxChargeForce);
        TrailColor = Color.Lerp(originalTrailColor, chargeFireColor, chargeForce / maxChargeForce);

        if (Time.timeScale > minDeltaTime)
        {
            Time.timeScale = Mathf.Lerp(1, minDeltaTime, chargeForce / maxChargeForce);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            if(GameManager.sharedInstance.slowDownMusic)
                GameManager.sharedInstance.backgroundMusic.pitch = Time.timeScale;
        }
    }

    void Bounce(Vector2 direction)
    {
        if (chargeForce > minChargeForce)
        {
            Vector2 newVelocity = Vector2.Reflect(rigidbody.velocity, direction.normalized);
            rigidbody.velocity = new Vector2(baseForceJump * newVelocity.normalized.x + newVelocity.x * chargeForce * bounceForce,
                baseForceJump * Math.Abs(newVelocity.normalized.y) + Math.Abs(newVelocity.y) * chargeForce * bounceForce);

            timeSinceLastJump = 0;
        }
        else
        {
            Vector2 newVelocity = Vector2.Reflect(rigidbody.velocity, direction.normalized);
            rigidbody.velocity = new Vector2(baseForceJump * newVelocity.normalized.x + newVelocity.x * minChargeForce * bounceForce,
               baseForceJump * Math.Abs(newVelocity.normalized.y) + Math.Abs(newVelocity.y) * minChargeForce * bounceForce);

            timeSinceLastJump = 0;
        }

        AnimateBounce();
        chargeForce = 0;
        UpdateChargeState();

    }
    void AnimateBounce()
    {
        ParticleSystem currentEffect = Instantiate(bounceEffect);
        currentEffect.transform.position = new Vector3(transform.position.x, transform.position.y - radius, transform.position.z);
        currentEffect.transform.rotation = new Quaternion(0, 0, 0, Vector2.Angle(Vector2.zero, rigidbody.velocity));

        ParticleSystem.MainModule main = currentEffect.main;
        main.startColor = spriteRenderer.color;
        currentEffect.Play();
        bounceSoundEffect.Play();
    }

    void AcceleratePlayer()
    {
        //Land accelleration (has my custom formula to fit my level design)
        if (onGround && rigidbody.velocity.x < maxXAceleration)
        {
            rigidbody.AddForce(new Vector2(xAcceleration * forceMultiplier * Math.Abs(rigidbody.velocity.normalized.x),
                0), ForceMode2D.Impulse);
        }
        if (onGround && rigidbody.velocity.y < maxYAceleration && rigidbody.velocity.x > 0.4f)
        {
            rigidbody.AddForce(new Vector2(0,
                yAcceleration * forceMultiplier * rigidbody.velocity.normalized.y)
                , ForceMode2D.Impulse);
        }
        if (onGround && rigidbody.velocity.magnitude < minVelocity)
        {
            rigidbody.AddForce(new Vector2(xAcceleration * forceMultiplier * Math.Abs(rigidbody.velocity.normalized.x),
                yAcceleration * forceMultiplier * Math.Abs(rigidbody.velocity.normalized.y))
                , ForceMode2D.Impulse);
        }
        rigidbody.velocity = Vector2.ClampMagnitude(rigidbody.velocity, maxVelocity);
    }

    
    void Descend()
    {
        
        if(descend)
        {
            rigidbody.AddForce(new Vector2(0, -descendingForce), ForceMode2D.Impulse);
        }
        descend = false;
    }
    void DirectCamera()
    {
        //Camera Direction
        if (!onGround && timeSinceLastJump > cameraDelay)
        {

            if (gameVCam.m_Lens.OrthographicSize < maxFOV)
                gameVCam.m_Lens.OrthographicSize += camSensivity;

            if (rigidbody.velocity.y < - descensionThreshold)
            {
                if (gameVCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y >= maxYOffset)
                    gameVCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y -= offsetYSensivity;
            }
            else
            {
                if (gameVCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y <= minYOffset)
                    gameVCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y += offsetYSensivity;
            }
            
        }
        else
        {
            if (gameVCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y <= minYOffset)
                                                                                                         // se multiplica por 2.5 para que se recupere antes hacia arriba que hacia abajo
                gameVCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y += offsetYSensivity * 2.5f;
            if (gameVCam.m_Lens.OrthographicSize > minFOV)
                gameVCam.m_Lens.OrthographicSize -= camSensivity;
        }

        if(rigidbody.velocity.y < -descensionThreshold && timeSinceLastJump > cameraDelay)
        {
            if (gameVCam.GetCinemachineComponent<
                CinemachineTransposer>().m_FollowOffset.y >= maxYOffset)
                gameVCam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y -= offsetYSensivity;
            if (gameVCam.m_Lens.OrthographicSize < maxFOV)
                gameVCam.m_Lens.OrthographicSize += camSensivity;
        }
    }

    Color InverseColor(Color ColourToInvert)
    {
        return new Color(RGBMAX - ColourToInvert.r,
          RGBMAX - ColourToInvert.g, RGBMAX - ColourToInvert.b);
    }


}
