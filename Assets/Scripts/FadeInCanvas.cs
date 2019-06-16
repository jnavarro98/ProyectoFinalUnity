using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInCanvas : MonoBehaviour
{

    public float secondsToFade = 2;

    void Start()
    {
        StartCoroutine(DoFade());
    }

    IEnumerator DoFade()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        while(canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime / secondsToFade;    
            yield return null;
        }
        yield return null;
    }
}
