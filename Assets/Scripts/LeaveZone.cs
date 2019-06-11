using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveZone : MonoBehaviour
{
    static float timeSinceLastDestruction = 0.0f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(timeSinceLastDestruction > 0.5f && collision.CompareTag("Player"))
        {
            
            LevelGenerator.sharedInstance.AddBlock();
            LevelGenerator.sharedInstance.RemoveOldestBlock();
            LevelGenerator.sharedInstance.UnfreezeCurrentLevelBlockEnemies();
            timeSinceLastDestruction = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastDestruction += Time.deltaTime;
    }
}
