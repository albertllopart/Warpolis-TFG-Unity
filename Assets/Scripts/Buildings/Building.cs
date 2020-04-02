using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BuildingArmy
{
    NEUTRAL, CANI, HIPSTER
};

public enum BuildingType
{
    BASE, FACTORY
};

public class Building : MonoBehaviour
{
    BuildingArmy army;
    public BuildingType type;

    public int maxHP;
    public int currentHP;

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = GameObject.Find("Buildings Controller").transform;
        Setup();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Setup()
    {
        //army
        if (CompareTag("Factory_neutral"))
            SetNeutral();
        else if (CompareTag("Base_cani") || CompareTag("Factory_cani"))
            SetCani();
        else if (CompareTag("Base_hipster") || CompareTag("Factory_hipster"))
            SetHipster();

        //type
        if (CompareTag("Base_cani"))
        {
            type = BuildingType.BASE;
            GetComponentInParent<BuildingsController>().caniBase = gameObject;
        }
        else if (CompareTag("Base_hipster"))
        {
            type = BuildingType.BASE;
            GetComponentInParent<BuildingsController>().hipsterBase = gameObject;
        }
        else if (CompareTag("Factory_neutral") || CompareTag("Factory_cani") || CompareTag("Factory_hipster"))
            type = BuildingType.FACTORY;
    }

    public void MyOnDestroy()
    {
        switch (army)
        {
            case BuildingArmy.NEUTRAL:
                GetComponentInParent<BuildingsController>().neutralBuildings.Remove(gameObject);
                break;

            case BuildingArmy.CANI:
                GetComponentInParent<BuildingsController>().caniBuildings.Remove(gameObject);
                break;

            case BuildingArmy.HIPSTER:
                GetComponentInParent<BuildingsController>().hipsterBuildings.Remove(gameObject);
                break;
        }

        Destroy(gameObject);
    }

    void SetNeutral()
    {
        army = BuildingArmy.NEUTRAL;
        GetComponentInParent<BuildingsController>().neutralBuildings.Add(gameObject);
    }

    void SetCani()
    {
        army = BuildingArmy.CANI;
        GetComponentInParent<BuildingsController>().caniBuildings.Add(gameObject);
    }

    void SetHipster()
    {
        army = BuildingArmy.HIPSTER;
        GetComponentInParent<BuildingsController>().hipsterBuildings.Add(gameObject);
    }

    public void ResetCapture()
    {
        currentHP = maxHP;
    }

    public bool ApplyCapture(int capturePower)
    {
        //retorna true si l'edifici s'ha capturat

        currentHP -= capturePower;

        if (currentHP <= 0)
            return true;

        return false;
    }
}
