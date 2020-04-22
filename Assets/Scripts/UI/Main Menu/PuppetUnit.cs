using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetUnit : MonoBehaviour
{
    public float speed = 1f;
    public float maxDistance = 25f;
    float accumulatedMovement = 0.0f;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        accumulatedMovement += speed * Time.deltaTime;

        if (accumulatedMovement >= maxDistance)
            Destroy(gameObject);
    }
}
