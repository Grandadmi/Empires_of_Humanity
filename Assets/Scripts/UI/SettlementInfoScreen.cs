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
    private string stringFormat3 = "##.#";

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
        settlementPop.text = Mathf.Round(settlement.settlementPopulaiton).ToString();
        settlementRuralPop.text = Mathf.Round(settlement.ruralPopulation).ToString();
        settlementFood.text = settlement.netFood.ToString(stringFormat2);
        settlementHappiness.text = (settlement._PopHappiness * 100).ToString();
        settlementGrowthRate.text = (settlement.localPopGrowth * 100).ToString(stringFormat3);
        foodTooltip.content = settlement.netFood.ToString(stringFormat2) + "\n" + settlement.rawFoodIncome.ToString(stringFormat2) + "\n" + settlement.foodConsuption.ToString(stringFormat2);
    }
}
