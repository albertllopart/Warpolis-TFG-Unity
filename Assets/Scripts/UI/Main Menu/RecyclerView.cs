using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecyclerView : MonoBehaviour
{
    public GameObject recyclerButtonPrefab;
    public float spacing; //espai entre el punt de pivot d'un botó i el punt de pivot del següent de la llista
    public int viewSize; //quantitat de botons que es mostraran a la view

    List<GameObject> buttons;
    int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        buttons = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InstantiateButton(string name)
    {
        GameObject newButton = Instantiate(recyclerButtonPrefab, transform.position - new Vector3(0, spacing * counter++, 0), Quaternion.identity);
        newButton.name = name;
        newButton.transform.SetParent(transform);
        newButton.transform.Find("MyText").GetComponent<MyTextManager>().SetNewText(name, new Color(0, 0, 0, 1), MyText.Anchor.LEFT);

        buttons.Add(newButton);

        if (counter > viewSize)
            newButton.SetActive(false);
    }
}
