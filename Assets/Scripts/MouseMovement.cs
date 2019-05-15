using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseMovement : MonoBehaviour
{
    private bool onGround;
    [SerializeField] private LayerMask whatIsGround;
    public float jumpingForce = 10f;
    float radius;
    public BoxCollider2D baseColider;
    public AudioSource deathSound;
    [SerializeField] private LayerMask whatIsPlayer;
    public ParticleSystem explosion;
    bool isDead;
    public bool canJump;

    // Start is called before the first frame update
    void Start()
    {
        radius = baseColider.bounds.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
            if(canJump)
                Jump();
            CheckDeath();
        }
    }

    void Awake()
    {
        isDead = false;
    }

    private void CheckDeath()
    {
        
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsPlayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject && !isDead)
            {
                explosion.Play();
                deathSound.Play();
                Invoke("Despawn", 0.5f);
                isDead = true;
            }
        }
    }

    private void Despawn()
    {
        gameObject.SetActive(false);
    }

    private void Jump()
    {
        //Checking wether the enemy is on ground or not
        bool wasGrounded = onGround;
        onGround = false;
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                onGround = true;
            }
        }
        //Jump
        if (onGround)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, jumpingForce));
        }
    }
}
