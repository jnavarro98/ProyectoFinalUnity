using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInCanvas : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(DoFade());
    }

    IEnumerator DoFade()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        while(canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }
}
