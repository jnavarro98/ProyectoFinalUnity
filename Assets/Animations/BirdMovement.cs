using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BirdMovement : MonoBehaviour
{
    public float jumpingForce = 10f;
    float radius;
    public float maxXVelocity = 10;
    public AudioSource deathSound;
    [SerializeField] private LayerMask whatIsPlayer;
    public ParticleSystem explosion;
    bool isDead;
    float timeSinceLastJump;
    public bool canJump;
    Rigidbody2D rigidbody;
    
    // Start is called before the first frame update
    void Start()
    {
        radius = GetComponent<CircleCollider2D>().bounds.size.y;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.sharedInstance.currentGameState == GameState.inGame)
        {
            Jump();
            CheckDeath();
        }
    }

    private void Despawn()
    {
        gameObject.SetActive(false);
    }

    void Awake()
    {
        isDead = false;
        timeSinceLastJump = 0;
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

    private void Jump()
    {
        if(timeSinceLastJump >= 0.2f)
        {
            rigidbody.AddForce(new Vector2(-10, jumpingForce));
            timeSinceLastJump = 0;
        }
        if (rigidbody.velocity.x > maxXVelocity)
            rigidbody.velocity = new Vector2(maxXVelocity, rigidbody.velocity.y);
        timeSinceLastJump += Time.deltaTime;
    }
}
