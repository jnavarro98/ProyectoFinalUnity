using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveBuff : MonoBehaviour
{
    void OnBecameVisible()
    {
        enabled = true;
    }
    void OnBecameInvisible()
    {
        enabled = false;
    }

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
        AudioSource soundEffect = GetComponent<AudioSource>();
        soundEffect.Play();
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        Invoke("Destroy", soundEffect.clip.length);
    }

    void Destroy()
    {
        gameObject.SetActive(false);
    }
}
