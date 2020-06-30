using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
    [Header("Sprite")]
    public Sprite sprite;

    [Header("Allowed Units")]
    public bool infantry;
    public bool transport;
    public bool tank;
    public bool aerial;
    public bool gunner;
    public bool ranged;

    [Header("Start Dialogues")]
    public List<string> startDialoguesList;

    [Header("End Dialogues")]
    public List<string> endDialoguesList;

    //info
    int maxDialogues = 8;
    public string mapName;

    public List<UnitType> BuildAllowedUnitList()
    {
        List<UnitType> ret = new List<UnitType>();

        if (infantry)
            ret.Add(UnitType.INFANTRY);

        if (transport)
            ret.Add(UnitType.TRANSPORT);

        if (tank)
            ret.Add(UnitType.TANK);

        if (aerial)
            ret.Add(UnitType.AERIAL);

        if (gunner)
            ret.Add(UnitType.GUNNER);

        if (ranged)
            ret.Add(UnitType.RANGED);

        return ret;
    }

    void Start()
    {
        BuildDialogues();

        //string name = gameObject.name;
        //int index = name.IndexOf("(");

        //if (index != 0)
        //    mapName = name.Substring(0, index);
        //else
        //    mapName = gameObject.name;
    }

    void BuildDialogues()
    {
        startDialoguesList = new List<string>();

        for (int i = 1; i <= maxDialogues; i++)
        {
            string nextDialogue = FindObjectOfType<JSONHandler>().RetrieveText("#JSONResource.dialogues." + mapName + ".startDialogue" + i.ToString());
            if (nextDialogue != null)
                startDialoguesList.Add(nextDialogue);
        }

        endDialoguesList = new List<string>();

        for (int i = 1; i <= maxDialogues; i++)
        {
            string nextDialogue = FindObjectOfType<JSONHandler>().RetrieveText("#JSONResource.dialogues." + mapName + ".endDialogue" + i.ToString());
            if (nextDialogue != null)
                endDialoguesList.Add(nextDialogue);
        }
    }
}
