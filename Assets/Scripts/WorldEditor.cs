using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/*TODO
 * Add new roads method
 * dynamic shore gen
 * dynamic peaks gen
 * further sealevel functionality
 */

enum OptionalToggle
{
    Disabled, Add, Remove
}

public class WorldEditor : MonoBehaviour
{
    //The Manager
    public GameManager GameManager;
    //The array of Scriptable Terrain Types
    public Terrain_Type[] terrain_Types;
    //The array of All map resources
    public Resource[] resources;

    //The grid of cells that is to be edited
    public HexGrid HexGrid;

    //ui controlling boolians
    bool applyTerrain;
    bool applyElevation = false;
    bool applyResource = false;
    bool applySettlement = false;
    bool applyImprovement = false;
    bool applyForest = false;
    private bool isDrag;

    //List of values to be set per cell
    private Terrain_Type activeTerrain;
    private Terrain_Type overightType;
    private Resource activeResource;

    //private Color activeTerrainColor;
    private int activeElevation;
    private int activeSealevel;
    private int activeSettlementLevel;
    private int activeImprovementLevel;
    private int activeForestLevel;

    OptionalToggle riverMode;

    //controlling UI Elements
    [SerializeField] private TabGroup editorPanelsTabGroup;
    [SerializeField] private GameObject resourcePanel;
    [SerializeField] private GameObject resourceUIArrayPrefab;
    [SerializeField] private Toggle disableResourceEditToggle;
    [SerializeField] private Toggle disableTerrainEditToggle;
    [SerializeField] private Toggle disableElevationEditToggle;
    [SerializeField] private Toggle disableRiverEditToggle;
    [SerializeField] private Toggle disableSettlementEditToggle;
    [SerializeField] private Toggle disableImprovementEditToggle;
    [SerializeField] private Toggle disableForestEditToggle;

    HexDirection dragDirection;
    HexCell currentCell;
    HexCell previousCell;
    HexCell dragStartCell;
    HexCell dragCurrentCell;

    //Event Crap
    UnityAction UIpanelchange;   

    void Awake()
    {
        SelectTerrainType(-1);
        activeSealevel = HexGrid.defaultSealevel;
        CreateResourceUIArray();

        UIpanelchange += DeselectEditorUIComponents;

        foreach (TabButton button in editorPanelsTabGroup.tabButtons)
        {
            button.onTabDeselected.AddListener(UIpanelchange);
        }
    }

    void Update()
    {
        HandleInput();        
    }

    void HandleInput()
    {
        //This will handle all mouse inputs while in edit mode
        if (GameManager.editMode == true)
        {
            if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                HandleSelection();
            }
            if (Input.GetMouseButton(0) && previousCell != currentCell && !EventSystem.current.IsPointerOverGameObject())
            {
                Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(inputRay, out hit))
                {
                    dragCurrentCell = HexGrid.GetCell(hit.point);
                    if (dragCurrentCell != currentCell)
                    {
                        HandleDrag();
                    }
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                DeselectEditorUIComponents();
            } 
        }
    }

    void HandleSelection()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (currentCell != null)
        {
            previousCell = currentCell;
        }
        if (Physics.Raycast(inputRay, out hit))
        {
            dragStartCell = currentCell = HexGrid.GetCell(hit.point);
            //Debug.Log("Hit cell" + currentCell);
            EditCell(currentCell);
        }
        else
        {
            previousCell = null;
        }
    }

    void HandleDrag()
    {
        previousCell = dragStartCell;
        currentCell = dragCurrentCell;
        ValidateDrag(currentCell);
        if (isDrag)
        {
            EditCell(currentCell);
            EditRiver(currentCell);
            dragStartCell = currentCell;
        }
    }
    
    public void DeselectEditorUIComponents()
    {
        disableTerrainEditToggle.isOn = true;
        disableElevationEditToggle.isOn = false;
        disableResourceEditToggle.isOn = true;
        disableRiverEditToggle.isOn = true;
        disableSettlementEditToggle.isOn = false;
        disableForestEditToggle.isOn = false;
        disableImprovementEditToggle.isOn = false;
    }
    void ValidateDrag(HexCell currentcell)
    {
        for (dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; dragDirection++)
        {
            if (previousCell.GetNeighbor(dragDirection) == currentcell)
            {
                isDrag = true;
                return;
            }
        }
        isDrag = false;
    }

    void EditCell(HexCell cell)
    {
        //set of states for editing terrain cells
        if (applyTerrain)
        {
            //Change cell type from generic type_Water to deep or shallow Ocean
            if (activeTerrain.terrainID == 10)
            {
                if (cell.IsDeep)
                {
                    SetOverightType(9);
                }
                //if target cell is above sealevel set it to one elevation below sealevel
                if (cell.IsUnderwater == false)
                {
                    cell.Elevation = (activeSealevel - 1);
                    SetOverightType(8);
                }
                else
                {
                    SetOverightType(8);
                }
                cell.SetTerrainType(overightType);
                return;
            }
            //if target cell is underwater set it to be above the water
            if (activeTerrain.terrainID != 10 && cell.IsUnderwater)
            {
                cell.Elevation = activeSealevel;
            }
            cell.SetTerrainType(activeTerrain);
            cell.ValidateResources();
        }
        if (applyElevation)
        {
            //setting of the cell elevation
            cell.Elevation = activeElevation;
            //check if cell is now underwater and dynamicly set deep/shallow ocean depending on the depth below sealevel
            if (cell.IsUnderwater)
            {
                if (cell.IsDeep)
                {
                    SetOverightType(9);
                }
                else
                {
                    SetOverightType(8);
                }
                cell.SetTerrainType(overightType);
            }
            //check to see if cell is now above water and is a ocean type otherwise return original terrain
            if (cell.IsUnderwater == false && (cell.cellTerrainID == 8 || cell.cellTerrainID == 7))
            {
                SetOverightType(0);
                cell.SetTerrainType(overightType);
            }
            else
            {
                return;
            }
            //make sure that the new terrain is compatable with the resources that exist on it
            cell.ValidateResources();
        }
        if (riverMode == OptionalToggle.Remove)
        {
            cell.RemoveRiver();
        }
        //check to see if a resource is selected, then add or remove the selected resource to the cell
        if (applyResource)
        {
            if (cell.resources.Contains(activeResource))
            {
                cell.RemoveResources(activeResource);
            }
            else
            {
                cell.AddResouces(activeResource);
            }
            //make sure that the resource being added is a vaild resource for the cell
            cell.ValidateResources();
        }
        //TEMP!! add settlement to map for dictated value
        if (applySettlement)
        {
            cell.SettlementLevel = activeSettlementLevel;
        }
        if (applyForest)
        {
            cell.ForestLevel = activeForestLevel;
        }
        if (applyImprovement)
        {
            cell.ImprovementLevel = activeImprovementLevel;
        }
    }

    void EditRiver(HexCell cell)
    {
        //River Add/Remove
        if (riverMode == OptionalToggle.Remove)
        {
            cell.RemoveRiver();
        }
        if (isDrag)
        {
            HexCell othercell = cell.GetNeighbor(dragDirection.Opposite());
            if (othercell)
            {
                if (riverMode == OptionalToggle.Add)
                {
                    othercell.SetOutgoingRiver(dragDirection);
                }
            }
        }
    }

    public void ApplySealevel()
    {
        HexGrid.ApplyCellValues(activeSealevel);
    }

    public void SelectTerrainType(int index)
    {
        //grab the currently selected terrain from the array and assign it to the currently selected cell
        applyTerrain = index >= 0;
        if (applyTerrain)
        {        
            activeTerrain = terrain_Types[index];
        }
    }

    void SetOverightType(int index)
    {
        overightType = terrain_Types[index];
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetSealevelValue(float sealevel)
    {
        activeSealevel = (int)sealevel;
    }

    public void SetApplySettlement(bool toggle)
    {
        applySettlement = toggle;
    }

    public void SetSettlement(float settlementValue)
    {
        activeSettlementLevel = (int)settlementValue;
    }
    public void SetApplyImprovement(bool toggle)
    {
        applyImprovement = toggle;
    }

    public void SetImprovemnt(float improvementValue)
    {
        activeImprovementLevel = (int)improvementValue;
    }
    public void SetApplyForest(bool toggle)
    {
        applyForest = toggle;
    }

    public void SetForest(float forestValue)
    {
        activeForestLevel = (int)forestValue;
    }

    public void SelectResource(int index)
    {
        //Debug.Log("Selected" + resources[index].name);
        applyResource = index >= 1;
        if (applyResource)
        {
            if (index == 0)
            {
                return;
            }
            else
            {
                activeResource = resources[index];
            }
        }
    }

    public void SetRiverMode(int mode)
    {
        riverMode = (OptionalToggle)mode;
    }

    private void CreateResourceUIArray()
    {
        for (int i = 0; i < resources.Length; i++)
        {
            GameObject holder = Instantiate(resourceUIArrayPrefab, resourcePanel.transform);

            UI_Helper helper = holder.GetComponentInChildren<UI_Helper>();
            EditorToggleManager holderInfo = holder.GetComponent<EditorToggleManager>();
            Toggle toggle = holder.GetComponentInChildren<Toggle>();
            ToggleGroup tg_group = resourcePanel.GetComponent<ToggleGroup>();
            toggle.group = tg_group;
            holderInfo.editorObject = this;
            holderInfo.arrayPos = i;

            if (i == 0)
            {
                toggle.isOn = true;
                disableResourceEditToggle = toggle;
            }
            helper.UpdateTooltip(resources[i].name);
            helper.UpdateResourceUI(resources[i]);
        }

    }
}
