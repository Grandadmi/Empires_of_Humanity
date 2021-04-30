using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    //Connections
    private GameManager gameManager;
    private HexGrid hexGrid;
    [SerializeField] private GameObject settlementPrefab;

    //private unique identifiers
    public HexCell hostCell;
    [SerializeField] int settlementID;

    //Tracked Variables
    public string settlementName;
    public float settlementPopulaiton;
    public float ruralPopulation;
    public int settlementLevel;
    public float _PopHappiness;

    //Resources
    public float rawFoodIncome;
    public float foodConsuption;
    public float netFood;
    public float storedFood;
    public float _Manpower;

    //TempVariables
    public float popGrowthMod = 0.01f;
    public float popDeclineMod = 0.01f;
    public float empireRuralPercentage = 0.6f;
    public float _EmpireManpowerPercentage = 0.8f;

    //Building Slots
    //public Building _govBuilding;
    //public Building _infrastrucureBuilding;
    //public Building _housingBuilding;
    //public Building _slot1;
    //public Building _slot2;
    //public Building _slot3;
    //public Building _slot4;
    //public Building _slot5;
    //public Building _slot6;

    //Local Storages
    [SerializeField] private float _StoredFoodMax = 100f;

    //Local Variables
    public float localPopGrowth = 0.01f;
    public bool isStarving;

    //ControledCells
    public List<HexCell> controlledCells;
    private int AOIradius;
    private float _PopHappyBonus;

    public void InstatiateSettlement(HexCell cell, string name ="")
    {
        gameManager = FindObjectOfType<GameManager>();
        hexGrid = FindObjectOfType<HexGrid>();
        gameManager.settlements.Add(this);
        settlementID = (gameManager.settlements.Count);
        if (name == null || settlementName == "null")
        {
            int randomName = (Random.Range(0, gameManager.settlementNames.Count)) - 1;
            settlementName = gameManager.settlementNames[randomName];
        }
        else
        {
            settlementName = name;
        }
        hostCell = cell;
        //Temporary Population at Instatation
        int initalPop = 150;
        ruralPopulation = Mathf.Round(initalPop * empireRuralPercentage);
        settlementPopulaiton = initalPop - ruralPopulation;
        settlementLevel = 1;
        AOIradius = settlementLevel;
        ClaimAreaofInfluence();
        SetConsumptions();
        ProduceResources();
        SetPopHappiness();
        Debug.Log("New settlement founded with name " + settlementName + " at " + hostCell.coordinates);
    }

    public void RazeSettlement()
    {
        gameManager.settlements.Remove(this);
        foreach (HexCell cell in controlledCells)
        {
            cell.controllingSettelmentID = 0;
        }
        controlledCells.Clear();
        Destroy(settlementPrefab);
    }

    public void BuildingBuilding(Building building)
    {
        Debug.Log("Built" + building.name + "This function not fully implemented");
    }

    private void OnEnable()
    {
        TimeControlManager.OnMonthTick += SettlementUpdate;
    }

    private void OnDisable()
    {
        TimeControlManager.OnMonthTick -= SettlementUpdate;
    }

    private void ProduceResources()
    {
        rawFoodIncome = 0f;
        for (int i = 0; i < controlledCells.Count; i++)
        {
            if (controlledCells[i] == hostCell)
            {
                continue;
            }
            rawFoodIncome += controlledCells[i].foodValue;
        }

        netFood = rawFoodIncome - foodConsuption;

        if (netFood > 0)
        {
            isStarving = false;
            if (storedFood < _StoredFoodMax)
            {
                storedFood += netFood;
                if (storedFood > _StoredFoodMax)
                {
                    storedFood = _StoredFoodMax;
                }
            }
        }
        if (netFood < 0)
        {
            float foodDeficit = netFood;
            if (storedFood > foodDeficit)
            {
                isStarving = false;
                storedFood += netFood;
            }
            if (storedFood < foodDeficit && storedFood > 0)
            {
                storedFood += foodDeficit;
                isStarving = true;
                storedFood = 0;
            }
            if (storedFood == 0)
            {
                isStarving = true;
            }          
        }
    }

    private void SetConsumptions()
    {
        //food
        foodConsuption = (settlementPopulaiton / gameManager._PopulationPerFoodUnit) + ruralPopulation / (gameManager._PopulationPerFoodUnit * 2);
        //Consumer Goods
    }

    private void UpdatePopulations()
    {
        localPopGrowth = ((netFood / gameManager._PopulationPerFoodUnit) * _PopHappyBonus) + (-(popDeclineMod) + popGrowthMod);
        float initSettlepop = settlementPopulaiton;
        float initRuralpop = ruralPopulation;
        settlementPopulaiton = (localPopGrowth * initSettlepop) + initSettlepop;
        ruralPopulation = (localPopGrowth * initRuralpop) + initRuralpop;
        float _NewPop = Mathf.Round(settlementPopulaiton + ruralPopulation);
        _Manpower = _NewPop * _EmpireManpowerPercentage;
    }

    private void SetPopHappiness()
    {
        _PopHappiness = 0.8f;
        if (isStarving)
        {
            _PopHappiness += -0.5f;
        }
        if (_PopHappiness >= 0.8f)
        {
            _PopHappyBonus = 2f;
        }
        else if (_PopHappiness >= 0.6f)
        {
            _PopHappyBonus = 1f;
        }
        else if (_PopHappiness >= 0.4f)
        {
            _PopHappyBonus = 0f;
        }
        else if (_PopHappiness <= 0.4f)
        {
            _PopHappyBonus = -1f;
        }
        else if (_PopHappiness <= 0.2f)
        {
            _PopHappyBonus = -2f;
        }
        else if (_PopHappiness <= 0)
        {
            _PopHappyBonus = -3f;
        }
    }

    private void ClaimAreaofInfluence()
    {
        int centerX = hostCell.coordinates.X;
        int centerZ = hostCell.coordinates.Z;

        for (int r = 0, z = centerZ - AOIradius; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + AOIradius; x++)
            {
                HexCell claimedCell = hexGrid.GetCell(new HexCoordinates(x, z));
                claimedCell.controllingSettelmentID = settlementID;
                controlledCells.Add(claimedCell);
                Debug.Log(settlementName + "claimed cell" + claimedCell.coordinates);
            }
        }
        for (int r = 0, z = centerZ + AOIradius; z > centerZ; z--, r++)
        {
            for (int x = centerX - AOIradius; x <= centerX + r; x++)
            {
                HexCell claimedCell = hexGrid.GetCell(new HexCoordinates(x, z));
                claimedCell.controllingSettelmentID = settlementID;
                controlledCells.Add(claimedCell);
                Debug.Log(settlementName + "claimed cell" + claimedCell.coordinates);
            }
        }
    }

    private void SettlementUpdate()
    {
        UpdatePopulations();
        ProduceResources();
        SetConsumptions();
        SetPopHappiness();
        Debug.Log(settlementName + " called Settlement Update Function");
    }
}
