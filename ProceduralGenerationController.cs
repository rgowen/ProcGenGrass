using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProceduralGenerationController : MonoBehaviour {
    public Vector2 mapSize;
    public bool generateItems;
    public bool generateOnStart; 
    public bool generateTiles; // generate base tile map
    private List<GameObject> generatedItems;
    public Sprite[] highDensitySprites;
    public Sprite[] lowDensitySprites;
    public Sprite baseTile;
    public RawImage uiCurrentMap; // reference to the UI raw image to draw the noise map to
    void Start()
    {
        // Generate the base tileset
        if (generateTiles)
        {
            for (int x = 0; x < mapSize.x; x += 2)
            {
                for (int y = 0; y < mapSize.y; y += 2)
                {
                    GameObject tile = GenerateSprite(baseTile, new Vector2(x, y), "Tileset", false);
                    tile.name = x.ToString() + " " + y.ToString();
                    tile.transform.parent = gameObject.transform;
                }
            }
        }
        generatedItems = new List<GameObject>();
        if (generateOnStart) 
        {
            GenerateFromNoiseMap();
        }
    }
    // Generate Items from a noisemap. Places a random cosmetic at half-unit intervals based on the float value at that position in the noisemap. 0 = empty, 1 = place cosmetic
    public void GenerateFromNoiseMap()
    {
        Clear();
        float[,] noiseMap = Noise.GenerateNoiseMapArray((int)mapSize.x * 4, (int)mapSize.y * 4, 35, .597f);
        float posX = 0, posY = 0;
        for (int x = 0; x < noiseMap.GetLength(0); x++)
        {
            for(int y = 0; y < noiseMap.GetLength(1); y++)
            {
                if(Random.Range(0.0f, 100.0f) > 40.0f) // chance of nothing at all to keep it interesting
                {
                    var value = noiseMap[x, y];
                    if (value > .9f) //high density?
                    {
                        int index = Random.Range(0, highDensitySprites.Length);
                        bool flipX = Random.value > 0.5f ? true : false;
                        generatedItems.Add(GenerateSprite(highDensitySprites[index], new Vector3(posX, posY, 0), "Environment Cosmetics", flipX));
                    }
                    else if (value > .8f) //low density?
                    {
                        int index = Random.Range(0, lowDensitySprites.Length);
                        bool flipX = Random.value > 0.5f ? true : false;
                        generatedItems.Add(GenerateSprite(lowDensitySprites[index], new Vector3(posX, posY, 0), "Environment Cosmetics", flipX));
                    }
                    else // random chance to generate anyway
                    {
                        if (Random.Range(0.0f, 100.0f) < .5f)
                        {
                            int index = Random.Range(0, lowDensitySprites.Length);
                            bool flipX = Random.value > 0.5f ? true : false;
                            generatedItems.Add(GenerateSprite(lowDensitySprites[index], new Vector3(posX, posY, 0), "Environment Cosmetics", flipX));
                        }
                        
                    }
                }
                posY += .25f;
            }
            posY = 0;
            posX += .25f;
        }
        SetUINoiseMap(noiseMap.GetLength(0), noiseMap.GetLength(1), noiseMap);
    }
    // 
    void SetUINoiseMap(int width, int height, float[,] noiseMap)
    {
        Texture2D texture = new Texture2D(width, height);
        //texture.filterMode = FilterMode.Point;
        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();
        uiCurrentMap.texture = texture;
    }
    // Generate new gameobject with sprite
    GameObject GenerateSprite(Sprite sprite, Vector3 pos, string sortingLayer, bool flipX)
    {
        GameObject newObj = new GameObject(sprite.name + " " + pos.ToString());
        SpriteRenderer spriteRenderer = newObj.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingLayerName = sortingLayer;
        spriteRenderer.flipX = flipX;
        newObj.transform.position = pos;
        return newObj;
    }
    // Clear ALL generated objects in scene
    public void Clear()
    {
        if(generatedItems.Count > 0)
        {
            foreach(GameObject obj in generatedItems)
            {
                Destroy(obj);
            }
            generatedItems.Clear();
        }
    }
    // Generate new random position based on limits, Z is ALWAYS zero
    Vector3 RandomFloatPosition(Vector2 limit)
    {
        float x = Random.Range(0, Mathf.Abs(limit.x));
        float y = Random.Range(0, Mathf.Abs(limit.y));
        return new Vector3(x, y, 0);
    }
    // Generate new random position based on limits, Z is ALWAYS zero, CLAMPED TO INT
    Vector3 RandomIntPosition(Vector2 limit)
    {
        int x = Random.Range(0, Mathf.Abs((int)limit.x));
        int y = Random.Range(0, Mathf.Abs((int)limit.y));
        return new Vector3(x, y, 0);
    }
    void SeedRandom()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
    }
}
