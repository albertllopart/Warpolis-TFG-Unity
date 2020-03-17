using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Hitpoints : MonoBehaviour
{
    // Start is called before the first frame update
    public Sprite one;
    public Sprite two;
    public Sprite three;
    public Sprite four;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetSprite(uint hp)
    {
        switch (hp)
        {
            case 1:
                GetComponent<SpriteRenderer>().sprite = one;
                break;

            case 2:
                GetComponent<SpriteRenderer>().sprite = two;
                break;

            case 3:
                GetComponent<SpriteRenderer>().sprite = three;
                break;

            case 4:
                GetComponent<SpriteRenderer>().sprite = four;
                break;

            case 5:
                GetComponent<SpriteRenderer>().sprite = null;
                break;
        }
    }
}
