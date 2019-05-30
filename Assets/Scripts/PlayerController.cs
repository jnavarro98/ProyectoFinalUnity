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
    public float baseForceJump = 10;
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
    public float buffYForce = 1.5f;

    bool jump = false;
    bool descend = false;
    private float initialXPosition;

    TrailRenderer trailRenderer;
    public Sprite spritePowerUp;
    Sprite originalSprite;
    SpriteRenderer spriteRenderer;
    private bool onGround;
    float radius;
    bool ascending;
    public int buffDuration;
    float ScreenWidth;

    public TrailRenderer redPowerUpTrail;

    public AudioSource grassEffect;
    public AudioSource landOnGrass;

    public LayerMask whatIsOutOfBounds;
    public LayerMask whatIsRedPowerUp;
    public LayerMask whatIsGround;
    public LayerMask whatIsEnemies;

    public CinemachineVirtualCamera vcam;
    


    // Start is called before the first frame update
    void Start()
    {
        timeSinceLastJump = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        ScreenWidth = Screen.width;
        radius = GetComponent<SpriteRenderer>().bounds.size.y;
        initialXPosition = transform.position.x;
        originalSprite = (GetComponent<SpriteRenderer>()).sprite;
        
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log(collider.gameObject.layer + " " + LayerMask.NameToLayer("Enemies"));
        if (collider.gameObject.layer == LayerMask.NameToLayer("Enemies"))
        {
            if (!GetComponent<TrailRenderer>().isVisible)
            {
                Debug.Log("Bola de fuego");
                GameOver();
            }
            else
            {
                Debug.Log("Bola de fuego");
                MiniBoost();
            }
        }
    }

    void Awake()
    {
        rigidbody.velocity = new Vector2(120, 120);
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
            //FireballCheck();
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
            Debug.Log("D");
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
                    //(GetComponent<SpriteRenderer>()).sprite = spritePowerUp;
                    yAcceleration += buffYForce;
                    GameManager.sharedInstance.timeSinceLastPowerUP = 0;
                    spriteRenderer.sprite = spritePowerUp;
                    redPowerUpTrail.enabled = true;
                }
            }
        }
        if (GameManager.sharedInstance.timeSinceLastPowerUP > buffDuration &&
            (GetComponent<TrailRenderer>()).isVisible)
        {
            EndRedBuff();
        }
    }

    private void EndRedBuff()
    {
        spriteRenderer.sprite = originalSprite;
        redPowerUpTrail.enabled = false;
        yAcceleration -= buffYForce;
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
                    Debug.Log(colliders[i].gameObject);
                    landOnGrass.Play();
                }
            }
        }
    }

    private void GameOver()
    {
        rigidbody.drag = 3;
        //rigidbody.freezeRotation = true;
        GetComponent<ParticleSystem>().Play();
        GetComponent<SpriteRenderer>().enabled = false;
        rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        redPowerUpTrail.enabled = false;
        GameManager.sharedInstance.SetGameState(GameState.gameOver);
    }

    void FireballCheck()
    {
        //Checking wether the player is hit or not
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemies);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                if (!GetComponent<TrailRenderer>().isVisible)
                {

                    Debug.Log("Bola de fuego");
                    GameOver();
                }
                else if (GetComponent<TrailRenderer>().isVisible)
                {
                    MiniBoost();
                }
            }
        }
    }

    public void MiniBoost()
    {
        float newXVelocity = Vector2.Reflect(rigidbody.velocity, Vector2.up).x;
        rigidbody.velocity = new Vector2(newXVelocity, baseForceJump);
    }

    void BoundsCheck()
    {
        //Checking wether the player is hit or not
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsOutOfBounds);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                Debug.Log(colliders[i].gameObject.name);
                Debug.Log("Me sali del mapa");
                GameOver();
            }
        }
    }

    void Jump()
    {
        
        if (jump && onGround && rigidbody.velocity.x > 0 && timeSinceLastJump > 1)
        {
            
            rigidbody.AddForce(new Vector2(0, (jumpForce * forceMultiplier * rigidbody.velocity.x) + baseForceJump), ForceMode2D.Impulse);
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
            if (vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y >= maxYOffset)
                vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y -= offsetYSensivity;
            if (vcam.m_Lens.OrthographicSize < maxFOV)
                vcam.m_Lens.OrthographicSize += camSensivity;
        }
        else
        {
            if (vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y <= minYOffset)
                vcam.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y += offsetYSensivity * 2.5f;
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
