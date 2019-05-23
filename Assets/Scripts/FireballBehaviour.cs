using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireballBehaviour : MonoBehaviour
{
    private bool onGround;
    
    float radius;
    public LayerMask whatIsPlayer;
    bool isDead;
    public float yDistance = 100;
    public float ySpeed = 15;
    public const float BASE_SPEED = 200;
    float distanceToTarget;
    float yStep;
    public float delay = 0;
    float timeActive = 0;
    SpriteRenderer sprite;

    ContactFilter2D playerFilter;

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
            if(IsReady())
                Move();
        }
    }

    bool IsReady()
    {
        
        timeActive += Time.deltaTime;
        return timeActive >= delay;
    }

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

        if (ySpeed < 0)
        {
            sprite.flipX = true;
            yDistance *= -1;
        }
        playerFilter = new ContactFilter2D();
        playerFilter.SetLayerMask(whatIsPlayer);
        target = new Vector3(transform.localPosition.x, transform.localPosition.y + yDistance, transform.localPosition.z);
        origin = transform.localPosition;
        nextPosition = target;
        isDead = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        GetComponent<ParticleSystem>().Play();
        GetComponent<AudioSource>().Play();
        Despawn();
        isDead = true;
    }

    private void Despawn()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Move()
    {
        distanceToTarget = (transform.localPosition.y - target.y) * Time.deltaTime;

        yStep = ySpeed * Time.deltaTime * Math.Abs(distanceToTarget);
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, nextPosition, yStep + Time.deltaTime * 2);

        if(Vector3.Distance(transform.localPosition,nextPosition) < Math.Abs(yStep))
        {
            ChangePosition();
        }
    }

    private void ChangePosition()
    {
        nextPosition = nextPosition == target ? origin : target;
        sprite.flipX = sprite.flipX != true ?
            true : false;
    }
}
