using UnityEngine;

[CreateAssetMenu (fileName = "New Terrain Type", menuName ="Objects/Terrain/Type", order = 1)]
public class Terrain_Type : ScriptableObject
{
    public new string name;
    public string type;

    public Color color;
    //public Texture2D texture;

    public int terrainID;

    public float movementModifer;
    public float foodModifier;
    public float resourceModifier;
    public float DefenseModifier;

    
}
