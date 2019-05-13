using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator sharedInstance;
    public LevelBlock firstBlock;
    public Transform LevelStartPoint;
    public bool freezeEnemiesOutOfCurrentBlock = true;

    public List<LevelBlock> easyLevelBlocks = new List<LevelBlock>();
    public List<LevelBlock> transitionBlocks = new List<LevelBlock>();
    public int numInitialBlocks = 3;
    private int blockCounter;
    public List<LevelBlock> currentBlocks = new List<LevelBlock>();
    int lastNormalBlockIndex;
    int lastTransitionBlockIndex;

    private void Awake()
    {
        sharedInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        lastNormalBlockIndex = -1;
        lastTransitionBlockIndex = -1;
        GenerateInitialBlocks();
        FreezeInitialEnemies();
        blockCounter = 1;
    }

    public void AddBlock()
    {

        LevelBlock newBlock;
        Vector3 spawnPosition = new Vector3(0,0,20); //inicializo la posicion a (0,0,10)
        if (currentBlocks.Count == 0)
        {
            newBlock = Instantiate(firstBlock); //en versiones anteriores es necesairo el cast (LevelBlock)Instantiate(firstBlock)
            newBlock.transform.SetParent(transform, false); //soy hijo del levelGenerator
            spawnPosition = LevelStartPoint.position; //donde hago el spawn, en la posicion inicial
        }
        else
        {
            if(blockCounter % 2 == 0)
            {
                while (lastNormalBlockIndex == (lastNormalBlockIndex  = Random.Range(0, easyLevelBlocks.Count))) ;
                newBlock = Instantiate(easyLevelBlocks[lastNormalBlockIndex]);
            }
            else
            {
                while (lastTransitionBlockIndex == (lastTransitionBlockIndex = Random.Range(0, transitionBlocks.Count)));
                newBlock = Instantiate(transitionBlocks[lastTransitionBlockIndex]);
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
            foreach (MouseMovement enemy in currentBlocks[currentLevelBlockIndex].GetComponentsInChildren<MouseMovement>())
            {
                enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                Debug.Log("I'm in index " + currentLevelBlockIndex + " levelblock unfreezing");
            }
            foreach (BirdMovement enemy in currentBlocks[currentLevelBlockIndex].GetComponentsInChildren<BirdMovement>())
            {
                enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
                Debug.Log("I'm in index " + currentLevelBlockIndex + " levelblock unfreezing");
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
            foreach (MouseMovement enemy in currentBlocks[index].GetComponentsInChildren<MouseMovement>())
            {
                enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                Debug.Log("I'm in " + index + " index levelblock freezing");
            }
            foreach (BirdMovement enemy in currentBlocks[index].GetComponentsInChildren<BirdMovement>())
            {
                enemy.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                Debug.Log("I'm in " + index + " index levelblock freezing");
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
