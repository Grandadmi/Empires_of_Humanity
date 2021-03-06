using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public bool editMode;
    public bool playMode;
    public bool devMode;
    public bool allowEditMode;
    public bool allowEdgeScroll;

    public TextMeshProUGUI timeText;

    public Canvas EditorUI;
    //public bool allowWorldEdit;
    //public bool allowCheats;

    //public List<Settlement> settlements;
    public List<string> settlementNames;

    //Global Variables
    public float _PopulationPerFoodUnit;
    public float _PopulationPerGoodsUnit;

    //TEMP LOCATION FOR DEFUALT BUILDINGS
    public Building _DefaultGovBuilding;
    public Building _DefaultInfrastureBuilding;
    public Building _DefualtHousingBuilding;

    
    // Start is called before the first frame update
    void Awake()
    {
        if (!allowEditMode)
        {
            EditorUI.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        SystemTimeUpdate();
        UpdateEditorUI(allowEditMode);
    }

    void UpdateEditorUI(bool active)
    {
        if (active)
        {
            EditorUI.gameObject.SetActive(true);
        }
        else
        {
            EditorUI.gameObject.SetActive(false);
        }
    }

    void SystemTimeUpdate()
    {
        timeText.text = System.DateTime.Now.ToString("HH:mm");
    }

    public void ApplicationQuit()
    {
        Application.Quit();
    }

}
