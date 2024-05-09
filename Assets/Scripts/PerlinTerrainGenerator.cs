using UnityEngine;

public class PerlinTerrainGenerator : MonoBehaviour
{
    // Dimensions of the terrain
    public int terrainWidth = 500;
    public int terrainHeight = 500;

    // Height multiplier
    public int heightScale;

    // Dimensions of the perlin noise
    public int perlinWidth = 256;
    public int perlinHeight = 256;

    // Smoothness of the perlin noise (0 - smooth, 1 - rough)
    // Gradual variation between noise values
    public float smoothness;

    // Tilling dimension for perlin noise
    // This allows perlin noise to seamlessly repeat itself when you move it
    public float offsetX;
    public float offsetY;

    // Frequency of the perlin noise
    // This is basically the scale of the noise
    public float xFrequency;
    public float yFrequency;

    public int seed;

    // 2D array to store the height of each coord
    float[,] heights;

    Terrain terrain;

    void Start()
    {
        terrain = GetComponent<Terrain>();
    }

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Space))
        {
            heights = GenerateHeights();

            terrain.terrainData = GenerateTerrain(terrain.terrainData);
        }
    }


    // This function takes in the current terrainData and process it 
    // to return a new terrainData with perlin noise applied
    TerrainData GenerateTerrain(TerrainData terrainData){
        // TODO: Heightmap resolution is always a value of power of 2 plus 1. This is based on perlin width or perlin height


        //terrainData.heightmapResolution =(int) (Mathf.Pow(perlinWidth, 2f) + 1f);
        terrainData.heightmapResolution = perlinWidth + 1;
        terrainData.size = new Vector3(terrainWidth, heightScale, terrainHeight);

        // TODO: Generate the height of perlin coord
        // See TerrainData SetHeights() function in Unity docs

        float[,] heights = GenerateHeights();
        terrainData.SetHeights(0, 0, heights);

        
        return terrainData;
    }

    private float[,] GenerateHeights(){
        heights = new float[perlinWidth, perlinHeight];

        // TODO: Iterate through the perlin coords and calculate height
        // Store generated height to heights 2D array
        for (int i = 0; i < perlinWidth; i++)
        {
            for (int j = 0; j < perlinHeight; j++)
            {
                heights[i, j] = CalculateHeight(i, j);
            }
        }


        return heights;
    }

    private float CalculateHeight(int x, int y){
        float height = 0f;

        Random.InitState(seed);

        offsetX = Random.value;
        offsetY = Random.value;

        // TODO: Calculate the height for a coord using perlin noise
        // To calculate the perlin coord, you will need to multiply the input coord (x, y) with its frequency
        // and add the offset.
        float xPerlinCoord = (x * xFrequency) + offsetX;
        float yPerlinCoord = (y * yFrequency) + offsetY;

        // TODO: Use the Mathf.PerlinNoise() function multiplied by smoothness.
        height = Mathf.PerlinNoise(xPerlinCoord, yPerlinCoord);

        return height;
    }
}

