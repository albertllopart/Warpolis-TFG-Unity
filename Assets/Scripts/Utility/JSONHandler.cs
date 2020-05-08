using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONHandler : MonoBehaviour
{
    string unitDatafilename = "unitsData.json";
    string path;

    public UnitDataCollection unitDataCollection = new UnitDataCollection();

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);

        path = Application.persistentDataPath + "/json/";
        
        Debug.Log("JSONHandler::Start - dataPath = " + path);

        ReadUnitData();
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
                TextAsset textAsset = Resources.Load("unitsData") as TextAsset; //-> Assets/Resources/fitxer(sense extensió)
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
}

//Apunts importants sobre llegir json o qualsevol format (xml?):
//El path de l'aplicació només serveix per quan s'està fent servir l'editor. Lògicament quan hi ha una build la seva estructura no té res a veure amb la del projecte
//de manera que s'ha de fer servir Resources.Load(). Tots els arxius que estiguin guardats a Assets/Resources podran ser carregats d'aquesta manera
