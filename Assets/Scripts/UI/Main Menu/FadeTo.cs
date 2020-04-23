using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FadeTo : MonoBehaviour
{
    public Color color;
    public float alphaDecreaseSpeed;
    public float alphaIncreaseSpeed;
    public bool playOnAwake;

    bool play;
    float currentAlpha;

    enum State { INCREASING, DECREASING };
    State state;

    public UnityEvent finishedDecreasing;
    public UnityEvent finishedIncreasing;

    void Awake()
    {
        finishedIncreasing = new UnityEvent();
        finishedDecreasing = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (playOnAwake)
        {
            GetComponent<SpriteRenderer>().color = color;
            play = true;
            currentAlpha = 1.0f;
            state = State.DECREASING;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0);
            play = false;
            currentAlpha = 0.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (play)
        {
            switch (state)
            {
                case State.INCREASING:
                    if (IncreaseAlpha())
                        play = false;
                    break;

                case State.DECREASING:
                    if (DecreaseAlpha())
                        play = false;
                    break;
            }
        }
    }

    public void FadeToSetup()
    {
        play = true;
        state = State.INCREASING;
        currentAlpha = 0.0f;
    }

    bool IncreaseAlpha()
    {
        currentAlpha += alphaIncreaseSpeed * Time.deltaTime;
        GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, currentAlpha);

        if (currentAlpha >= 1.0f)
        {
            finishedIncreasing.Invoke();
            return true;
        }

        return false;
    }

    bool DecreaseAlpha()
    {
        currentAlpha -= alphaDecreaseSpeed * Time.deltaTime;
        GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, currentAlpha);

        if (currentAlpha <= 0.0f)
        {
            finishedDecreasing.Invoke();
            return true;
        }

        return false;
    }
}
