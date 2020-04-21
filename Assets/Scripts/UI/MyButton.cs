using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButtonTemplate
{
    public virtual void OnClick()
    {
        
    }
}

public class MyButton : MonoBehaviour
{
    public MyButtonTemplate button;

    public Sprite idle;
    public Sprite highlighted;

    public bool isEnabled;
    public int shopValue = 0;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnHighlight()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = highlighted;
    }

    public void OnIdle()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = idle;
    }

    public void OnDisabled()
    {
        //aplicar capa de color gris
        Color color = new Color(128 / 255f, 128 / 255f, 128 / 255f, 90 / 255f);
        GetComponent<SpriteRenderer>().color = color;

        foreach (Transform child in transform.Find("Number").transform)
        {
            child.GetComponent<SpriteRenderer>().color = color;
        }

        foreach (Transform character in transform.Find("MyText").transform.Find("Text").transform)
        {
            character.GetComponent<SpriteRenderer>().color = color;
        }

        isEnabled = false;
    }

    public void OnEnabled()
    {
        //aplicar capa de color blanc
        Color color = new Color(1, 1, 1, 1);
        GetComponent<SpriteRenderer>().color = color;

        foreach (Transform child in transform.Find("Number").transform)
        {
            child.GetComponent<SpriteRenderer>().color = color;
        }

        foreach (Transform character in transform.Find("MyText").transform.Find("Text").transform)
        {
            character.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 1);
        }

        isEnabled = true;
    }
}
