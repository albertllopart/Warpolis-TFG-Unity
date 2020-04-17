using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealSign : MonoBehaviour
{
    public float timer = 0.0f;
    public float alpha = 1.2f;
    public float alphaTime = 0.05f;

    public UnityEvent animationEnd;

    // Start is called before the first frame update
    void Start()
    {
        animationEnd = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= alphaTime)
        {
            timer = 0.0f;
            alphaTime += 0.01f;

            transform.position += new Vector3(0.0f, 1 / 16f);

            alpha -= 0.15f;

            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
        }

        if (alpha <= 0.0f)
            Destroy(gameObject);
    }
}
