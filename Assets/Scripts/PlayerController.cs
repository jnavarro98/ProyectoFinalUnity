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

    public Rigidbody2D rigidbody;
    public float forceMultiplier = 1f;
    public float maxVelocity;
    public float maxXAceleration = 40f;
    public float maxYAceleration = 10f;
    public float xAcceleration = 4.6f;
    public float yAcceleration = 1f;
    public float descendingForce = 1f;
    public float jumpForce = 100f;
    public float minVelocity = 5;

    public float minFOV = 10f;
    public float maxFOV = 40f;
    public float maxYOffset = 40f;
    public float minYOffset = 40f;
    public float camSensivity = 0.01f;
    public float offsetYSensivity = 0.01f;
    float timeSinceLastJump;
    public float cameraDelay;

    float lastYPosition;
    float yMovement;
    float originalYForce;
    public float buffYForce = 1.5f;

    bool jump = false;
    bool descend = false;
    private float initialXPosition;

    public Sprite spritePowerUp;
    Sprite spriteCheese;
    private bool onGround;
    float radius;
    bool ascending;
    public int buffDuration;
    float ScreenWidth;

    public ParticleSystem deathParticle;
    public TrailRenderer redPowerUpTrail;

    public AudioSource grassEffect;
    public AudioSource landOnGrass;

    [SerializeField] private LayerMask whatIsOutOfBounds;
    [SerializeField] private LayerMask whatIsRedPowerUp;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsEnemies;

    public CinemachineVirtualCamera vcam;
    public float velocityBoost = 20;


    // Start is called before the first frame update
    void Start()
    {
        timeSinceLastJump = 0;
        ScreenWidth = Screen.width;
        radius = GetComponent<SpriteRenderer>().bounds.size.y;
        initialXPosition = transform.position.x;
        spriteCheese = (GetComponent<SpriteRenderer>()).sprite;
    }

    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
            InputManagement();
            Jump();
            Descend();
            AcceleratePlayer();
            UpdateAscension();
            GroundCheck();
            MouseHitCheck();
            BoundsCheck();
            CheckRedPowerUp();
            ManageSound();
            UpdateMeters();
        }
             

    }

    private void UpdateMeters()
    {
        GameManager.sharedInstance.metersTraveled =
            Math.Abs((int)((0.3)*(transform.position.x - initialXPosition)));
    }
    private void ManageSound()
    {
        grassEffect.mute = !onGround;
    }

    private void InputManagement()
    {
        
        int i = 0;
        while(i < Input.touchCount)
        {
            if(Input.GetTouch(i).position.x > ScreenWidth / 2 && 
                Input.GetTouch(i).phase == TouchPhase.Began)
            {
                jump = true;
            }
            if (Input.GetTouch(i).position.x < ScreenWidth / 2)
            {
                descend = true;
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
        if (Input.GetKey(KeyCode.D))
        {
            GameOver();
        }


    }

    private void CheckRedPowerUp()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsRedPowerUp);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                if (GameManager.sharedInstance.timeSinceLastPowerUP > 1f)
                {
                    //Buff
                    (GetComponent<SpriteRenderer>()).sprite = spritePowerUp;
                    yAcceleration = buffYForce;
                    GameManager.sharedInstance.timeSinceLastPowerUP = 0;
                    redPowerUpTrail.enabled = true;
                }
            }
        }
        if (GameManager.sharedInstance.timeSinceLastPowerUP > buffDuration &&
            (GetComponent<SpriteRenderer>()).sprite == spritePowerUp)
        {
            EndRedBuff();
        }
    }

    private void EndRedBuff()
    {
        (GetComponent<SpriteRenderer>()).sprite = spriteCheese;
        redPowerUpTrail.enabled = false;
        originalYForce = yAcceleration;
    }

    void FixedUpdate()
    {
        if (GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
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
    void GroundCheck()
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
                    landOnGrass.Play();
                }
            }
        }
    }

    private void GameOver()
    {
        rigidbody.drag = 3;
        //rigidbody.freezeRotation = true;
        deathParticle.Play();
        GetComponent<SpriteRenderer>().enabled = false;
        redPowerUpTrail.enabled = false;
        GameManager.sharedInstance.SetGameState(GameState.gameOver);
    }

    void MouseHitCheck()
    {
        //Checking wether the player is hit or not
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemies);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject && (GetComponent<SpriteRenderer>()).sprite == spriteCheese)
            {
                GameOver();
            } else if ((GetComponent<SpriteRenderer>()).sprite == spritePowerUp)
            {
                MiniBoost();
            }
        }
    }

    public void MiniBoost()
    {
        rigidbody.AddForce(new Vector2(velocityBoost * forceMultiplier, velocityBoost * forceMultiplier));
    }

    void BoundsCheck()
    {
        //Checking wether the player is hit or not
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsOutOfBounds);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                GameOver();
            }
        }
    }

    void Jump()
    {
        
        if (jump && onGround && rigidbody.velocity.x > 0 && timeSinceLastJump > 1)
        {
            
            rigidbody.AddForce(new Vector2(0, jumpForce * forceMultiplier * rigidbody.velocity.magnitude));
            jump = false;
            timeSinceLastJump = 0;
        }
        timeSinceLastJump += Time.deltaTime;
    }
    void AcceleratePlayer()
    {
        //Land accelleration (has my custom formula to fit my level design)
        if (onGround && rigidbody.velocity.x < maxXAceleration)
        {
            rigidbody.AddForce(new Vector2(xAcceleration * forceMultiplier * Math.Abs(rigidbody.velocity.normalized.x),
                0));
        }
        if (onGround && rigidbody.velocity.y < maxYAceleration)
        {
            rigidbody.AddForce(new Vector2(0,
                yAcceleration * forceMultiplier * rigidbody.velocity.normalized.y));
        }
        if (onGround && rigidbody.velocity.magnitude < minVelocity)
        {
            rigidbody.AddForce(new Vector2(xAcceleration * forceMultiplier * Math.Abs(rigidbody.velocity.normalized.x),
                yAcceleration * forceMultiplier * Math.Abs(rigidbody.velocity.normalized.y)));
        }
        rigidbody.velocity = Vector2.ClampMagnitude(rigidbody.velocity, maxVelocity);
    }
    void Descend()
    {
        
        if(descend)
        {
            rigidbody.AddForce(new Vector2(0, -descendingForce));
        }
        descend = false;
    }
    void DirectCamera()
    {
        //Camera Direction
        if (!onGround && timeSinceLastJump > cameraDelay)
        {
            if (vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y >= maxYOffset)
                vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y -= offsetYSensivity;
            if (vcam.m_Lens.OrthographicSize < maxFOV)
                vcam.m_Lens.OrthographicSize += camSensivity;
        }
        else
        {
            if (vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y <= minYOffset)
                vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y += offsetYSensivity;
            if (vcam.m_Lens.OrthographicSize > minFOV)
                vcam.m_Lens.OrthographicSize -= camSensivity;
        }

        if(rigidbody.velocity.y < -2 && timeSinceLastJump > cameraDelay)
        {
            if (vcam.GetCinemachineComponent<
                CinemachineTransposer>().m_FollowOffset.y >= maxYOffset)
                vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y -= offsetYSensivity;
            if (vcam.m_Lens.OrthographicSize < maxFOV)
                vcam.m_Lens.OrthographicSize += camSensivity;
        }
    }
}
