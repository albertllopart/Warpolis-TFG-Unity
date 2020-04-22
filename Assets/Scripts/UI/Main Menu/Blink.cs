using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    public float activeTime;
    public float blinkTime;

    float timer = 0.0f;
    bool isEnabled = true;

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        switch (isEnabled)
        {
            case true:
                if (timer >= activeTime)
                {
                    isEnabled = !isEnabled;
                    foreach (Transform child in transform)
                    {
                        child.gameObject.SetActive(isEnabled);
                    }
                    timer = 0.0f;
                }
                break;

            case false:
                if (timer >= blinkTime)
                {
                    isEnabled = !isEnabled;
                    foreach (Transform child in transform)
                    {
                        child.gameObject.SetActive(isEnabled);
                    }
                    timer = 0.0f;
                }
                break;
        }
        
    }
}
