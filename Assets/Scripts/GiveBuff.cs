using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveBuff : MonoBehaviour
{
    float radius;
    [SerializeField] private LayerMask whatIsPlayer;
    public AudioSource powerUpSound;
    bool hasBeenTaken;
    // Start is called before the first frame update
    void Start()
    {
        radius = GetComponent<CircleCollider2D>().radius;
    }

    void Awake()
    {
        hasBeenTaken = false;
    }

    // Update is called once per frame
    void Update()
    {
        Disappear();
    }
    

    private void Disappear()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsPlayer);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject && !hasBeenTaken)
            {
                powerUpSound.Play();
                GetComponent<SpriteRenderer>().enabled = false;
                hasBeenTaken = true;
            }
        }
    }
}
