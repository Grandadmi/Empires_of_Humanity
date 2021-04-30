using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _versionText;
    // Start is called before the first frame update
    void Start()
    {
        _versionText.text = Application.version.ToString();
    }

}
