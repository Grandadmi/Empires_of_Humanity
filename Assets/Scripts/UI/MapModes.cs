using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapModes : MonoBehaviour
{
    public HexGrid hexGrid;

    public void ShowUI (bool visible)
    {
        hexGrid.ShowUI(visible);
    }
}
