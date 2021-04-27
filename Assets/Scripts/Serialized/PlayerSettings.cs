using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSettings : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private CameraController camControl;
    [SerializeField] private TimeControlManager timeControl;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider sensititvitySlider;
    [SerializeField] private TextMeshProUGUI cameraSensText;


    private readonly string stringFormat = "###";

    public void Start()
    {
        sensititvitySlider.value = camControl.cameraMoveSensitivity;
        cameraSensText.text = sensititvitySlider.value.ToString(stringFormat);
    }

    public void SetCameraSensitivity(float value)
    {
        camControl.cameraMoveSensitivity = value;
        float valueText = value * 100f;
        cameraSensText.text = valueText.ToString(stringFormat);
    }

    public void SetEdgeScroll(bool isEdgeScroll)
    {
        gameManager.allowEdgeScroll = isEdgeScroll;
    }

    public void SetDevMode(bool isDevMode)
    {
        gameManager.devMode = isDevMode;
    }

    public void SetAllowWorldEdit(bool isWorldEdit)
    {
        gameManager.allowEditMode = isWorldEdit;
    }

    public void CloseUI()
    {
        settingsPanel.SetActive(false);
        camControl.isInSettingsMenu = false;
    }

    public void OpenUI()
    {
        settingsPanel.SetActive(true);
        timeControl.UIPause();
        camControl.isInSettingsMenu = true;
    }
}
