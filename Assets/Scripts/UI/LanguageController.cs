using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageController : MonoBehaviour
{
    [Header("Cursor")]
    public GameObject cursor;

    [Header("Languages")]
    public GameObject catala;
    public GameObject english;
    public GameObject castellano;

    List<GameObject> languages;

    Language language;

    int index = 1;

    bool afterStart = false;

    void Awake()
    {
        languages = new List<GameObject>();
        languages.Add(catala);
        languages.Add(english);
        languages.Add(castellano);

        language = Language.ENGLISH;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    void AfterStart()
    {
        afterStart = true;
        SubscribeToEvents();
    }

    // Update is called once per frame
    void Update()
    {
        if (!afterStart)
            AfterStart();
    }

    void Up()
    {
        index--;
        if (index < 0)
            index = languages.Count - 1;

        UpdateCursor();

        FindObjectOfType<SoundController>().PlayPlayerMove();
    }

    void Down()
    {
        index++;
        if (index >= languages.Count)
            index = 0;

        UpdateCursor();

        FindObjectOfType<SoundController>().PlayPlayerMove();
    }

    void Confirm()
    {
        UnsbuscribeFromEvents();

        if (languages[index].name == "English")
            language = Language.ENGLISH;
        else if (languages[index].name == "Catala")
            language = Language.CATALA;
        else if (languages[index].name == "Castellano")
            language = Language.CASTELLANO;

        FindObjectOfType<DataTransferer>().TransferLanguage(language);

        Loader.EarlyLoad(Loader.Scene.intro);

        FindObjectOfType<SoundController>().PlayButton();
    }

    void UpdateCursor()
    {
        cursor.transform.position = new Vector3(0, 0, 0) + new Vector3(0, languages[index].transform.position.y, 0);
    }

    void SubscribeToEvents()
    {
        FindObjectOfType<Controls>().keyboard_w_down.AddListener(Up);
        FindObjectOfType<Controls>().keyboard_s_down.AddListener(Down);

        FindObjectOfType<Controls>().keyboard_o_down.AddListener(Confirm);
    }

    void UnsbuscribeFromEvents()
    {
        FindObjectOfType<Controls>().keyboard_w_down.RemoveListener(Up);
        FindObjectOfType<Controls>().keyboard_s_down.RemoveListener(Down);

        FindObjectOfType<Controls>().keyboard_o_down.RemoveListener(Confirm);
    }
}
