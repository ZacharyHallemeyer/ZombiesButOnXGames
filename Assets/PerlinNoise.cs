using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoise : MonoBehaviour
{
    public Renderer render;
    public int width, height, scale;
    public int xOffsetMin, xOffsetMax, yOffsetMin, yOffsetMax;
    public int xOffset, yOffset;

    private void Start()
    {
        xOffset = Random.Range(xOffsetMin, xOffsetMax);
        yOffset = Random.Range(yOffsetMin, yOffsetMax);
        //render.material.mainTexture = GenerateTexture();
    }

    private void Update()
    {
       render.material.mainTexture = GenerateTexture();
    }

    private Texture2D GenerateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private Color CalculateColor(int x, int y)
    {
        float xCoord = (float) x / width * scale + xOffset;
        float yCoord = (float) y / height * scale + yOffset;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);

        return new Color(sample, sample, sample);
    }

}
