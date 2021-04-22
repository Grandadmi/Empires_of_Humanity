using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    //private unique identifiers
    [SerializeField] HexCell hostCell;
    [SerializeField] int settlementID;


    //Tracked Variables
    public string settlementName = "Test Name";
    public int population;

    private void OnEnable()
    {
        TimeControlManager.OnMonthTick += SettlementUpdate;
    }

    private void OnDisable()
    {
        TimeControlManager.OnMonthTick -= SettlementUpdate;
    }

    private void SettlementUpdate()
    {
        //This will be called every month
        Debug.Log(settlementName + " called Settlement Update Function");
    }
}
