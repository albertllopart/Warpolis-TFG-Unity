using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class ResultsController : MonoBehaviour
{
    public Sprite cani;
    public Sprite hipster;
    public Color caniColor;
    public Color hipsterColor;
    public Color drawColor;
    public GameObject myText;

    public float popTime;
    float timer = 0.0f;

    GameObject popList;
    List<GameObject> toPop;

    bool popping = false;
    int index = 0;

    //events
    UnityEvent finishedPopping;

    void Awake()
    {
        finishedPopping = new UnityEvent();
    }

    void Start()
    {
        popList = transform.Find("PopList").gameObject;

        toPop = new List<GameObject>();
        toPop.Add(popList.transform.Find("Winner").gameObject);
        toPop.Add(popList.transform.Find("Sprite").gameObject);
        toPop.Add(popList.transform.Find("By").gameObject);
        toPop.Add(popList.transform.Find("Wincon").gameObject);
        toPop.Add(popList.transform.Find("Turns").gameObject);
        toPop.Add(popList.transform.Find("Number").gameObject);
        toPop.Add(popList.transform.Find("Map").gameObject);
        toPop.Add(popList.transform.Find("MapName").gameObject);
        toPop.Add(popList.transform.Find("Minimap").gameObject);
        toPop.Add(popList.transform.Find("PressStart").gameObject);

        BuildPopList();

        FindObjectOfType<FadeTo>().finishedDecreasing.AddListener(PopListSetup);
        finishedPopping.AddListener(SubscribeToEvents);
    }

    // Update is called once per frame
    void Update()
    {
        if (popping)
            Pop();
    }

    void BuildPopList()
    {
        Color winnerColor = new Color(0, 0, 0, 1);

        switch (FindObjectOfType<DataTransferer>().resultsInfo.winner)
        {
            case DataController.Winner.CANI:
                popList.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>().sprite = cani;
                winnerColor = caniColor;
                break;

            case DataController.Winner.HIPSTER:
                popList.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>().sprite = hipster;
                winnerColor = hipsterColor;
                break;

            case DataController.Winner.DRAW:
                popList.transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>().sprite = null;
                winnerColor = drawColor;
                GameObject auxiliarText = Instantiate(myText);
                auxiliarText.transform.Find("Text").GetComponent<MyText>().text = FindObjectOfType<JSONHandler>().RetrieveText("#JSONResource.UIText.draw");
                auxiliarText.transform.Find("Text").GetComponent<MyText>().color = winnerColor;
                auxiliarText.transform.Find("Text").GetComponent<MyText>().anchor = MyText.Anchor.CENTERED;
                auxiliarText.transform.Find("Text").GetComponent<MyText>().layer = 12;
                auxiliarText.transform.SetParent(popList.transform.Find("Sprite"));
                auxiliarText.transform.position = auxiliarText.transform.parent.transform.position + new Vector3(0.5f, 9/32f, 0);
                break;
        }

        GameObject newText = Instantiate(myText);
        newText.transform.Find("Text").GetComponent<MyText>().color = winnerColor;
        newText.transform.Find("Text").GetComponent<MyText>().anchor = MyText.Anchor.CENTERED;
        newText.transform.Find("Text").GetComponent<MyText>().layer = 12;
        newText.transform.SetParent(popList.transform.Find("Wincon"));
        newText.transform.position = newText.transform.parent.transform.position;

        switch (FindObjectOfType<DataTransferer>().resultsInfo.winCondition)
        {
            case DataController.WinCondition.DOMINATION: 
                if (FindObjectOfType<DataTransferer>().resultsInfo.winner != DataController.Winner.DRAW)
                    newText.transform.Find("Text").GetComponent<MyText>().text = FindObjectOfType<JSONHandler>().RetrieveText("#JSONResource.UIText.domination");
                else
                    newText.transform.Find("Text").GetComponent<MyText>().text = FindObjectOfType<JSONHandler>().RetrieveText("#JSONResource.UIText.draw");
                break;

            case DataController.WinCondition.EXTERMINATION:
                newText.transform.Find("Text").GetComponent<MyText>().text = FindObjectOfType<JSONHandler>().RetrieveText("#JSONResource.UIText.extermination");
                break;

            case DataController.WinCondition.OCUPATION:
                newText.transform.Find("Text").GetComponent<MyText>().text = FindObjectOfType<JSONHandler>().RetrieveText("#JSONResource.UIText.ocupation");
                break;
        }

        popList.transform.Find("Number").gameObject.GetComponent<Number>().CreateNumber(FindObjectOfType<DataTransferer>().resultsInfo.turns);

        GameObject newText2 = Instantiate(myText);
        newText2.transform.Find("Text").GetComponent<MyText>().text = FindObjectOfType<JSONHandler>().RetrieveText("#JSONResource.UIText." + FindObjectOfType<DataTransferer>().map.name);
        newText2.transform.Find("Text").GetComponent<MyText>().color = new Color(0, 0, 0, 1);
        newText2.transform.Find("Text").GetComponent<MyText>().anchor = MyText.Anchor.CENTERED;
        newText2.transform.Find("Text").GetComponent<MyText>().layer = 12;
        newText2.transform.SetParent(popList.transform.Find("MapName"));
        newText2.transform.position = newText2.transform.parent.transform.position;

        SetMinimapPosition(FindObjectOfType<DataTransferer>().minimap);

        foreach (GameObject item in toPop)
        {
            item.SetActive(false);
        }
    }

    void SetMinimapPosition(GameObject minimap)
    {
        minimap.SetActive(true);

        minimap.transform.SetParent(popList.transform.Find("Minimap"));

        Tilemap tilemap = minimap.transform.Find("Tilemap").GetComponent<Tilemap>();
        minimap.transform.position = minimap.transform.parent.position - new Vector3(3 / 16f, 0, 0);
        minimap.transform.position += new Vector3(-(tilemap.size.x / 2f) * (3 / 16f), (tilemap.size.y / 2f) * (3 / 16f), 0);

        if (minimap.transform.position.x % 0.0625f == 0) //això és per corregir un bug visual que només passa si la posició del minimapa coincideix al pixel (té a veure amb el fet que les tiles del minimapa son de 3x3, em fa l'efecte)
            minimap.transform.position += new Vector3(-1 / 32f, 0, 0);
    }

    void PopListSetup()
    {
        FindObjectOfType<FadeTo>().finishedDecreasing.RemoveListener(PopListSetup);
        FindObjectOfType<SoundController>().PlayResults();

        popping = true;
    }

    void Pop()
    {
        timer += Time.deltaTime;

        if (timer >= popTime)
        {
            timer = 0.0f;

            toPop[index++].SetActive(true);
            //so

            if (index == toPop.Count)
            {
                popping = false;
                finishedPopping.Invoke();
            }

            FindObjectOfType<SoundController>().PlayUnavailable();
        }

        //finishedPopping.Invoke();
    }

    void TransitionToNextScene()
    {
        UnsibscribeFromEvents();

        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(LoadNextScene);

        FindObjectOfType<SoundController>().PlayButton();
    }

    void LoadNextScene()
    {
        FindObjectOfType<FadeTo>().finishedIncreasing.RemoveListener(LoadNextScene);
        FindObjectOfType<SoundController>().StopResults();

        Loader.Load(Loader.Scene.title);
    }

    void SubscribeToEvents()
    {
        finishedPopping.RemoveListener(SubscribeToEvents);
        FindObjectOfType<Controls>().keyboard_o_down.AddListener(TransitionToNextScene);
    }

    void UnsibscribeFromEvents()
    {
        FindObjectOfType<Controls>().keyboard_o_down.RemoveListener(TransitionToNextScene);
    }
}
