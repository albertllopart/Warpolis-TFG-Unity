using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    enum SwingState
    {
        GOING, RETURNING
    };

    public uint pixels;
    public uint direction;
    public float speed;
    float timer = 0.0f;

    //intern
    SwingState state = SwingState.GOING;
    uint pixelsCount = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= speed)
        {
            DoTheSwing();
            timer = 0.0f;
        }
    }

    void DoTheSwing()
    {
        switch (direction)
        {
            case 0: //vertical
                ApplySwingVector(new Vector3(0, 1/16f, 0));
                break;

            case 1: //horitzontal
                ApplySwingVector(new Vector3(1/16f, 0, 0));
                break;
        }
    }

    void ApplySwingVector(Vector3 swing)
    {
        if (state == SwingState.GOING)
        {
            if (pixelsCount < pixels)
            {
                transform.position += swing;
                pixelsCount++;
            }
            else
            {
                pixelsCount = 0;
                state = SwingState.RETURNING;
            }
        }
        else if (state == SwingState.RETURNING)
        {
            if (pixelsCount < pixels)
            {
                transform.position -= swing;
                pixelsCount++;
            }
            else
            {
                pixelsCount = 0;
                state = SwingState.GOING;
            }
        }
    }
}
