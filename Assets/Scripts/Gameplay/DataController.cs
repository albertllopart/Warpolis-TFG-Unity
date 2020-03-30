using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoBehaviour
{
    public int caniMoney = 0;
    public int hipsterMoney = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddCaniMoney(int amount)
    {
        caniMoney += amount;
        Camera.main.transform.Find("UI Controller").transform.Find("Money_info").GetComponent<MoneyInfo>().UpdateMoney((uint)caniMoney);
    }

    public void AddHipsterMoney(int amount)
    {
        hipsterMoney += amount;
        Camera.main.transform.Find("UI Controller").transform.Find("Money_info").GetComponent<MoneyInfo>().UpdateMoney((uint)hipsterMoney);
    }
}
