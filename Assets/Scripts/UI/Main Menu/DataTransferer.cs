using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTransferer : MonoBehaviour
{
    public GameObject map;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void TransferMap(GameObject map)
    {
        this.map = map;
    }
}
