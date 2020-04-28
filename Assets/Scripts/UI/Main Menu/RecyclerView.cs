using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecyclerView : MonoBehaviour
{
    public GameObject recyclerButtonPrefab;
    public float spacing; //espai entre el punt de pivot d'un botó i el punt de pivot del següent de la llista
    public float margin;
    public int viewSize; //quantitat de botons que es mostraran a la view

    [HideInInspector]
    public List<GameObject> buttons;
    [HideInInspector]
    GameObject selectedButtonGO;
    int counter = 0;

    public int upperButton;
    public int lowerButton;
    public int selectedButton;

    // Start is called before the first frame update
    void Start()
    {
        buttons = new List<GameObject>();

        upperButton = 1;
        selectedButton = 1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MyOnEnable()
    {
        UpdateLimitArrows();
    }

    public GameObject InstantiateButton(string name)
    {
        GameObject newButton = Instantiate(recyclerButtonPrefab, transform.position + new Vector3(0, -(4 / 16f + margin + spacing * counter++), 0), Quaternion.identity); //els magic numbers són per centrar els botons al background
        newButton.name = name;
        newButton.transform.SetParent(transform);
        newButton.transform.Find("MyText").GetComponent<MyTextManager>().SetNewText(name, new Color(0, 0, 0, 1), MyText.Anchor.LEFT);

        buttons.Add(newButton);

        if (counter > viewSize)
            newButton.SetActive(false);

        UpdateButtonTracker();

        if (buttons.Count == 1)
            SelectButton(newButton);

        return newButton;
    }

    void SelectButton(GameObject button)
    {
        if (selectedButtonGO != null)
            DeselectButton();

        selectedButtonGO = button;
        selectedButtonGO.GetComponent<MyButton>().OnHighlight();
    }

    void DeselectButton()
    {
        selectedButtonGO.GetComponent<MyButton>().OnIdle();
    }

    void UpdateButtonTracker()
    {
        if (buttons.Count <= viewSize)
            lowerButton = buttons.Count;
        else
            lowerButton = viewSize;
    }

    public bool MoveDown()
    {
        if (selectedButton == lowerButton)
        {
            if (lowerButton < buttons.Count)
            {
                AfterDown();

                foreach (GameObject button in buttons)
                {
                    button.transform.position += new Vector3(0, spacing, 0);
                }

                return true;
            }
        }
        else
        {
            SelectButton(buttons[selectedButton++]);
            return true;
        }

        return false;
    }

    void AfterDown()
    {
        buttons[upperButton - 1].SetActive(false);
        upperButton++;
        buttons[lowerButton].SetActive(true);
        lowerButton++;

        SelectButton(buttons[selectedButton++]);

        UpdateLimitArrows();
    }

    public bool MoveUp()
    {
        if (selectedButton == upperButton)
        {
            if (upperButton > 1)
            {
                AfterUp();

                foreach (GameObject button in buttons)
                {
                    button.transform.position -= new Vector3(0, spacing, 0);
                }

                return true;
            }
        }
        else
        {
            SelectButton(buttons[--selectedButton - 1]);
            return true;
        }

        return false;
    }

    void AfterUp()
    {
        buttons[--lowerButton].SetActive(false);
        buttons[--upperButton - 1].SetActive(true);

        SelectButton(buttons[--selectedButton - 1]);

        UpdateLimitArrows();
    }

    void UpdateLimitArrows()
    {
        if (upperButton > 1)
            transform.Find("ArrowUp").gameObject.SetActive(true);
        else
            transform.Find("ArrowUp").gameObject.SetActive(false);

        if (buttons.Count > lowerButton)
            transform.Find("ArrowDown").gameObject.SetActive(true);
        else
            transform.Find("ArrowDown").gameObject.SetActive(false);
    }

    public int GetSelectedButtonIndex()
    {
        return buttons.IndexOf(selectedButtonGO);
    }
}
