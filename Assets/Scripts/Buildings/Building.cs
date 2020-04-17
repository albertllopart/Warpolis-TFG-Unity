using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum BuildingArmy
{
    NEUTRAL, CANI, HIPSTER
};

public enum BuildingType
{
    BASE, FACTORY, BUILDING
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
        if (CompareTag("Factory_neutral") || CompareTag("Building_neutral"))
            SetNeutral();
        else if (CompareTag("Base_cani") || CompareTag("Factory_cani") || CompareTag("Building_cani"))
            SetCani();
        else if (CompareTag("Base_hipster") || CompareTag("Factory_hipster") || CompareTag("Building_hipster"))
            SetHipster();

        //type
        if (CompareTag("Base_cani"))
        {
            type = BuildingType.BASE;
        }
        else if (CompareTag("Base_hipster"))
        {
            type = BuildingType.BASE;
        }
        else if (CompareTag("Factory_neutral") || CompareTag("Factory_cani") || CompareTag("Factory_hipster"))
            type = BuildingType.FACTORY;
        else if (CompareTag("Building_neutral") || CompareTag("Building_cani") || CompareTag("Building_hipster"))
            type = BuildingType.BUILDING;
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

    public GameObject CheckUnit()
    {
        GameObject ret = null;

        Vector2 from = transform.position; from += new Vector2(0.5f, -0.5f);
        Vector2 to = from;
        int layer = 0;
        RaycastHit2D result;

        switch (army)
        {
            case BuildingArmy.CANI:

                layer = LayerMask.GetMask("Cani_units");
                result = Physics2D.Linecast(from, to, layer);

                if (result.collider != null)
                    ret = result.collider.gameObject;

                break;

            case BuildingArmy.HIPSTER:
                
                layer = LayerMask.GetMask("Hipster_units");
                result = Physics2D.Linecast(from, to, layer);

                if (result.collider != null)
                    ret = result.collider.gameObject;

                break;
        }

        return ret;
    }
}
