using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ResourceType
{
    Food, Mineral, Sea, Sea_Food, Construction, Rare, Lux
}

[CreateAssetMenu(fileName = "New Resource", menuName = "Objects/Terrain/Resource", order = 2)]
public class Resource : ScriptableObject
{
    public new string name;
    public ResourceType type;

    public int value;

    public Sprite icon;
}
