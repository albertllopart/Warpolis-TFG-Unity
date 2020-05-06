using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Announcer : MonoBehaviour
{
    public GameObject myTextDouble;
    public GameObject number;

    //skins
    public Sprite cani;
    public Sprite hipster;

    GameObject numbers;

    //movement
    float speed = 10.0f;
    bool moving = true;
    bool fading = false;

    //events
    public UnityEvent announcementEnded;

    public enum AnnouncementType
    {
        TURN_COUNT, TURN_LIMIT_REACHED
    };

    void Awake()
    {
        numbers = transform.Find("Numbers").gameObject;

        announcementEnded = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.position = Camera.main.transform.position + new Vector3(0, 8, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if (moving)
        {
            Move();
        }
        
        if (fading)
        {
            Fade();
        }
    }

    public void CreateAnnouncement(AnnouncementType type)
    {
        switch (type)
        {
            case AnnouncementType.TURN_COUNT:
                SetTurnCount();
                SetSkin();
                break;

            case AnnouncementType.TURN_LIMIT_REACHED:
                SetTurnLimitReached();
                break;
        }
    }

    void SetTurnCount()
    {
        GameObject newTextDouble = Instantiate(myTextDouble);
        newTextDouble.transform.SetParent(transform);
        newTextDouble.transform.position = transform.position;
        newTextDouble.transform.Find("Text1").GetComponent<MyText>().text = "Turn";
        newTextDouble.transform.Find("Text2").GetComponent<MyText>().text = "/";

        GameObject currentTurn = Instantiate(number);
        currentTurn.transform.SetParent(numbers.transform.Find("Number1").transform); 
        currentTurn.transform.position = numbers.transform.Find("Number1").transform.position;
        currentTurn.GetComponent<Number>().CreateNumber(FindObjectOfType<DataController>().currentTurn);

        GameObject turnLimit = Instantiate(number);
        turnLimit.transform.SetParent(numbers.transform.Find("Number2").transform);
        turnLimit.transform.position = numbers.transform.Find("Number2").transform.position;

        if (FindObjectOfType<DataController>().turnLimit == 0)
            turnLimit.GetComponent<Number>().SetInfinite();
        else
            turnLimit.GetComponent<Number>().CreateNumber(FindObjectOfType<DataController>().turnLimit);
    }

    void SetSkin()
    {
        switch (FindObjectOfType<CutsceneController>().currentTurn)
        {
            case GameplayController.Turn.CANI:
                GetComponent<SpriteRenderer>().sprite = cani;
                break;

            case GameplayController.Turn.HIPSTER:
                GetComponent<SpriteRenderer>().sprite = hipster;
                break;
        }
    }

    void SetTurnLimitReached()
    {
        GameObject newTextDouble = Instantiate(myTextDouble);
        newTextDouble.transform.SetParent(transform);
        newTextDouble.transform.position = transform.position;
        newTextDouble.transform.Find("Text1").GetComponent<MyText>().text = "Turn Limit";
        newTextDouble.transform.Find("Text2").GetComponent<MyText>().text = "has been reached!";
    }

    void Move()
    {
        Vector3 difference = Camera.main.transform.position - transform.position + new Vector3(0, 0, 10);

        if (difference.magnitude <= speed * Time.deltaTime)
        {
            transform.position = Camera.main.transform.position + new Vector3(0, 0, 10);
            moving = false;
            SubscribeToEvents();
        }
        else
            transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
    }

    void FadeSetup()
    {
        fading = true;
        UnsubscribeFromEvents();
    }

    void Fade()
    {
        transform.position += new Vector3(0, speed * Time.deltaTime, 0);

        Vector3 difference = Camera.main.transform.position - transform.position + new Vector3(0, 0, 10);
        if (difference.magnitude >= 8)
        {
            announcementEnded.Invoke();
            Destroy(gameObject);
        }
    }

    void SubscribeToEvents()
    {
        FindObjectOfType<Controls>().keyboard_o_down.AddListener(FadeSetup);
    }

    void UnsubscribeFromEvents()
    {
        FindObjectOfType<Controls>().keyboard_o_down.RemoveListener(FadeSetup);
    }
}
