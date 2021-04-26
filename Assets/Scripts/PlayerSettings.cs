using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSettings : MonoBehaviour
{
    [SerializeField] private CameraController camControl;
    [SerializeField] private TimeControlManager timeControl;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider sensititvitySlider;
    [SerializeField] private TextMeshProUGUI cameraSensText;

    public void Start()
    {
        sensititvitySlider.value = camControl.cameraMoveSensitivity;
        cameraSensText.text = sensititvitySlider.value.ToString();
    }

    public void SetCameraSensitivity(float value)
    {
        camControl.cameraMoveSensitivity = value;
        float valueText = value * 100f;
        cameraSensText.text = valueText.ToString();
    }

    public void CloseUI()
    {
        settingsPanel.SetActive(false);
    }

    public void OpenUI()
    {
        settingsPanel.SetActive(true);
        timeControl.UIPause();
    }
}
