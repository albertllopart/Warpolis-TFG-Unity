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
}
