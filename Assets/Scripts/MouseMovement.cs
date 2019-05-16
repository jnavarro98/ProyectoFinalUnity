using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseMovement : MonoBehaviour
{
    private bool onGround;
    [SerializeField] private LayerMask whatIsGround;
    
    float radius;
    public BoxCollider2D baseColider;
    public AudioSource deathSound;
    [SerializeField] private LayerMask whatIsPlayer;
    public ParticleSystem explosion;
    bool isDead;
    public bool canJump;
    public float maxY = 100;
    public float ySpeed = 15;
    float distanceToTarget;
    float yStep;

    Vector3 target;
    Vector3 origin;
    Vector3 nextPosition;

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
            Move();
            CheckDeath();  
        }
    }

    void Awake()
    {
        target = new Vector3(transform.localPosition.x, transform.localPosition.y + maxY, transform.localPosition.z);
        origin = transform.localPosition;
        nextPosition = target;
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

    private void Move()
    {
        distanceToTarget = (transform.localPosition.y - target.y) * Time.deltaTime;

        yStep = ySpeed * Time.deltaTime * Math.Abs(distanceToTarget);
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, nextPosition, yStep + Time.deltaTime * 2);

        if(Vector3.Distance(transform.localPosition,nextPosition) < yStep)
        {
            ChangePosition();
        }
    }

    private void ChangePosition()
    {
        nextPosition = nextPosition == target ? origin : target;
        GetComponent<SpriteRenderer>().flipX = GetComponent<SpriteRenderer>().flipX != true ?
            true : false;
    }
}
