using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{

    [SerializeField]
    private int m_FadeInTime = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.sharedInstance.backgroundMusic.volume < 1)
        {
            GameManager.sharedInstance.backgroundMusic.volume =
                GameManager.sharedInstance.backgroundMusic.volume + (Time.deltaTime / (m_FadeInTime + 1));
        }
        else
        {
            Destroy(this);
        }
    }
}
