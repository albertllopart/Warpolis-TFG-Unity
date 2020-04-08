using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfantry : MonoBehaviour
{
    [Header("Capture")]
    public GameObject currentCapture;
    public GameObject lastCapture;
    public bool toStopCapture;
    GameObject UICaptureSign;

    //instantiate
    [Header("Instances")]
    public GameObject factory;
    public GameObject building;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Unit>().UITarget = transform.Find("Targeting").gameObject;
        GetComponent<Unit>().EnableUITarget(false);
        GetComponent<Unit>().UIDamageInfo = transform.Find("Damage_info").gameObject;
        GetComponent<Unit>().EnableUIDamageInfo(false);

        UICaptureSign = transform.Find("Capture").gameObject;
        UICaptureSign.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (toStopCapture)
        {
            StopCapture();
        }
    }

    public void StopCapture()
    {
        if (currentCapture != null)
        {
            currentCapture.GetComponent<Building>().currentHP = currentCapture.GetComponent<Building>().maxHP;
            currentCapture = null;
        }

        EnableUICaptureSign(false);

        toStopCapture = false;
    }

    public void OnCapture()
    {
        //comprovem que l'edifici que volem capturar és el mateix que abans, en cas contrari resetegem l'anterior
        if (currentCapture != null && currentCapture != SearchForOtherBuilding())
            StopCapture();

        //obtenim edifici que volem capturar
        if (currentCapture == null)
            currentCapture = SearchForOtherBuilding();

        if (currentCapture != null)
        {
            EnableUICaptureSign(true);
            int capturePower = (int)GetComponent<Unit>().CalculateUIHitpoints();

            //restar vida al building i mirar si s'ha capturat
            if(currentCapture.GetComponent<Building>().ApplyCapture(capturePower))
            {
                GetComponent<Unit>().OnWait();

                if (currentCapture.name.Contains("Base"))
                {
                    GetComponent<Unit>().OnWinCon();
                    GameObject.Find("Data Controller").GetComponent<DataController>().baseCaptured.Invoke();
                }

                DestroyAndCreate();
                currentCapture = null;

                return; // això és perquè un cop ha acabat la captura no segueixi en estat de captura
            }
        }

        GetComponent<Unit>().OnWaitWithCapture();
    }

    public void EnableUICaptureSign(bool enable)
    {
        UICaptureSign.SetActive(enable);
    }

    void DestroyAndCreate()
    {
        if (currentCapture.GetComponent<Building>().type == BuildingType.BASE)
        {
            Instantiate(building, transform.position, Quaternion.identity);
        }
        else if (currentCapture.GetComponent<Building>().type == BuildingType.FACTORY)
        {
            Instantiate(factory, transform.position, Quaternion.identity);
        }
        else if (currentCapture.GetComponent<Building>().type == BuildingType.BUILDING)
        {
            Instantiate(building, transform.position, Quaternion.identity);
        }

        currentCapture.GetComponent<Building>().MyOnDestroy();
    }

    public GameObject SearchForOtherBuilding()
    {
        //aquest mètode retorna el building enemic o neutral que hi ha a la casella de la unitat

        Vector2 from = transform.position; from += new Vector2(0.5f, -0.5f); //establim el punt de partida al centre de la casella
        Vector2 to = from;

        RaycastHit2D neutral = Physics2D.Linecast(from, to, LayerMask.GetMask("Neutral_buildings")); ;

        if (neutral.collider != null)
        {
            return neutral.collider.gameObject;
        }
        else
        {
            if (GetComponent<Unit>().army == UnitArmy.CANI)
            {
                RaycastHit2D hipster = Physics2D.Linecast(from, to, LayerMask.GetMask("Hipster_buildings"));
                if (hipster.collider != null)
                {
                    return hipster.collider.gameObject;
                }
            }
            else if (GetComponent<Unit>().army == UnitArmy.HIPSTER)
            {
                RaycastHit2D cani = Physics2D.Linecast(from, to, LayerMask.GetMask("Cani_buildings"));
                if (cani.collider != null)
                {
                    return cani.collider.gameObject;
                }
            }
        }

        return null;
    }

    public GameObject SearchForTransport()
    {
        //aquest mètode retorna la unitat de transport que hi ha a la casella de la unitat

        Vector2 from = transform.position; from += new Vector2(0.5f, -0.5f); //establim el punt de partida al centre de la casella
        Vector2 to = from;

        GetComponent<Unit>().EnableOwnCollider(false); //desactivem el propi collider per mirar què hi ha sota

        if (GetComponent<Unit>().army == UnitArmy.CANI)
        {
            RaycastHit2D cani = Physics2D.Linecast(from, to, LayerMask.GetMask("Cani_units"));
            if (cani.collider != null && cani.collider.gameObject.GetComponent<Unit>().unitType == (uint)UnitType.TRANSPORT)
            {
                GetComponent<Unit>().EnableOwnCollider(true);
                return cani.collider.gameObject;
            }
        }
        else if (GetComponent<Unit>().army == UnitArmy.HIPSTER)
        {
            RaycastHit2D hipster = Physics2D.Linecast(from, to, LayerMask.GetMask("Hipster_units"));
            if (hipster.collider != null && hipster.collider.gameObject.GetComponent<Unit>().unitType == (uint)UnitType.TRANSPORT)
            {
                GetComponent<Unit>().EnableOwnCollider(true);
                return hipster.collider.gameObject;
            }
        }

        GetComponent<Unit>().EnableOwnCollider(true);
        return null;
    }

    public void OnLoad()
    {
        GameObject transport = SearchForTransport();

        transport.GetComponent<UnitTransport>().loadedUnit = gameObject;
        transport.GetComponent<UnitTransport>().EnableLoadSign(true);

        StopCapture();
        GetComponent<Unit>().UpdateTileInfo();
        GetComponent<Unit>().UnsubscribeFromEvents();

        gameObject.SetActive(false);
        GameObject.Find("Gameplay Controller").GetComponent<GameplayController>().DisableMenuUnit();
    }
}
