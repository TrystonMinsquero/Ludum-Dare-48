using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class LevelGenerator : MonoBehaviour
{
    public int numberOfDifficulties = 10;
    public static List<Texture2D[]> obstacleMaps;
    public static Texture2D[] foregroundMaps;
    public static Texture2D[] backgroundMaps;
    public ColorToPrefab[] colorMappings;




    enum TileMapType
    {
        FOREGROUND,
        FEEDTAPE,
        BACKGROUND
    }

    public Tilemap[] tileMaps;

    void Start()
    {
        obstacleMaps = new List<Texture2D[]>();

        for( int i = 0; i < numberOfDifficulties; i++)
        {
            obstacleMaps.Add(Resources.LoadAll<Texture2D>("Segments/Obstacles/Difficulty_" + i));
        }

        foregroundMaps = Resources.LoadAll<Texture2D>("Segments/Foreground");
        backgroundMaps = Resources.LoadAll<Texture2D>("Segments/Background");
        //GenerateLevel();
    }

    void GenerateLevel()
    {
        
    }

    private void GenerateForeground(Texture2D segment)
    {
        for (int x = 0; x < segment.width; x++)
        {
            for (int y = 0; y < segment.height; y++)
            {
                GenerateTile(segment, x, y);
            }
        }

    }

    private void GenerateFeedTape(Texture2D segment)
    {
        
    }

    private void GenerateBackground(Texture2D segment)
    {

    }

    void GenerateTile(Texture2D map, int x, int y, TileMapType tilemap = TileMapType.FOREGROUND)
    {
        Color pixelColor = map.GetPixel(x, y);
        
        Vector2 pos = tilemap == TileMapType.FOREGROUND ? 
            (Vector2)(LevelManager.cam.transform.position) - Vector2.up * 4.5f - Vector2.left * 5.5f
            : (Vector2)(LevelManager.cam.transform.position) - Vector2.up * 4.5f - Vector2.left * 6.5f;

        pos -= Vector2.down * 20;
        pos.x += x;
        pos.y += y;
        Vector3Int worldPos = tileMaps[(int)tilemap].WorldToCell(pos);

        if (pixelColor.a == 0)
        { return; }

        ColorUtility.ToHtmlStringRGB(pixelColor);
        int tileIndex = int.Parse(ColorUtility.ToHtmlStringRGB(pixelColor), System.Globalization.NumberStyles.HexNumber);
        Debug.Log("(" + x + ", " + y + ") = " + tileIndex);

        /*
        foreach (ColorToPrefab colorMapping in colorMappings)
        {
            switch(tilemap)
            {
                case TileMapType.FEEDTAPE:
                    if(colorMapping.color)
            }
            if(colorMapping.color.Equals(pixelColor))
            {
                Vector2 position = new Vector2(x, y);
                Instantiate(colorMapping.prefab, position, Quaternion.identity,transform);
            }
        }
        */
    }

    public static string ToRGBHex(Color c)
    {
        return string.Format("#{0:X2}{1:X2}{2:X2}", ToByte(c.r), ToByte(c.g), ToByte(c.b));
    }

    private static byte ToByte(float f)
    {
        f = Mathf.Clamp01(f);
        return (byte)(f * 255);
    }

}
