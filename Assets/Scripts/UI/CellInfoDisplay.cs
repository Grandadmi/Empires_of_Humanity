using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CellInfoDisplay : MonoBehaviour
{
    //UI Prefabs
    [SerializeField] private GameObject resourceBoxPrefab;

    //UI Compartments
    [SerializeField] private GameObject resourceDisplay;
    [SerializeField] private GameObject foodValueDisplay;
    [SerializeField] private GameObject supplyValueDisplay;
    [SerializeField] private GameObject influenceValueDisplay;
    [SerializeField] private GameObject movementValueDisplay;
    [SerializeField] private GameObject moistureValueDisplay;

    //MISC UI Elements
    [SerializeField] private TextMeshProUGUI cellTypeString;
    [SerializeField] private Image cellArtworkDisplay;
    [SerializeField] private Slider influenceSlider;

    private HexCell selectedCell;
    private HexGrid grid;

    //Various Utility Collections
    [SerializeField] private List<GameObject> prefabs;

    public void Start()
    {
        grid = FindObjectOfType<HexGrid>();
    }

    public void UpdateUIInformation(HexCell cell)
    {
        selectedCell = cell;
        cellTypeString.text = cell.cellName.ToString(); //Set to biome instead of terrain
        cellArtworkDisplay.color = cell.terrain_Type.color; //Set to biome Artwork w/feature mods
        influenceSlider.value = 0f; //Display of cell influences in graphical form
        foodValueDisplay.GetComponentInChildren<TextMeshProUGUI>().text = cell.foodValue.ToString();
        //Add to display as components are finished
        //supplyValueDisplay.GetComponentInChildren<TextMeshProUGUI>().text = cell.foodValue.ToString();
        //influenceValueDisplay.GetComponentInChildren<TextMeshProUGUI>().text = cell.foodValue.ToString();
        //movementValueDisplay.GetComponentInChildren<TextMeshProUGUI>().text = cell.foodValue.ToString();
        //moistureValueDisplay.GetComponentInChildren<TextMeshProUGUI>().text = cell.foodValue.ToString();

        //Destroy the resource display prefabs before updating for new cell data
        for (int i = 0; i < prefabs.Count; i++)
        {
            Destroy(prefabs[i]);
        }
        //get each unique non food resource and display it and its value
        foreach (Resource resource in cell.resources)
        {
            GameObject resourceBox = Instantiate(resourceBoxPrefab, resourceDisplay.transform);
            UI_Helper helper = resourceBox.GetComponent<UI_Helper>();
            helper.UpdateResourceUI(resource);
            helper.UpdateTooltip(resource.name);
            prefabs.Add(resourceBox);            
        }
        //TODO: do above for the cell modifiers
    }

    public void  UISettlementFounding()
    {
        if (!selectedCell.IsUnderwater)
        {
            selectedCell.FoundSettlement(grid.settlementPrefab);
        }
    }
}
