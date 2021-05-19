using UnityEngine;

public static class HexMetrics
{
    //World Data
    public const int chunkSizeX = 4, chunkSizeZ = 6;

    //Cell MeshData
    public const float outerToInner = 0.866025404f;
    public const float innerToOuter = 1f / outerToInner;
    public const float outerRadius = 4f;
    public const float innerRadius = outerRadius * outerToInner;
    public const float solidFactor = 0.8f;
    public const float solidCenter = solidFactor * 0.4f;
    public const float waterFactor = 0.6f;
    public const float blendFactor = 1f - solidFactor;
    public const float waterBlendFactor = 1f - waterFactor;

    //Cell Elevation Data
    public const float elevationStep = 0.5f;
    public const float horizontalTerraceStepSize = 1f / terraceSteps;
    public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);
    public const int terracesPerSlope = 3;
    public const int terraceSteps = terracesPerSlope * 2 + 1;
    public const float riverBedElevationOffset = -0.65f;
    public const float waterElevationOffset = -0.30f;

    //Noise Data
    public const float cellPerturbStrenth = 0.75f;
    public const float noiseScale = 0.03f;
    public const float elevationPerturbStrength = 0.15f;

    //Hash Grid
    public const int hashGridSize = 512;
    public const float hashGridScale = 0.75f;
    static HexHash[] hashGrid;

    //feature level thresholds
    static float[][] featureThresholds =
    {
        new float[] { 0.0f, 0.0f, 0.2f },
        new float[] { 0.0f, 0.2f, 0.4f },
        new float[] { 0.2f, 0.4f, 0.6f },
        new float[] { 0.4f, 0.6f, 0.8f },
    };

    public static Texture2D noiseSource;

    static Vector3[] corners =
    {
        new Vector3(0f, 0f, outerRadius),
        new Vector3(innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(0f, 0f, -outerRadius),
        new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
        new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
        new Vector3(0f, 0f, outerRadius)
    };

    public static Vector3 GetFirstCorner(HexDirection direction)
    {
        return corners[(int)direction];
    }

    public static Vector3 GetSecondCorner(HexDirection direction)
    {
        return corners[(int)direction + 1];
    }

    public static Vector3 GetFirstSolidCorner(HexDirection direction)
    {
        return corners[(int)direction] * solidFactor;
    }

    public static Vector3 GetSecondSolidCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * solidFactor;
    }

    public static Vector3 GetFirstInnerCorner(HexDirection direction)
    {
        return corners[(int)direction] * solidCenter;
    }

    public static Vector3 GetSecondInnerCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * solidCenter;
    }
    public static Vector3 GetFirstWaterCorner(HexDirection direction)
    {
        return corners[(int)direction] * waterFactor;
    }

    public static Vector3 GetSecondWaterCorner(HexDirection direction)
    {
        return corners[(int)direction + 1] * waterFactor;
    }

    public static Vector3 GetBridge (HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) * blendFactor;
    }

    public static Vector3 GetWaterBridge(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) * waterBlendFactor;
    }

    public static Vector3 GetSolidEdgeMiddle(HexDirection direction)
    {
        return (corners[(int)direction] + corners[(int)direction + 1]) * (0.5f * solidFactor);
    }

    public static Vector3 TerraceLerp (Vector3 a, Vector3 b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        a.x += (b.x - a.x) * h;
        a.z += (b.z - a.z) * h;
        float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
        a.y += (b.y - a.y) * v;
        return a;
    }

    public static Vector3 Perturb(Vector3 position)
    {
        Vector4 sample = HexMetrics.SampleNoise(position);
        position.x += (sample.x * 2f - 1f) * HexMetrics.cellPerturbStrenth;
        position.z += (sample.z * 2f - 1f) * HexMetrics.cellPerturbStrenth;
        return position;
    }

    public static Color TerraceLerp(Color a, Color b, int step)
    {
        float h = step * HexMetrics.horizontalTerraceStepSize;
        return Color.Lerp(a, b, h);
    }

    public static float[] GetFeatureThresholds (int level)
    {
        return featureThresholds[level];
    }

    public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
    {
        if (elevation1 == elevation2)
        {
            return HexEdgeType.Flat;
        }
        int delta = elevation2 - elevation1;
        if (delta == 1 || delta == -1)
        {
            return HexEdgeType.Slope;
        }
        return HexEdgeType.Cliff;
    }

    public static Vector4 SampleNoise(Vector3 position)
    {
        return noiseSource.GetPixelBilinear(position.x * noiseScale, position.y * noiseScale);
    }

    public static HexHash SampleHashGrid(Vector3 position)
    {
        int x = (int)(position.x * hashGridScale) % hashGridSize;
        if (x < 0)
        {
            x += hashGridSize;
        }
        int z = (int)(position.z * hashGridScale) % hashGridSize;
        if (z < 0)
        {
            z += hashGridSize;
        }
        return hashGrid[x + z * hashGridSize];
    }
    public static void InitializeHashGrid(int seed)
    {
        hashGrid = new HexHash[hashGridSize * hashGridSize];
        Random.State currentState = Random.state;
        Random.InitState(seed);
        for (int i = 0; i < hashGrid.Length; i++)
        {
            hashGrid[i] = HexHash.Create();
        }
        Random.state = currentState;
    }
}
