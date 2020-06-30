using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainMenuTutorial : MonoBehaviour
{
    [Header("Tutorial Maps")]
    public GameObject tutorialMap1;
    public GameObject tutorialMap2;
    public GameObject tutorialMap3;
    public GameObject tutorialMap4;
    public GameObject tutorialMap5;
    public GameObject tutorialMap6;
    public GameObject tutorialMap7;
    public GameObject tutorialMap8;

    List<GameObject> mapList;

    GameObject recyclerView;
    GameObject mapInfo;
    GameObject selectedMap;

    public GameObject myText;

    // Start is called before the first frame update
    void Start()
    {
        mapList = new List<GameObject>();
    }

    public void AfterStart()
    {
        BuildMapList();

        recyclerView = transform.Find("RecyclerView").gameObject;
        mapInfo = transform.Find("MapInfo").gameObject;

        foreach (GameObject map in mapList)
        {
            string mapName = GetMapName(map);
            GameObject button = recyclerView.GetComponent<RecyclerView>().InstantiateButton(mapName);
        }

        UpdateMapInfo(0);

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    string GetMapName(GameObject map)
    {
        string resource = "JSONResource.UIText." + map.name;
        return FindObjectOfType<JSONHandler>().RetrieveText(resource);
    }

    void BuildMapList()
    {
        mapList.Add(tutorialMap1);
        mapList.Add(tutorialMap2);
        mapList.Add(tutorialMap3);
        mapList.Add(tutorialMap4);
        mapList.Add(tutorialMap5);
        mapList.Add(tutorialMap6);
        mapList.Add(tutorialMap7);
        mapList.Add(tutorialMap8);
    }

    public void MyOnEnable()
    {
        FindObjectOfType<FadeTo>().finishedDecreasing.AddListener(SubscribeToEvents);
        recyclerView.GetComponent<RecyclerView>().MyOnEnable();
    }

    public void MyOnDisable()
    {
        gameObject.SetActive(false);
    }

    public void TransitionToMode()
    {
        GetComponentInParent<MainMenuController>().UnsubscribeFromEvents();
        UnsubscribeFromEvents();
        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(SwitchToMode);

        FindObjectOfType<SoundController>().PlayBack();
    }

    void SwitchToMode()
    {
        FindObjectOfType<FadeTo>().finishedIncreasing.RemoveListener(SwitchToMode);
        GetComponentInParent<MainMenuController>().EnableMode();
        FindObjectOfType<FadeTo>().FadeFromSetup();
        MyOnDisable();
    }

    public void TransitionToNextScene()
    {
        GetComponentInParent<MainMenuController>().UnsubscribeFromEvents();
        UnsubscribeFromEvents();
        FindObjectOfType<FadeTo>().FadeToSetup();
        FindObjectOfType<FadeTo>().finishedIncreasing.AddListener(SwitchToNextScene);

        FindObjectOfType<SoundController>().PlayButton();
    }

    public void SwitchToNextScene()
    {
        FindObjectOfType<DataTransferer>().TransferMap(selectedMap);
        FindObjectOfType<DataTransferer>().TransferCommanders(DataController.PlayerCommander.HUMAN, DataController.PlayerCommander.COMPUTER);
        FindObjectOfType<DataTransferer>().TransferAllowedUnits(selectedMap.GetComponent<MapInfo>().BuildAllowedUnitList());
        FindObjectOfType<DataTransferer>().TransferIsTutorial(true);

        FindObjectOfType<SoundController>().StopTitle();
        FindObjectOfType<FadeTo>().finishedIncreasing.RemoveListener(SwitchToNextScene);
        Loader.Load(Loader.Scene.game);
    }

    public void MoveUp()
    {
        if (recyclerView.GetComponent<RecyclerView>().MoveUp())
        {
            int index = recyclerView.GetComponent<RecyclerView>().GetSelectedButtonIndex();

            UpdateMapInfo(index);

            FindObjectOfType<SoundController>().PlayPlayerMove();
        }
    }

    public void MoveDown()
    {
        if (recyclerView.GetComponent<RecyclerView>().MoveDown())
        {
            int index = recyclerView.GetComponent<RecyclerView>().GetSelectedButtonIndex();

            UpdateMapInfo(index);

            FindObjectOfType<SoundController>().PlayPlayerMove();
        }
    }

    void UpdateMapInfo(int index)
    {
        selectedMap = mapList[index];

        Destroy(mapInfo.transform.Find("Description").gameObject);

        GameObject description = Instantiate(myText, mapInfo.transform);
        description.GetComponent<MyTextManager>().length = 8;
        description.transform.Find("Text").GetComponent<MyText>().text = FindObjectOfType<JSONHandler>().RetrieveText("#JSONResource.UIText." + mapList[index].name + "Description");
        description.transform.Find("Text").GetComponent<MyText>().anchor = MyText.Anchor.LEFT;
        description.name = "Description";

        mapInfo.transform.Find("Icon").GetComponent<SpriteRenderer>().sprite = mapList[index].GetComponent<MapInfo>().sprite;
    }

    void SubscribeToEvents()
    {
        //el primer que hem de fer és treure el listener del fadeTo
        FindObjectOfType<FadeTo>().finishedDecreasing.RemoveListener(SubscribeToEvents);

        FindObjectOfType<Controls>().keyboard_w_down.AddListener(MoveUp);
        FindObjectOfType<Controls>().keyboard_s_down.AddListener(MoveDown);
    }

    void UnsubscribeFromEvents()
    {
        FindObjectOfType<Controls>().keyboard_w_down.RemoveListener(MoveUp);
        FindObjectOfType<Controls>().keyboard_s_down.RemoveListener(MoveDown);
    }
}
