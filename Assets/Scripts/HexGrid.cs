using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class HexGrid : MonoBehaviour
{
    //World Size
    public int chunkCountX = 8, chunkCountZ = 6;

    int cellCountX, cellCountZ;

    //public Color defaultColor = Color.white;
    public Texture2D noiseSource;


    //Prefabs
    public HexGridChunk chunkPrefab;
    public HexCell cellPrefab;
    public Text cellLabelPrefab;
    public Terrain_Type defaultTerrain;
    public GameObject settlementPrefab;

    //public values
    public int defaultSealevel;
    public int seed;

    public Resource[] resources;
    //public Image cellSelectedPrefab;

    HexGridChunk[] chunks;
    HexCell[] cells;

    void Awake()
    {
        HexMetrics.noiseSource = noiseSource;
        HexMetrics.InitializeHashGrid(seed);

        cellCountX = chunkCountX * HexMetrics.chunkSizeX;
        cellCountZ = chunkCountZ * HexMetrics.chunkSizeZ;

        CreateChunks();
        CreateCells();
    }

    void CreateChunks()
    {
        chunks = new HexGridChunk[chunkCountX * chunkCountZ];

        for (int z = 0, i = 0; z < chunkCountZ; z++)
        {
            for (int x = 0; x < chunkCountX; x++)
            {
                HexGridChunk chunk = chunks[i++] = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform);
            }
        }
    }

    void CreateCells()
    {
        cells = new HexCell[cellCountZ * cellCountX];

        for (int z = 0, i = 0; z < cellCountZ; z++)
        {
            for (int x = 0; x < cellCountX; x++)
            {
                CreateCell(x, z, i++);
            }
        }
    }

    private void OnEnable()
    {
        if (!HexMetrics.noiseSource)
        {
            HexMetrics.noiseSource = noiseSource;
            HexMetrics.InitializeHashGrid(seed);
        }
    }

    public HexCell GetCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = coordinates.X + coordinates.Z * cellCountX + coordinates.Z / 2;
        return cells[index];
    }

    public HexCell GetCell(HexCoordinates coordinates)
    {
        int z = coordinates.Z;
        if (z < 0 || z >= cellCountZ)
        {
            return null;
        }
        int x = coordinates.X + z / 2;
        if (x < 0 || x >= cellCountX)
        {
            return null;
        }
        return cells[x + z * cellCountX];
    }

    void CreateCell (int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffestCorrdinates(x, z);
        cell.name = "cell" + cell.coordinates;
        cell.foodValue = 1f;
        cell.resources = new List<Resource>();
        SetDefaultTerrain(cell);        

        if (x > 0)
        {
            cell.SetNeighbor(HexDirection.W, cells[i - 1]);
        }
        if (z > 0)
        {
            if ((z & 1) == 0)
            {
                cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX]);
                if (x > 0)
                {
                    cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX - 1]);
                }
            }
            else
            {
                cell.SetNeighbor(HexDirection.SW, cells[i - cellCountX]);
                if (x < cellCountX - 1)
                {
                    cell.SetNeighbor(HexDirection.SE, cells[i - cellCountX + 1]);
                }
            }
        }

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSparateLines();
        cell.uiRect = label.rectTransform;
        //Image selected = Instantiate<Image>(cellSelectedPrefab);
        //selected.rectTransform.anchoredPosition = new Vector2(position.x, position.z);
        //cell.uiRect = selected.rectTransform;
        cell.Elevation = 3;
        cell.WaterLevel = defaultSealevel;
        cell.ValidateResources();

        AddCellToChunk(x, z, cell);
    }

    void AddCellToChunk(int x, int z, HexCell cell)
    {
        int chunkX = x / HexMetrics.chunkSizeX;
        int chunkZ = z / HexMetrics.chunkSizeZ;
        HexGridChunk chunk = chunks[chunkX + chunkZ * chunkCountX];

        int localX = x - chunkX * HexMetrics.chunkSizeX;
        int localZ = z - chunkZ * HexMetrics.chunkSizeZ;
        chunk.AddCell(localX + localZ * HexMetrics.chunkSizeX, cell);
    }

    void SetDefaultTerrain(HexCell cell)
    {
        cell.SetTerrainType(defaultTerrain);
    }

    void AddResourceToCell(HexCell cell, Resource resource)
    {
        cell.AddResouces(resource);
    }

    public void ApplyCellValues(int sealevel)
    {
        foreach (HexCell cell in cells)
        {
            cell.WaterLevel = sealevel;
        }
    }

    public void ShowUI(bool visible)
    {
        for (int i = 0; i < chunks.Length; i++)
        {
            chunks[i].ShowUI(visible);
        }
    }
}
