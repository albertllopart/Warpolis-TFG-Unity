using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONHandler : MonoBehaviour
{
    string unitDatafilename = "unitData.json";
    string path;

    public UnitDataCollection unitDataCollection = new UnitDataCollection();

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        path = Application.persistentDataPath + "/Resources/";

        Debug.Log("JSONHandler::Start - dataPath = " + path);

        ReadUnitData();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void SaveData()
    {
        JSONWrapper wrapper = new JSONWrapper(); //serveix per tenir el nom de la classe al json i la classe entre claudàtors
        wrapper.unitDataCollection = unitDataCollection;

        string contents = JsonUtility.ToJson(wrapper, true); //el true li posa un format llegible a l'arxiu
        System.IO.File.WriteAllText(path + "testing.json", contents);
    }

    void ReadUnitData()
    {
        try
        {
            if (System.IO.File.Exists(path + unitDatafilename))
            {
                string contents = System.IO.File.ReadAllText(path + unitDatafilename);
                JSONWrapper wrapper = JsonUtility.FromJson<JSONWrapper>(contents);
                unitDataCollection = wrapper.unitDataCollection;

                foreach (UnitData unitData in unitDataCollection.unitDataList)
                {
                    Debug.Log(unitData.name);
                }
            }
            else //build
            {
                TextAsset textAsset = Resources.Load("unitData") as TextAsset; //-> Assets/Resources/fitxer(sense extensió)
                if (textAsset != null)
                {
                    string contents = textAsset.text;
                    JSONWrapper wrapper = JsonUtility.FromJson<JSONWrapper>(contents);
                    unitDataCollection = wrapper.unitDataCollection;
                }
                else
                {
                    Application.Quit();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        Debug.Log("JSONHandler::ReadUnitData - UnitData loaded: " + unitDataCollection.unitDataList.Count);
    }

    public string RetrieveText(string resourceCode)
    {
        //estructura del resourceCode - #JSONResource.Class.Index(si és una llista).Field

        if (resourceCode == null)
            return "resourceCode format error";

        string ret = " ";

        string[] codeSegments = resourceCode.Split('.'); //separar el codi del recurs entre .

        if (codeSegments[1] != null)
        {
            if (codeSegments[1] == "unitData")
            {
                ret = RetrieveTextFromUnitData(codeSegments);
            }
            else
            {
                ret = "Unknown Resource";
            }
        }
        else
        {
            ret = "resourceCode format error";
        }

        return ret;
    }

    string RetrieveTextFromUnitData(string[] codeSegments)
    {
        int index = 0;
        string ret = " ";

        if (codeSegments[2] != null) //filtrar tipus d'unitat
        {
            if (codeSegments[2] == "infantry")
            {
                index = 0;
            }
            else if (codeSegments[2] == "transport")
            {
                index = 1;
            }
            else if (codeSegments[2] == "tank")
            {
                index = 2;
            }
            else if (codeSegments[2] == "aerial")
            {
                index = 3;
            }
            else if (codeSegments[2] == "gunner")
            {
                index = 4;
            }
            else if (codeSegments[2] == "ranged")
            {
                index = 5;
            }
        }
        else
        {
            Debug.LogError("JSONHandler::RetrieveTextFromUnitData - resourceCode format error");
            return "resourceCode format error";
        }

        if (codeSegments[3] != null) //filtrar element
        {
            if (codeSegments[3] == "description")
            {
                ret = RetrieveUnitDataDescription(index);
            }
        }
        else
        {
            Debug.LogError("JSONHandler::RetrieveTextFromUnitData - resourceCode format error");
            return "resourceCode format error";
        }

        return ret;
    }

    string RetrieveUnitDataDescription(int index)
    {
        if (unitDataCollection.unitDataList[index] != null)
            return unitDataCollection.unitDataList[index].description;
        else
        {
            Debug.LogError("JSONHandler::RetrieveUnitDataDescription - unitDataList[" + index + "] is null");
            return "unitDataList[" + index + "] is null";
        }
    }
}

//Apunts importants sobre llegir json o qualsevol format (xml?):
//El path de l'aplicació només serveix per quan s'està fent servir l'editor. Lògicament quan hi ha una build la seva estructura no té res a veure amb la del projecte
//de manera que s'ha de fer servir Resources.Load(). Tots els arxius que estiguin guardats a Assets/Resources podran ser carregats d'aquesta manera
