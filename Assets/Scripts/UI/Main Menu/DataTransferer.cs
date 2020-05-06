using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public GameObject map;
    public GameObject minimap;
    public ResultsInfo resultsInfo;

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

    public void TransferResults(DataController.Winner winner, DataController.WinCondition winCondition, int turns)
    {
        resultsInfo = new ResultsInfo(winner, winCondition, turns);
    }
}
