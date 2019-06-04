using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveBuff : MonoBehaviour
{
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
        GetComponent<AudioSource>().Play();
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
