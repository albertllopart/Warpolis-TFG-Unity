using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language
{
    ENGLISH, CATALA, CASTELLANO
};

public class DataTransferer : MonoBehaviour
{
    public class ResultsInfo
    {
        public ResultsInfo(DataController.Winner winner, DataController.WinCondition winCondition, int turns)
        {
            this.winner = winner;
            this.winCondition = winCondition;
            this.turns = turns;
        }

        public DataController.Winner winner;
        public DataController.WinCondition winCondition;
        public int turns;
    }

    public Language language;
    public GameObject map;
    public GameObject minimap;
    public ResultsInfo resultsInfo;
    public DataController.PlayerCommander caniPlayer;
    public DataController.PlayerCommander hipsterPlayer;
    public List<UnitType> allowedUnits;
    public bool isTutorial;

    void Awake()
    {
        
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void TransferMap(GameObject map, GameObject minimap)
    {
        this.map = map;
        this.minimap = minimap;

        minimap.SetActive(false);
    }

    public void TransferMap(GameObject map)
    {
        this.map = map;
    }

    public void TransferResults(DataController.Winner winner, DataController.WinCondition winCondition, int turns)
    {
        resultsInfo = new ResultsInfo(winner, winCondition, turns);
    }

    public void TransferCommanders(DataController.PlayerCommander caniPlayer, DataController.PlayerCommander hipsterPlayer)
    {
        this.caniPlayer = caniPlayer;
        this.hipsterPlayer = hipsterPlayer;
    }

    public void TransferLanguage(Language language)
    {
        this.language = language;
    }

    public void TransferAllowedUnits(List<UnitType> allowedUnits)
    {
        this.allowedUnits = allowedUnits;

        if (allowedUnits.Count == 0)
        {
            allowedUnits.Add(UnitType.INFANTRY);
            allowedUnits.Add(UnitType.TRANSPORT);
            allowedUnits.Add(UnitType.TANK);
            allowedUnits.Add(UnitType.AERIAL);
            allowedUnits.Add(UnitType.GUNNER);
            allowedUnits.Add(UnitType.RANGED);
        }
    }

    public void TransferIsTutorial(bool isTutorial)
    {
        this.isTutorial = isTutorial;
    }
}
