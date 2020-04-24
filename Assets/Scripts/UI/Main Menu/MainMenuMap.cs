using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMap : MonoBehaviour
{
    GameObject recyclerView;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AfterStart()
    {
        recyclerView = transform.Find("RecyclerView").gameObject;

        for (int i = 0; i < 8; i++)
        {
            recyclerView.GetComponent<RecyclerView>().InstantiateButton("Testing " + i);
        }

        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
