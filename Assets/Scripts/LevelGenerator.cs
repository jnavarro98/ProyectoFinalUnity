using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{

    private enum Phase { Rock, Lava, Ice }
    private Phase currentPhase;

    const int PHASE_SWITCH_AMOUNT = 2;

    public static LevelGenerator sharedInstance;
    public LevelBlock firstBlockRock;
    public LevelBlock firstBlockIce;
    public LevelBlock firstBlockLava;
    public Transform LevelStartPoint;
    public bool freezeEnemiesOutOfCurrentBlock = true;

    public List<LevelBlock> rockLevelBlocks = new List<LevelBlock>();
    public List<LevelBlock> lavaLevelBlocks = new List<LevelBlock>();
    public List<LevelBlock> iceLevelBlocks = new List<LevelBlock>();
    public int numInitialBlocks = 3;
    private int phaseTracker;
    private int blockCounter;
    public List<LevelBlock> currentBlocks = new List<LevelBlock>();
    int lastRockBlockIndex;
    int lastIceBlockIndex;
    int lastLavaBlockIndex;

    LevelBlock newBlock;

    private void Awake()
    {
        sharedInstance = this;
        currentPhase = (Phase)Random.Range(0, 3);
    }

    // Start is called before the first frame update
    void Start()
    {
        lastRockBlockIndex = -1;
        lastIceBlockIndex = -1;
        lastLavaBlockIndex = -1;
        GenerateInitialBlocks();
        FreezeInitialEnemies();
        blockCounter = 1;
        phaseTracker = 0;
    }

    private void CheckPhase()
    {
        if (phaseTracker == PHASE_SWITCH_AMOUNT)
        {
            while (currentPhase == (currentPhase = (Phase)Random.Range(0, 3))) ;
            phaseTracker = 0;
        }
    }

    public void AddBlock()
    {

        CheckPhase();
        
        Vector3 spawnPosition = Vector3.zero;

        if (currentBlocks.Count == 0)
        {
            switch (currentPhase)
            {
                case Phase.Rock:
                    newBlock = Instantiate(firstBlockRock);
                    break;
                case Phase.Ice:
                    newBlock = Instantiate(firstBlockIce);
                    break;
                case Phase.Lava:
                    newBlock = Instantiate(firstBlockLava);
                    break;
            }
            newBlock.transform.SetParent(transform, false); //soy hijo del levelGenerator
            spawnPosition = LevelStartPoint.position; //donde hago el spawn, en la posicion inicial
            phaseTracker--;
        }
        else
        {

            switch (currentPhase)
            {
                case Phase.Rock:
                    while (lastRockBlockIndex == (lastRockBlockIndex = Random.Range(0, rockLevelBlocks.Count))) ;
                    newBlock = Instantiate(rockLevelBlocks[lastRockBlockIndex]);
                    break;
                case Phase.Ice:
                    while (lastIceBlockIndex == (lastIceBlockIndex = Random.Range(0, iceLevelBlocks.Count))) ;
                    newBlock = Instantiate(iceLevelBlocks[lastIceBlockIndex]);
                    break;
                case Phase.Lava:
                    while (lastLavaBlockIndex == (lastLavaBlockIndex = Random.Range(0, lavaLevelBlocks.Count))) ;
                    newBlock = Instantiate(lavaLevelBlocks[lastLavaBlockIndex]);
                    break;
            }

            newBlock.transform.SetParent(transform, false); //soy hijo del levelGenerator
            spawnPosition = currentBlocks[currentBlocks.Count - 1].exitPoint.position; //donde hago el spawn, en el fin del anterior
        }
        Vector3 correction = new Vector3(
            spawnPosition.x - newBlock.startPoint.position.x,
            spawnPosition.y - newBlock.startPoint.position.y,
            20
            );
        //corrige de la posicion donde le has dado hasta la posicion donde debe de estar
        newBlock.transform.position = correction;
        currentBlocks.Add(newBlock);
        blockCounter++;
        phaseTracker++;
    }
    //int nextLevelBlockIndex = ((currentBlocks.Count - 1) / 2) + 1;
    public void FreezeInitialEnemies()
    {
        FreezeLevelBlockEnemies(2);
    }

    public void RemoveAllBlocks()
    {
        currentBlocks.Clear();
    }

    public void UnfreezeCurrentLevelBlockEnemies()
    {
        if(currentBlocks.Count == 3 && freezeEnemiesOutOfCurrentBlock)
        {
            int currentLevelBlockIndex = ((currentBlocks.Count - 1) / 2);
            foreach (FireballBehaviour enemy in currentBlocks[currentLevelBlockIndex].GetComponentsInChildren<FireballBehaviour>())
            {
                enemy.ySpeed = FireballBehaviour.BASE_SPEED;
            }
            foreach (BirdMovement enemy in currentBlocks[currentLevelBlockIndex].GetComponentsInChildren<BirdMovement>())
            {
                enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            }
        } else
        {
            Debug.LogWarning("Freeze is disabled");
        }
        FreezeLevelBlockEnemies(currentBlocks.Count -1);
    }

    public void FreezeLevelBlockEnemies(int index)
    {
        if (currentBlocks.Count == 3 && freezeEnemiesOutOfCurrentBlock)
        {
            foreach (FireballBehaviour enemy in currentBlocks[index].GetComponentsInChildren<FireballBehaviour>())
            {
                enemy.ySpeed = 0;
            }
            foreach (BirdMovement enemy in currentBlocks[index].GetComponentsInChildren<BirdMovement>())
            {
                enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                //Debug.Log("I'm in " + index + " index levelblock freezing");
            }
        } else
        {
            Debug.LogWarning("Freeze is disabled");
        }
    }

    public void RemoveOldestBlock()
    {
        Destroy(currentBlocks[0].gameObject);
        currentBlocks.RemoveAt(0);
    }

    public void GenerateInitialBlocks()
    {
        for (int i = 0; i < numInitialBlocks; i++)
        {
            AddBlock();
        }
    }
}
