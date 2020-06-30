using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueController : MonoBehaviour
{
    public GameObject myText;
    public GameObject cursor;
    Vector3 lowerPosition;
    Vector3 goal;

    //dialogues
    List<string> dialogues;
    GameObject dialogueLocation;
    GameObject cursorLocation;

    public GameObject dialoguePrefab;
    GameObject currentDialogue;

    //cursor
    public bool needToDelete = false;

    //events
    public UnityEvent finishedEntering;
    public UnityEvent finishedFading;

    //enter fade
    float time = 0.01f;
    float timer = 0.00f;
    float movement = 4 / 16f;

    //gameplay
    public GameObject gameplayController;

    public enum DialogueState
    {
        ENTERING, RESTING, FADING
    };

    DialogueState state;

    void Awake()
    {
        finishedEntering = new UnityEvent();
        finishedFading = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {
        state = DialogueState.RESTING;

        SetPosition();

        finishedEntering.AddListener(SubscribeToEvents);
        FindObjectOfType<CameraController>().fadeToWhiteEnd.AddListener(StartDialogue);

        //TestDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown("f1"))
        //    TestDialogue();

        switch (state)
        {
            case DialogueState.ENTERING:
                Enter();
                break;

            case DialogueState.RESTING:
                break;

            case DialogueState.FADING:
                Fade();
                break;
        }
    }

    void MyOnEnable(List<string> dialogues)
    {
        SetPosition();

        if (dialogues != null && dialogues.Count > 0)
        {
            InstantiateDialogue();

            this.dialogues = dialogues;
            state = DialogueState.ENTERING;

            NextDialogue();

            if (this.dialogues.Count > 0)
            {
                InstantiateCursor();
            }
            else
                needToDelete = false;

            OnEnter();
        }

        //eliminar subscripcions a events d'altres controladors
    }

    void StartDialogue()
    {
        FindObjectOfType<CameraController>().fadeToWhiteEnd.RemoveListener(StartDialogue);
        MyOnEnable(FindObjectOfType<MapInfo>().startDialoguesList);
    }

    public void EndDialogue()
    {
        MyOnEnable(FindObjectOfType<MapInfo>().endDialoguesList);
    }

    void InstantiateDialogue()
    {
        currentDialogue = Instantiate(dialoguePrefab, gameObject.transform);
        currentDialogue.transform.position = lowerPosition;

        dialogueLocation = currentDialogue.transform.Find("Dialogue").gameObject;
        cursorLocation = currentDialogue.transform.Find("Cursor").gameObject;
    }

    void TestDialogue()
    {
        string text1 = "Mira esitc provant aquí el dialeg tot guapo que estic programant per fer el tutorial. Em fa una mica de mandra però és el que em toca, ves. Estic a la recta final.";
        string text2 = "I ara estic provant si el cursor funciona bé i a més a més també vull provar si el sistema de diàleg queda com m'esperava. Ara el cursor hauria de desaparèixer perquè queda només un text.";
        string text3 = "I finalment una última prova per veure si la cosa queda ben arrodonida. Jo diria que ho he bordat, però vaja algú altre m'ho haurà de confirmar. Vaig a buscar unes galetes.";

        List<string> test = new List<string>();
        test.Add(text1);
        test.Add(text2);
        test.Add(text3);

        MyOnEnable(test);
    }

    void SetPosition()
    {
        goal = Camera.main.transform.position + new Vector3(0, -4 - 4 / 16f, 10);
        lowerPosition = goal + new Vector3(0, -4, 0);
    }

    void Enter()
    {
        if (timer >= time)
        {
            timer = 0.00f;
            currentDialogue.transform.position += new Vector3(0, movement);
        }
        else
        {
            timer += Time.deltaTime;
        }

        if (currentDialogue.transform.position == goal)
        {
            finishedEntering.Invoke();
            state = DialogueState.RESTING;
        }
    }

    void Fade()
    {
        if (timer >= time)
        {
            timer = 0.00f;
            currentDialogue.transform.position -= new Vector3(0, movement);
        }
        else
        {
            timer += Time.deltaTime;
        }

        if (currentDialogue.transform.position == lowerPosition)
        {
            finishedFading.Invoke();
            state = DialogueState.RESTING;
            DestroyCurrentDialogue();
        }
    }

    void NextDialogue()
    {
        if (dialogues != null && dialogues.Count > 0)
        {
            Destroy(dialogueLocation.transform.Find("Dialogue").gameObject);

            GameObject nextDialogue = Instantiate(myText, dialogueLocation.transform);
            nextDialogue.GetComponent<MyTextManager>().length = 18;
            nextDialogue.transform.Find("Text").GetComponent<MyText>().text = GetNextDialogue();
            nextDialogue.transform.Find("Text").GetComponent<MyText>().anchor = MyText.Anchor.LEFT;
            nextDialogue.name = "Dialogue";

            if (needToDelete && dialogues.Count == 0)
            {
                Destroy(cursorLocation.transform.Find("Cursor").gameObject);
                needToDelete = false;
            }

            FindObjectOfType<SoundController>().PlayPlayerMove();
        }
        else
        {
            state = DialogueState.FADING;
            UnsubscribeFromEvents();
        }
    }

    string GetNextDialogue()
    {
        string ret = dialogues[0];
        dialogues.Remove(ret);

        return ret;
    }

    void InstantiateCursor()
    {
        GameObject newCursor = Instantiate(cursor, cursorLocation.transform);
        newCursor.name = "Cursor";
        needToDelete = true;
    }

    void DestroyCurrentDialogue()
    {
        OnDestroy();
        Destroy(currentDialogue);
    }

    void OnEnter()
    {
        //if (gameplayController.activeSelf)
        //    FindObjectOfType<GameplayController>().MyOnDisable();
    }

    void OnDestroy()
    {

    }

    void SubscribeToEvents()
    {
        FindObjectOfType<Controls>().keyboard_o_down.AddListener(NextDialogue);
    }

    void UnsubscribeFromEvents()
    {
        FindObjectOfType<Controls>().keyboard_o_down.RemoveListener(NextDialogue);
    }
}
