using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsController : MonoBehaviour
{
    public List<GameObject> neutralBuildings;
    public List<GameObject> caniBuildings;
    public List<GameObject> hipsterBuildings;

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
}
