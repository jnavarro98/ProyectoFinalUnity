using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBlock : MonoBehaviour
{
    public Transform startPoint;
    public Transform exitPoint;

    void Awake()
    {
        foreach (Transform gameObject in GetComponent<Transform>())
        {
            gameObject.position = new Vector3(gameObject.position.x, gameObject.position.y,
                transform.position.z);
        }
    }


}
