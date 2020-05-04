using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Number : MonoBehaviour
{
    //xifres
    [Header("Xifres")]
    public GameObject numberInstance;
    public Sprite dot;
    private uint counter = 0;
    private float spacing = -7/16f;
    private float dotSpacing = 0.0f;

    //posicionament
    Vector3 startPosition;

    //individual
    [Header("Individual")]
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
    public Sprite infinite;

    List<Sprite> numbers = new List<Sprite>();
    int orderInLayer = 30;

    void Awake()
    {
        SetupNumbers();
    }

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
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

    public void SetInfinite()
    {
        DeleteNumber();
        GetComponent<SpriteRenderer>().sprite = infinite;
    }

    public void CenterNumber()
    {
        //aquest mètode és per centrar un nombre de dues xifres
        int count = 0;

        foreach (Transform child in transform)
            count++;
    }

    public void CreateNumber(int num)
    {
        DeleteNumber();

        List<int> listOfInt = GetNumbersFromInt(num);

        foreach(int number in listOfInt)
        {
            if (counter == 3)
            {
                dotSpacing = -4 / 16f;

                GameObject dotInstance = Instantiate(numberInstance, transform.position + new Vector3(spacing * counter - spacing - 3/16f, 0, 0), Quaternion.identity);
                dotInstance.transform.parent = transform;
                dotInstance.GetComponent<SpriteRenderer>().sprite = dot;
                dotInstance.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;
            }

            GameObject currentInstance = Instantiate(numberInstance, transform.position + new Vector3(spacing * counter + dotSpacing, 0, 0), Quaternion.identity);
            currentInstance.transform.parent = transform;
            currentInstance.GetComponent<SpriteRenderer>().sprite = numbers[number];
            currentInstance.GetComponent<SpriteRenderer>().sortingOrder = orderInLayer;

            counter++;
        }

        dotSpacing = 0.0f;
        counter = 0;
    }

    List<int> GetNumbersFromInt(int num)
    {
        List<int> listOfInt = new List<int>();

        if (num == 0)
        {
            listOfInt.Add(0);
            return listOfInt;
        }

        while (num > 0)
        {
            listOfInt.Add(num % 10);
            num = num / 10;
        }

        return listOfInt;
    }

    void DeleteNumber()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        if (GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().sprite = null;
    }
}
