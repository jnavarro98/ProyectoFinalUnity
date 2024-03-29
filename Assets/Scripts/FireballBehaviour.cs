﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireballBehaviour : MonoBehaviour
{
    private bool onGround;
    
    float radius;
    public LayerMask whatIsPlayer;
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
            if(IsReady() || ySpeed != 0)
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
        sprite.flipX = yDistance < 0;

        playerFilter = new ContactFilter2D();
        playerFilter.SetLayerMask(whatIsPlayer);
        target = new Vector3(transform.localPosition.x, transform.localPosition.y + yDistance, transform.localPosition.z);
        origin = transform.localPosition;
        nextPosition = target;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.layer == LayerMask.NameToLayer("Player"))
            Despawn();
    }

    private void Despawn()
    {
        GetComponent<AudioSource>().Play();
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<TrailRenderer>().enabled = false;
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
        sprite.flipX = sprite.flipX != true ? true : false;
    }
}
