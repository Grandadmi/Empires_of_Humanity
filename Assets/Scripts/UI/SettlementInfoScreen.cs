using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SettlementInfoScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI settlementName;
    [SerializeField] private TextMeshProUGUI settlementPop;
    [SerializeField] private TextMeshProUGUI settlementRuralPop;
    [SerializeField] private TextMeshProUGUI settlementHappiness;
    [SerializeField] private TextMeshProUGUI settlementFood;
    [SerializeField] private TextMeshProUGUI settlementGrowthRate;
    //[SerializeField] private TextMeshProUGUI settlementName;

    //Tooltips
    [SerializeField] private TooltipTrigger foodTooltip;
    [SerializeField] private TooltipTrigger HappyTooltip;

    //Control Image
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform settlementCameraTransform;

    //private string stringFormat1 = "###%";
    private string stringFormat2 = "##.#";
    private string stringFormat3 = "#.##";

    public void OpenUI(Settlement settlement)
    {
        panel.SetActive(true);
        settlementName.text = settlement.settlementName;
        UpdateSettlementUI(settlement);
        Vector3 camPos = settlementCameraTransform.position;
        camPos.x = settlement.hostCell.Position.x;
        camPos.z = settlement.hostCell.Position.z;
        settlementCameraTransform.position = camPos;
    }

    public void CloseUI()
    {
        panel.SetActive(false);
    }
    public void UpdateSettlementUI(Settlement settlement)
    {
        settlementPop.text = "Settlement Population: " + Mathf.Round(settlement.settlementPopulaiton).ToString();
        settlementRuralPop.text = "Rural Population: " + Mathf.Round(settlement.ruralPopulation).ToString();
        settlementFood.text = "Settlement Food Balance: " + settlement.netFood.ToString(stringFormat2);
        settlementHappiness.text = "Settlement Happiness: " + (settlement._PopHappiness * 100).ToString();
        settlementGrowthRate.text = "Local Growth Rate: " + (settlement.localPopGrowth).ToString(stringFormat3);
        foodTooltip.content = "Balance: " + settlement.netFood.ToString(stringFormat2) + 
            "\n" + "\n" + "Income: " + settlement.rawFoodIncome.ToString(stringFormat2) + 
            "\n" + "\n" + "Consuption: " + settlement.foodConsuption.ToString(stringFormat2);
    }
}
