﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSONHandler : MonoBehaviour
{
    public string unitDatafilename = "unitData";
    public string UITextfilename = "UIText";
    public string dialoguesfilename = "dialogues";
    string extension = ".bytes";
    string path;

    public UnitDataCollection unitDataCollection = new UnitDataCollection();
    public UITextCollection UITextCollection = new UITextCollection();
    public DialoguesCollection dialoguesCollection = new DialoguesCollection();

    void Awake()
    {
        SetLanguage();

        DontDestroyOnLoad(gameObject);

        path = Application.persistentDataPath + "/Resources/";

        Debug.Log("JSONHandler::Start - dataPath = " + path);

        ReadUnitData();
        ReadUIText();
        ReadDialogues();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void SetLanguage()
    {
        switch (FindObjectOfType<DataTransferer>().language)
        {
            case Language.ENGLISH:
                SetEnglish();
                break;

            case Language.CATALA:
                SetCatala();
                break;

            case Language.CASTELLANO:
                SetCastellano();
                break;
        }
    }

    void SetEnglish()
    {
        
    }

    void SetCatala()
    {
        unitDatafilename += "_Catala";
        dialoguesfilename += "_Catala";
    }

    void SetCastellano()
    {
        unitDatafilename += "_Castellano";
        dialoguesfilename += "_Castellano";
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
                TextAsset textAsset = Resources.Load<TextAsset>(path + unitDatafilename + extension);
                string contents = System.Text.Encoding.Default.GetString(textAsset.bytes);

                JSONWrapper wrapper = JsonUtility.FromJson<JSONWrapper>(contents);
                unitDataCollection = wrapper.unitDataCollection;

                foreach (UnitData unitData in unitDataCollection.unitDataList)
                {
                    Debug.Log(unitData.name);
                }
            }
            else //build
            {
                TextAsset textAsset = Resources.Load<TextAsset>(unitDatafilename); //-> Assets/Resources/fitxer(sense extensió)
                if (textAsset != null)
                {
                    string contents = System.Text.Encoding.Default.GetString(textAsset.bytes);
                    JSONWrapper wrapper = JsonUtility.FromJson<JSONWrapper>(contents);
                    unitDataCollection = wrapper.unitDataCollection;
                }
                else
                {
                    Application.Quit();
                }

                Debug.Log("JSONHandler::ReadUnitData - Read with Build Method");
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        Debug.Log("JSONHandler::ReadUnitData - UnitData loaded: " + unitDataCollection.unitDataList.Count);
    }

    void ReadUIText()
    {
        try
        {
            if (System.IO.File.Exists(path + UITextfilename))
            {
                TextAsset textAsset = Resources.Load<TextAsset>(path + UITextfilename + extension);
                string contents = System.Text.Encoding.Default.GetString(textAsset.bytes);

                JSONWrapper wrapper = JsonUtility.FromJson<JSONWrapper>(contents);
                UITextCollection = wrapper.UITextCollection;

                foreach (UIText uiText in UITextCollection.UITextList)
                {
                    Debug.Log(uiText.language);
                }
            }
            else //build
            {
                TextAsset textAsset = Resources.Load<TextAsset>(UITextfilename); //-> Assets/Resources/fitxer(sense extensió)
                if (textAsset != null)
                {
                    string contents = System.Text.Encoding.Default.GetString(textAsset.bytes);
                    JSONWrapper wrapper = JsonUtility.FromJson<JSONWrapper>(contents);
                    UITextCollection = wrapper.UITextCollection;
                }
                else
                {
                    Application.Quit();
                }

                Debug.Log("JSONHandler::ReadUIText - Read with Build Method");
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        Debug.Log("JSONHandler::ReadUIText - UIText loaded: " + UITextCollection.UITextList.Count + " Languages");
    }

    void ReadDialogues()
    {
        try
        {
            if (System.IO.File.Exists(path + dialoguesfilename))
            {
                TextAsset textAsset = Resources.Load<TextAsset>(path + dialoguesfilename + extension);
                string contents = System.Text.Encoding.Default.GetString(textAsset.bytes);

                JSONWrapper wrapper = JsonUtility.FromJson<JSONWrapper>(contents);
                dialoguesCollection = wrapper.dialoguesCollection;

                foreach (Dialogues dialogues in dialoguesCollection.dialoguesList)
                {
                    Debug.Log(dialogues.mapName);
                }
            }
            else //build
            {
                TextAsset textAsset = Resources.Load<TextAsset>(dialoguesfilename); //-> Assets/Resources/fitxer(sense extensió)
                if (textAsset != null)
                {
                    string contents = System.Text.Encoding.Default.GetString(textAsset.bytes);
                    JSONWrapper wrapper = JsonUtility.FromJson<JSONWrapper>(contents);
                    dialoguesCollection = wrapper.dialoguesCollection;
                }
                else
                {
                    Application.Quit();
                }

                Debug.Log("JSONHandler::ReadDialogues - Read with Build Method");
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        Debug.Log("JSONHandler::ReadDialogues - Dialogues loaded: " + dialoguesCollection.dialoguesList.Count + " Maps");
    }

    public string RetrieveText(string resourceCode)
    {
        //estructura del resourceCode per unitData - #JSONResource.unitData.Index(nom d'unitat).Field
        //estructura del resourceCode per UIText - #JSONResource.UIText.Field
        //estructura del resourceCode per dialogues - #JSONResource.dialogues.Index(nom del mapa).Field

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
            else if (codeSegments[1] == "UIText")
            {
                ret = RetrieveTextFromUIText(codeSegments);
            }
            else if (codeSegments[1] == "dialogues")
            {
                ret = RetrieveTextFromDialogues(codeSegments);
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
            else if (codeSegments[3] == "name")
            {
                ret = RetrieveUnitDataName(index);
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

    string RetrieveUnitDataName(int index)
    {
        if (unitDataCollection.unitDataList[index] != null)
            return unitDataCollection.unitDataList[index].name;
        else
        {
            Debug.LogError("JSONHandler::RetrieveUnitDataName - unitDataList[" + index + "] is null");
            return "unitDataList[" + index + "] is null";
        }
    }

    string RetrieveTextFromUIText(string[] codeSegments)
    {
        string ret = " ";
        UIText text = GetUITextFromLanguage();

        if (codeSegments[2] != null) //filtrar tipus d'unitat
        {
            if (codeSegments[2] == "language")
            {
                ret = text.language;
            }
            else if (codeSegments[2] == "createdBy")
            {
                ret = text.createdBy;
            }
            else if (codeSegments[2] == "loading")
            {
                ret = text.loading;
            }
            else if (codeSegments[2] == "pressStart")
            {
                ret = text.pressStart;
            }
            else if (codeSegments[2] == "versus")
            {
                ret = text.versus;
            }
            else if (codeSegments[2] == "versusDescription1")
            {
                ret = text.versusDescription1;
            }
            else if (codeSegments[2] == "versusDescription2")
            {
                ret = text.versusDescription2;
            }
            else if (codeSegments[2] == "battle")
            {
                ret = text.battle;
            }
            else if (codeSegments[2] == "battleDescription1")
            {
                ret = text.battleDescription1;
            }
            else if (codeSegments[2] == "battleDescription2")
            {
                ret = text.battleDescription2;
            }
            else if (codeSegments[2] == "tutorial")
            {
                ret = text.tutorial;
            }
            else if (codeSegments[2] == "tutorialDescription")
            {
                ret = text.tutorialDescription;
            }
            else if (codeSegments[2] == "options")
            {
                ret = text.options;
            }
            else if (codeSegments[2] == "optionsDescription")
            {
                ret = text.optionsDescription;
            }
            else if (codeSegments[2] == "quit")
            {
                ret = text.quit;
            }
            else if (codeSegments[2] == "quitDescription")
            {
                ret = text.quitDescription;
            }
            else if (codeSegments[2] == "mapSelector")
            {
                ret = text.mapSelector;
            }
            else if (codeSegments[2] == "size")
            {
                ret = text.size;
            }
            else if (codeSegments[2] == "battleSelector")
            {
                ret = text.battleSelector;
            }
            //versus maps
            else if (codeSegments[2] == "turdIsland")
            {
                ret = text.turdIsland;
            }
            else if (codeSegments[2] == "alphaIsland")
            {
                ret = text.alphaIsland;
            }
            else if (codeSegments[2] == "spannIsland")
            {
                ret = text.spannIsland;
            }
            else if (codeSegments[2] == "twinIsle")
            {
                ret = text.twinIsle;
            }
            //battle maps
            else if (codeSegments[2] == "mojiIsland")
            {
                ret = text.mojiIsland;
            }
            else if (codeSegments[2] == "duoFalls")
            {
                ret = text.duoFalls;
            }
            else if (codeSegments[2] == "ridgeIsland")
            {
                ret = text.ridgeIsland;
            }
            else if (codeSegments[2] == "snakeHills")
            {
                ret = text.snakeHills;
            }
            //unit stats
            else if (codeSegments[2] == "movement")
            {
                ret = text.movement;
            }
            else if (codeSegments[2] == "attackRange")
            {
                ret = text.attackRange;
            }
            else if (codeSegments[2] == "strongAgainst")
            {
                ret = text.strongAgainst;
            }
            else if (codeSegments[2] == "weakTo")
            {
                ret = text.weakTo;
            }
            //miscel·lània
            else if (codeSegments[2] == "turnLimit")
            {
                ret = text.turnLimit;
            }
            else if (codeSegments[2] == "pressStartToBegin")
            {
                ret = text.pressStartToBegin;
            }
            else if (codeSegments[2] == "turn")
            {
                ret = text.turn;
            }
            else if (codeSegments[2] == "turnLimitReached1")
            {
                ret = text.turnLimitReached1;
            }
            else if (codeSegments[2] == "turnLimitReached2")
            {
                ret = text.turnLimitReached2;
            }
            //bye
            else if (codeSegments[2] == "thankYou")
            {
                ret = text.thankYou;
            }
            //botons ingame
            else if (codeSegments[2] == "resume")
            {
                ret = text.resume;
            }
            else if (codeSegments[2] == "exit")
            {
                ret = text.exit;
            }
            else if (codeSegments[2] == "endTurn")
            {
                ret = text.endTurn;
            }
            else if (codeSegments[2] == "wait")
            {
                ret = text.wait;
            }
            else if (codeSegments[2] == "capture")
            {
                ret = text.capture;
            }
            else if (codeSegments[2] == "attack")
            {
                ret = text.attack;
            }
            else if (codeSegments[2] == "load")
            {
                ret = text.load;
            }
            else if (codeSegments[2] == "drop")
            {
                ret = text.drop;
            }
            //confirm
            else if (codeSegments[2] == "areYouSure")
            {
                ret = text.areYouSure;
            }
            else if (codeSegments[2] == "confirmExit")
            {
                ret = text.confirmExit;
            }
            else if (codeSegments[2] == "cancelExit")
            {
                ret = text.cancelExit;
            }
            //results
            else if (codeSegments[2] == "winner")
            {
                ret = text.winner;
            }
            else if (codeSegments[2] == "by")
            {
                ret = text.by;
            }
            else if (codeSegments[2] == "domination")
            {
                ret = text.domination;
            }
            else if (codeSegments[2] == "extermination")
            {
                ret = text.extermination;
            }
            else if (codeSegments[2] == "ocupation")
            {
                ret = text.ocupation;
            }
            else if (codeSegments[2] == "draw")
            {
                ret = text.draw;
            }
            else if (codeSegments[2] == "turns")
            {
                ret = text.turns;
            }
            else if (codeSegments[2] == "map")
            {
                ret = text.map;
            }
            //tutorial
            else if (codeSegments[2] == "tutorialSelector")
            {
                ret = text.tutorialSelector;
            }
            else if (codeSegments[2] == "tutorialMap1")
            {
                ret = text.tutorialMap1;
            }
            else if (codeSegments[2] == "tutorialMap1Description")
            {
                ret = text.tutorialMap1Description;
            }
            else if (codeSegments[2] == "tutorialMap2")
            {
                ret = text.tutorialMap2;
            }
            else if (codeSegments[2] == "tutorialMap2Description")
            {
                ret = text.tutorialMap2Description;
            }
            else if (codeSegments[2] == "tutorialMap3")
            {
                ret = text.tutorialMap3;
            }
            else if (codeSegments[2] == "tutorialMap3Description")
            {
                ret = text.tutorialMap3Description;
            }
            else if (codeSegments[2] == "tutorialMap4")
            {
                ret = text.tutorialMap4;
            }
            else if (codeSegments[2] == "tutorialMap4Description")
            {
                ret = text.tutorialMap4Description;
            }
            else if (codeSegments[2] == "tutorialMap5")
            {
                ret = text.tutorialMap5;
            }
            else if (codeSegments[2] == "tutorialMap5Description")
            {
                ret = text.tutorialMap5Description;
            }
            else if (codeSegments[2] == "tutorialMap6")
            {
                ret = text.tutorialMap6;
            }
            else if (codeSegments[2] == "tutorialMap6Description")
            {
                ret = text.tutorialMap6Description;
            }
            else if (codeSegments[2] == "tutorialMap7")
            {
                ret = text.tutorialMap7;
            }
            else if (codeSegments[2] == "tutorialMap7Description")
            {
                ret = text.tutorialMap7Description;
            }
            else if (codeSegments[2] == "tutorialMap8")
            {
                ret = text.tutorialMap8;
            }
            else if (codeSegments[2] == "tutorialMap8Description")
            {
                ret = text.tutorialMap8Description;
            }
        }
        else
        {
            Debug.LogError("JSONHandler::RetrieveTextFromUIText - resourceCode format error");
            return "resourceCode format error";
        }

        return ret;
    }

    UIText GetUITextFromLanguage()
    {
        switch (FindObjectOfType<DataTransferer>().language)
        {
            case Language.ENGLISH:
                return UITextCollection.UITextList[0];

            case Language.CATALA:
                return UITextCollection.UITextList[1];

            case Language.CASTELLANO:
                return UITextCollection.UITextList[2];
        }

        return null;
    }

    string RetrieveTextFromDialogues(string[] codeSegments)
    {
        int index = 0;
        string ret = " ";

        if (codeSegments[2] != null) //filtrar tipus d'unitat
        {
            if (codeSegments[2] == "tutorialMap1")
            {
                index = 0;
            }
            else if (codeSegments[2] == "tutorialMap2")
            {
                index = 1;
            }
            else if (codeSegments[2] == "tutorialMap3")
            {
                index = 2;
            }
            else if (codeSegments[2] == "tutorialMap4")
            {
                index = 3;
            }
            else if (codeSegments[2] == "tutorialMap5")
            {
                index = 4;
            }
            else if (codeSegments[2] == "tutorialMap6")
            {
                index = 5;
            }
            else if (codeSegments[2] == "tutorialMap7")
            {
                index = 6;
            }
            else if (codeSegments[2] == "tutorialMap8")
            {
                index = 7;
            }
        }
        else
        {
            Debug.LogError("JSONHandler::RetrieveTextFromUnitData - resourceCode format error");
            return "resourceCode format error";
        }

        if (codeSegments[3] != null) //filtrar element
        {
            //start
            if (codeSegments[3] == "startDialogue1")
            {
                ret = dialoguesCollection.dialoguesList[index].startDialogue1;
            }
            else if (codeSegments[3] == "startDialogue2")
            {
                ret = dialoguesCollection.dialoguesList[index].startDialogue2;
            }
            else if (codeSegments[3] == "startDialogue3")
            {
                ret = dialoguesCollection.dialoguesList[index].startDialogue3;
            }
            else if (codeSegments[3] == "startDialogue4")
            {
                ret = dialoguesCollection.dialoguesList[index].startDialogue4;
            }
            else if (codeSegments[3] == "startDialogue5")
            {
                ret = dialoguesCollection.dialoguesList[index].startDialogue5;
            }
            else if (codeSegments[3] == "startDialogue6")
            {
                ret = dialoguesCollection.dialoguesList[index].startDialogue6;
            }
            else if (codeSegments[3] == "startDialogue7")
            {
                ret = dialoguesCollection.dialoguesList[index].startDialogue7;
            }
            else if (codeSegments[3] == "startDialogue8")
            {
                ret = dialoguesCollection.dialoguesList[index].startDialogue8;
            }
            //end
            else if (codeSegments[3] == "endDialogue1")
            {
                ret = dialoguesCollection.dialoguesList[index].endDialogue1;
            }
            else if (codeSegments[3] == "endDialogue2")
            {
                ret = dialoguesCollection.dialoguesList[index].endDialogue2;
            }
            else if (codeSegments[3] == "endDialogue3")
            {
                ret = dialoguesCollection.dialoguesList[index].endDialogue3;
            }
            else if (codeSegments[3] == "endDialogue4")
            {
                ret = dialoguesCollection.dialoguesList[index].endDialogue4;
            }
            else if (codeSegments[3] == "endDialogue5")
            {
                ret = dialoguesCollection.dialoguesList[index].endDialogue5;
            }
            else if (codeSegments[3] == "endDialogue6")
            {
                ret = dialoguesCollection.dialoguesList[index].endDialogue6;
            }
            else if (codeSegments[3] == "endDialogue7")
            {
                ret = dialoguesCollection.dialoguesList[index].endDialogue7;
            }
            else if (codeSegments[3] == "endDialogue8")
            {
                ret = dialoguesCollection.dialoguesList[index].endDialogue8;
            }
        }
        else
        {
            Debug.LogError("JSONHandler::RetrieveTextFromUnitData - resourceCode format error");
            return "resourceCode format error";
        }

        return ret;
    }
}

//Apunts importants sobre llegir json o qualsevol format (xml?):
//El path de l'aplicació només serveix per quan s'està fent servir l'editor. Lògicament quan hi ha una build la seva estructura no té res a veure amb la del projecte
//de manera que s'ha de fer servir Resources.Load(). Tots els arxius que estiguin guardats a Assets/Resources podran ser carregats d'aquesta manera
