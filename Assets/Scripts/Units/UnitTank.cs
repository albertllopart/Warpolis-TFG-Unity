using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTank : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Unit>().UITarget = transform.Find("Targeting").gameObject;
        GetComponent<Unit>().EnableUITarget(false);
        GetComponent<Unit>().UIDamageInfo = transform.Find("Damage_info").gameObject;
        GetComponent<Unit>().EnableUIDamageInfo(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
