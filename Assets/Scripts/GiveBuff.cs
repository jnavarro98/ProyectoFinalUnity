﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveBuff : MonoBehaviour
{
    public AudioSource powerUpSound;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        powerUpSound.Play();
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
