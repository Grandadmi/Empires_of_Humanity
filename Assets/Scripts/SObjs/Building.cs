using UnityEngine;

public enum BuildingType
{
    Government,
    Infrastructure,
    Housing,
    Logistics,
    Food,
    Industry,
    Science,
    Culture,

}

[CreateAssetMenu(fileName = "New Building", menuName = "Objects/Buildings", order = 1)]
public class Building : ScriptableObject
{
    //Identifiers
    public new string name;
    public BuildingType type;
    public int level;

    //Build Costs
    public Resource buildResource1;
    public int buildResourceAmount1;
    public Resource buildResource2;
    public int buildResourceAmount2;
    public Resource buildResource3;
    public int buildResourceAmount3;
    public int buildTime;
    public Building prerequsite;

    //Building Upkeep
    public int upkeep;
    public int workforce;
    //public Castetype workforceType;
    public int popHappyMod;

    //Production/Consumption
    public Resource consumedResource1;
    public int consumedAmount1;
    public Resource consumedResource2;
    public int consumedAmount2;
    public Resource outputResource;
    public int outputAmount;
}
