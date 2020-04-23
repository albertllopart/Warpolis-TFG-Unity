using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private float timer = 0.0f;

    private void Update()
    {
        if (timer >= 1f)
        {
            Loader.LoaderCallback();
        }

        timer += Time.deltaTime;
    }
}
