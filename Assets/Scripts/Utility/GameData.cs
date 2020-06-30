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
public class UITextCollection
{
    public List<UIText> UITextList = new List<UIText>();
}

[Serializable]
public class DialoguesCollection
{
    public List<Dialogues> dialoguesList = new List<Dialogues>();
}

[Serializable]
public class UnitData
{
    public string name;
    public string description;

    public int value;
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

[Serializable]
public class UIText
{
    //language
    public string language;

    //intro
    public string createdBy;
    public string loading;
    public string pressStart;

    //mode
    public string versus;
    public string versusDescription1;
    public string versusDescription2;
    public string battle;
    public string battleDescription1;
    public string battleDescription2;
    public string tutorial;
    public string tutorialDescription;
    public string options;
    public string optionsDescription;
    public string quit;
    public string quitDescription;

    //versus
    public string mapSelector;
    public string size;

    //battle
    public string battleSelector;

    //versus maps
    public string turdIsland;
    public string alphaIsland;
    public string spannIsland;
    public string twinIsle;

    //battle maps
    public string mojiIsland;
    public string duoFalls;
    public string ridgeIsland;
    public string snakeHills;

    //unit stats
    public string movement;
    public string attackRange;
    public string strongAgainst;
    public string weakTo;

    //miscel·lània
    public string turnLimit;
    public string pressStartToBegin;
    public string turn;
    public string turnLimitReached1;
    public string turnLimitReached2;

    //bye
    public string thankYou;

    //botons ingame
    public string resume;
    public string exit;
    public string endTurn;
    public string wait;
    public string capture;
    public string attack;
    public string load;
    public string drop;

    //confirm
    public string areYouSure;
    public string confirmExit;
    public string cancelExit;

    //resultats
    public string winner;
    public string by;
    public string domination;
    public string extermination;
    public string ocupation;
    public string draw;
    public string turns;
    public string map;

    //tutorial
    public string tutorialSelector;
    public string tutorialMap1;
    public string tutorialMap1Description;
    public string tutorialMap2;
    public string tutorialMap2Description;
    public string tutorialMap3;
    public string tutorialMap3Description;
    public string tutorialMap4;
    public string tutorialMap4Description;
    public string tutorialMap5;
    public string tutorialMap5Description;
    public string tutorialMap6;
    public string tutorialMap6Description;
    public string tutorialMap7;
    public string tutorialMap7Description;
    public string tutorialMap8;
    public string tutorialMap8Description;
}

[Serializable]
public class Dialogues
{
    //map
    public string mapName;

    //start
    public string startDialogue1;
    public string startDialogue2;
    public string startDialogue3;
    public string startDialogue4;
    public string startDialogue5;
    public string startDialogue6;
    public string startDialogue7;
    public string startDialogue8;

    //end
    public string endDialogue1;
    public string endDialogue2;
    public string endDialogue3;
    public string endDialogue4;
    public string endDialogue5;
    public string endDialogue6;
    public string endDialogue7;
    public string endDialogue8;
}

