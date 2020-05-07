using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class UnitDataCollection
{
    public List<UnitData> unitDataList = new List<UnitData>();
}

[Serializable]
public class UnitData
{
    public string name;
    public int movementRange;
    public int attackRangeFrom;
    public int attackRangeTo;

    public float vsInfantry;
    public float vsTransport;
    public float vsTank;
    public float vsAerial;
    public float vsGunner;
    public float vsRanged;

    public float fromInfantry;
    public float fromTransport;
    public float fromTank;
    public float fromAerial;
    public float fromGunner;
    public float fromRanged;
}

