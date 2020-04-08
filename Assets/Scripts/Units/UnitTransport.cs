using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTransport : MonoBehaviour
{
    [Header("Load")]
    GameObject UILoadSign;
    public GameObject loadedUnit;

    // Start is called before the first frame update
    void Start()
    {
        UILoadSign = transform.Find("Load").gameObject;
        UILoadSign.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableLoadSign(bool enable)
    {
        UILoadSign.SetActive(enable);
    }
}
