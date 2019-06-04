using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarBehaviour : MonoBehaviour
{
    public AudioSource starEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.sharedInstance.starsAmount++;
        starEffect.Play();
        ParticleSystem.EmissionModule emission = GetComponent<ParticleSystem>().emission;
        emission.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false; 
    }
}
