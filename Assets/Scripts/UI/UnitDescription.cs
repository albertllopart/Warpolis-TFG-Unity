using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDescription : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject myText;
    public int length;

    string resource = "#JSONResource.unitData.";
    UnitType unitType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BuildDescription(UnitType unitType)
    {
        this.unitType = unitType;

        string description = GetUnitDescription(); //obtenim les dades del json

        SetDescription(description);
    }

    string GetUnitDescription()
    {
        switch (unitType)
        {
            case UnitType.INFANTRY:
                resource += "infantry.";
                break;

            case UnitType.TRANSPORT:
                resource += "transport.";
                break;

            case UnitType.TANK:
                resource += "tank.";
                break;

            case UnitType.AERIAL:
                resource += "aerial.";
                break;

            case UnitType.GUNNER:
                resource += "gunner.";
                break;

            case UnitType.RANGED:
                resource += "ranged.";
                break;
        }

        resource += "description";

        return FindObjectOfType<JSONHandler>().RetrieveText(resource);
    }

    void SetDescription(string description)
    {
        GameObject newText = Instantiate(myText);
        newText.GetComponent<MyTextManager>().length = length;
        newText.transform.SetParent(transform.Find("MyText").transform);
        newText.transform.position = newText.transform.parent.transform.position;
        newText.transform.Find("Text").GetComponent<MyText>().text = description;
    }
}
