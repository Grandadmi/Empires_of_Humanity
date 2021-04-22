using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Helper : MonoBehaviour
{
    string sliderTextToUpdate = "0";

    [SerializeField] private TextMeshProUGUI sliderText;
    [SerializeField] private Image targetImage;
    [SerializeField] private GameObject targetScreen;
    [SerializeField] private Image initialSprite;
    [SerializeField] private Image spriteToSwap;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI valueText;

    private Image activeSprite;
    public GameManager manager;

    void Awake()
    {
        if (manager == null)
        {
            return;
        }
        if (manager.editMode == false)
        {
            ShowUIImage(false);
        }
        activeSprite = initialSprite;
    }

    public void TextUpdate(float textUpdateNumber)
    {
        sliderTextToUpdate = textUpdateNumber.ToString();
        sliderText.text = sliderTextToUpdate;
    }

    public void OpenCloseEditorUI()
    {
        if (manager.editMode == true)
        {
            ShowUIImage(false);
            manager.editMode = false;
            return;
        }
        if (manager.editMode == false)
        {
            ShowUIImage(true);
            manager.editMode = true;
            return;
        }
    }

    public void ShowUIImage(bool visible)
    {
        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(visible);
        }
    }
    public void ShowUICanvas(bool visible)
    {
        if (targetScreen != null)
        {
            targetScreen.gameObject.SetActive(visible);
        }
    }

    public void SwapSprites()
    {
        if (initialSprite.enabled == true)
        {
            activeSprite.enabled = false;
            spriteToSwap.enabled = true;
            activeSprite = spriteToSwap;
        }
        else
        {
            activeSprite.enabled = false;
            initialSprite.enabled = true;
            activeSprite = initialSprite;
        }

    }
    public void UpdateResourceUI(Resource resource)
    {
        icon.sprite = resource.icon;
        if (valueText != null)
        {
            valueText.text = resource.value.ToString();
        }
    }
}
