using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsController : MonoBehaviour
{
    public List<GameObject> neutralBuildings;
    public List<GameObject> caniBuildings;
    public List<GameObject> hipsterBuildings;

    public GameObject caniBase;
    public GameObject hipsterBase;

    // Start is called before the first frame update
    void Start()
    {
        neutralBuildings = new List<GameObject>();
        caniBuildings = new List<GameObject>();
        hipsterBuildings = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DestroyAllBuildings()
    {
        foreach (GameObject building in neutralBuildings)
        {
            Destroy(building);
        }

        foreach (GameObject building in caniBuildings)
        {
            Destroy(building);
        }

        foreach (GameObject building in hipsterBuildings)
        {
            Destroy(building);
        }

        ResetLists();
    }

    void ResetLists()
    {
        neutralBuildings = new List<GameObject>();
        caniBuildings = new List<GameObject>();
        hipsterBuildings = new List<GameObject>();
    }

    public List<GameObject> GetUnitsOnBuildings(UnitArmy army)
    {
        List<GameObject> ret = new List<GameObject>();

        switch (army)
        {
            case UnitArmy.CANI:

                foreach (GameObject building in caniBuildings)
                {
                    GameObject toAdd = building.GetComponent<Building>().CheckUnit();

                    if (toAdd != null && toAdd.GetComponent<Unit>().hitPoints < 50)
                    {
                        ret.Add(toAdd);
                    }
                }

                break;

            case UnitArmy.HIPSTER:

                foreach (GameObject building in hipsterBuildings)
                {
                    GameObject toAdd = building.GetComponent<Building>().CheckUnit();

                    if (toAdd != null && toAdd.GetComponent<Unit>().hitPoints < 50)
                    {
                        ret.Add(toAdd);
                    }
                }

                break;
        }

        return ret;
    }
}
