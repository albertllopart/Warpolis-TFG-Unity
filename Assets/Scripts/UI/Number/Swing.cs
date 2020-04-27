using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Swing : MonoBehaviour
{
    public enum Direction { GOING, RETURNING };

    public enum Orientation { VERTICAL, HORIZONTAL };

    public uint pixels;
    public Orientation orientation;
    public Direction direction;
    public float speed;
    float timer = 0.0f;

    
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
        switch (orientation)
        {
            case Orientation.VERTICAL: //vertical
                ApplySwingVector(new Vector3(0, 1/16f, 0));
                break;

            case Orientation.HORIZONTAL: //horitzontal
                ApplySwingVector(new Vector3(1/16f, 0, 0));
                break;
        }
    }

    void ApplySwingVector(Vector3 swing)
    {
        if (direction == Direction.GOING)
        {
            if (pixelsCount < pixels)
            {
                transform.position += swing;
                pixelsCount++;
            }
            else
            {
                pixelsCount = 0;
                direction = Direction.RETURNING;
            }
        }
        else if (direction == Direction.RETURNING)
        {
            if (pixelsCount < pixels)
            {
                transform.position -= swing;
                pixelsCount++;
            }
            else
            {
                pixelsCount = 0;
                direction = Direction.GOING;
            }
        }
    }
}
