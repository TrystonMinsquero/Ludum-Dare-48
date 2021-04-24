﻿using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public Texture2D map;

    public ColorToPrefab[] colorMappings;

    void Start()
    {
        //GenerateLevel();
    }
    void GenerateLevel(Texture2D segment)
    {
        map = segment;
        for(int x = 0; x < map.width;x++)
        {
            for(int y = 0; y < map.height;y++)
            {
                GenerateTile(x, y);
            }
        }
    }
    void GenerateTile(int x, int y)
    {
        Color pixelColor = map.GetPixel(x, y);

        if(pixelColor.a == 0)
        { return; }
        
        foreach(ColorToPrefab colorMapping in colorMappings)
        {
            if(colorMapping.color.Equals(pixelColor))
            {
                Vector2 position = new Vector2(x, y);
                Instantiate(colorMapping.prefab, position, Quaternion.identity,transform);
            }
        }
    }

}
