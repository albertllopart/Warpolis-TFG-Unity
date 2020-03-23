using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Number : MonoBehaviour
{
    public Sprite zero;
    public Sprite one;
    public Sprite two;
    public Sprite three;
    public Sprite four;
    public Sprite five;
    public Sprite six;
    public Sprite seven;
    public Sprite eight;
    public Sprite nine;

    List<Sprite> numbers = new List<Sprite>();
    // Start is called before the first frame update
    void Start()
    {
        SetupNumbers();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupNumbers()
    {
        numbers = new List<Sprite>();

        numbers.Add(zero);
        numbers.Add(one);
        numbers.Add(two);
        numbers.Add(three);
        numbers.Add(four);
        numbers.Add(five);
        numbers.Add(six);
        numbers.Add(seven);
        numbers.Add(eight);
        numbers.Add(nine);
    }

    public void SetNumber(int num)
    {
        if (numbers.Count > num)
            GetComponent<SpriteRenderer>().sprite = numbers[num];
    }
}
