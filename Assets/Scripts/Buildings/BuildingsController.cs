using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsController : MonoBehaviour
{
    public List<GameObject> neutralBuildings;
    public List<GameObject> caniBuildings;
    public List<GameObject> hipsterBuildings;

    [HideInInspector]
    public GameObject caniBase;
    [HideInInspector]
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
}
