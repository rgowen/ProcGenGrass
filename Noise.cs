using UnityEngine;
using System.Collections;

public static class Noise
{
    public static float[,] GenerateNoiseMapArray(int width, int height, float scale, float threshold)
    {
        float[,] noiseMap = new float[width, height];
        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        int randOffset = Random.Range(0, 1000);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                float sampleX = x / scale;
                float sampleY = y / scale;
                float perlinValue = Mathf.PerlinNoise(sampleX + randOffset, sampleY + randOffset);

                //Debug.Log(perlinValue);
                /*
                // separate into 3 values
                float thresholdValue;

                if (perlinValue < .6f) thresholdValue = 0;
                else if (perlinValue > .66f) thresholdValue = 1;
                else thresholdValue = .5f; */
                noiseMap[x, y] = perlinValue;
            }
        }
        return noiseMap;
    }
    // DEPRECATED?
    public static Texture2D GenerateNoiseMapTexture(int width, int height, float scale, float threshold)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        float[,] noiseMap = new float[width, height];
        if (scale <= 0)
        {
            scale = 0.0001f;
        }
        int randOffset = Random.Range(0, 1000);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                
                float sampleX = x / scale;
                float sampleY = y / scale;
                float perlinValue = Mathf.PerlinNoise(sampleX + randOffset, sampleY + randOffset);
                float thresholdValue;
                if (perlinValue <= threshold) thresholdValue = 0;
                else thresholdValue = 1;
                noiseMap[x, y] = thresholdValue;
            }
        }
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
        return texture;
    }
}