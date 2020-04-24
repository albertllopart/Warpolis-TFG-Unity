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

    public bool play;
    public float currentAlpha;

    public enum State { INCREASING, DECREASING };
    public State state;

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
            SetPlay(true);
            currentAlpha = 1.0f;
            state = State.DECREASING;
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, 0);
            SetPlay(false);
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
                    SetPlay(!IncreaseAlpha());

                    if (!play)
                        finishedIncreasing.Invoke();
                    break;

                case State.DECREASING:
                    SetPlay(!DecreaseAlpha());

                    if (!play)
                        finishedDecreasing.Invoke();
                    break;
            }
        }
    }

    public void FadeToSetup()
    {
        SetPlay(true);
        currentAlpha = 0.0f;

        Debug.Log("FadeTo::FadeFromSetup - Starting to Increase Alpha");
    }

    public void FadeFromSetup()
    {
        SetPlay(true);
        currentAlpha = 1.0f;

        Debug.Log("FadeTo::FadeFromSetup - Starting to Decrease Alpha");
    }

    bool IncreaseAlpha()
    {
        currentAlpha += alphaIncreaseSpeed * Time.deltaTime;
        GetComponent<SpriteRenderer>().color = new Color(color.r, color.g, color.b, currentAlpha);

        if (currentAlpha >= 1.0f)
        {
            state = State.DECREASING;
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
            state = State.INCREASING;
            return true;
        }

        return false;
    }

    void SetPlay(bool set)
    {
        play = set;
    }
}
