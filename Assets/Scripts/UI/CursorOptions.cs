using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CursorOptions : MonoBehaviour
{
    public float animationOffset = 1.0f / 16.0f; // es mourà 1 píxel ja que treballem amb unitats de 16 (16 píxels són 1 a la transform)
    public float animationSpeed = 0.25f;

    float animationTimer = 0.0f;
    uint currentFrame = 0;

    public UnityEvent sendO;

    // Start is called before the first frame update
    void Start()
    {
        sendO = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        if(isActiveAndEnabled)
        {
            Animate();
        }

        DrawLines();
    }

    public void MyOnEnable()
    {
        Reposition();
        SubscribeToEvents();
    }

    public void MyOnDisable()
    {
        UnsubscribeFromEvents();
    }

    void Animate()
    {
        animationTimer += Time.deltaTime;

        if (animationTimer >= animationSpeed)
        {
            if (currentFrame == 0)
            {
                currentFrame = 1;
                transform.position += new Vector3(animationOffset, 0, 0);
            }
            else if (currentFrame == 1)
            {
                currentFrame = 0;
                transform.position += new Vector3(-animationOffset, 0, 0);
            }

            animationTimer = 0.0f;
        }
    }

    public void MoveUp()
    {
        bool moved = false;
        int index = transform.GetComponentInParent<MenuOptionsController>().GetButtonList().IndexOf(transform.GetComponentInParent<MenuOptionsController>().GetSelectedButton());

        if (index != -1)
        {
            if (index == 0) // el botó és el de dalt de tot
            {
                transform.GetComponentInParent<MenuOptionsController>().SelectButton(
                    transform.GetComponentInParent<MenuOptionsController>().GetButtonList().Count - 1);
                moved = true;
            }
            else
            {
                transform.GetComponentInParent<MenuOptionsController>().SelectButton(index - 1);
                moved = true;
            }
        }

        if (moved)
        {
            Reposition();
        }
    }

    public void MoveDown()
    {
        bool moved = false;
        int index = transform.GetComponentInParent<MenuOptionsController>().GetButtonList().IndexOf(transform.GetComponentInParent<MenuOptionsController>().GetSelectedButton());

        if (index != -1)
        {
            if (index == transform.GetComponentInParent<MenuOptionsController>().GetButtonList().Count - 1) // el botó és el de baix de tot
            {
                transform.GetComponentInParent<MenuOptionsController>().SelectButton(0);
                moved = true;
            }
            else
            {
                transform.GetComponentInParent<MenuOptionsController>().SelectButton(index + 1);
                moved = true;
            }
        }

        if (moved)
        {
            Reposition();
        }
    }

    public RaycastHit2D CheckRightButton()
    {
        Vector2 from = transform.position;
        Vector2 to = from + new Vector2(2, 0);

        RaycastHit2D result = Physics2D.Linecast(from, to, LayerMask.GetMask("Buttons")); //Buttons

        if (result.collider != null)
            Debug.Log("Collisioned to the right with: " + result.collider.gameObject.name);

        return result;
    }

    public RaycastHit2D CheckUpButton()
    {
        Vector2 from = transform.position;
        Vector2 to = from + new Vector2(0, 1);

        RaycastHit2D result = Physics2D.Linecast(from, to, LayerMask.GetMask("Buttons")); //Buttons

        if (result.collider != null)
            Debug.Log("Collisioned upwards with: " + result.collider.gameObject.name);

        return result;
    }

    public RaycastHit2D CheckDownButton()
    {
        Vector2 from = transform.position;
        Vector2 to = from + new Vector2(0, -1);

        Debug.DrawLine(from, to, Color.green);

        RaycastHit2D result = Physics2D.Linecast(from, to, LayerMask.GetMask("Buttons")); //Buttons

        if (result.collider != null)
            Debug.Log("Collisioned downwards with: " + result.collider.gameObject.name);

        return result;
    }

    void Reposition()
    {
        transform.position = transform.GetComponentInParent<MenuOptionsController>().GetSelectedButton().transform.position + new Vector3(-1, 0, 0);
        AdjustOffset();
    }

    void AdjustOffset()
    {
        transform.position += new Vector3(0, -13f / 16f, 0);
    }

    void DrawLines()
    {
        Vector2 from = transform.position;
        Vector2 right = from + new Vector2(2, 0);
        Debug.DrawLine(from, right, Color.green);
    }

    void SendO()
    {
        sendO.Invoke();
    }
    void SubscribeToEvents()
    {
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_w_down.AddListener(MoveUp); // W
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_s_down.AddListener(MoveDown); // S
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_o_down.AddListener(SendO); // O
    }

    void UnsubscribeFromEvents()
    {
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_w_down.RemoveListener(MoveUp); // W
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_s_down.RemoveListener(MoveDown); // S
        GameObject.Find("Controls").GetComponent<Controls>().keyboard_o_down.RemoveListener(SendO); // O
    }
}
