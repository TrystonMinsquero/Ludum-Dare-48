using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;

    public int numberOfDifficulties = 10;
    public GameObject obstacle_Folder;
    public GameObject obstacle_Prefab;

    private static GameObject obstacleFolder;
    private static GameObject obstaclePrefab;

    public static List<Texture2D[]> obstacleMaps;
    public static Texture2D[] foregroundMaps;
    public static Texture2D[] backgroundMaps;

    public static List<TileBase[]> obstacleTiles;
    public static List<TileBase[]> environmentTiles;

    public Tilemap[] tile_Maps;

    private static Tilemap[] tileMaps;

    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    void Start()
    {
        obstacleMaps = new List<Texture2D[]>();
        obstacleTiles = new List<TileBase[]>();
        environmentTiles = new List<TileBase[]>();
        obstacleFolder = obstacle_Folder;
        obstaclePrefab = obstacle_Prefab;
        tileMaps = tile_Maps;


        for( int i = 0; i < numberOfDifficulties; i++)
        {
            obstacleMaps.Add(Resources.LoadAll<Texture2D>("Segments/Obstacles/Difficulty_" + i));
            //Debug.Log("Obstacle maps of diff " + i + " loaded " + obstacleMaps[i].Length + " maps");
        }

        foregroundMaps = Resources.LoadAll<Texture2D>("Segments/Foreground");
        //Debug.Log("Foreground maps loaded " + foregroundMaps.Length + " maps");
        backgroundMaps = Resources.LoadAll<Texture2D>("Segments/Background");
        //Debug.Log("Background maps loaded " + backgroundMaps.Length + " maps");

        for(int i = (int)LevelSection.GROUND; i < (int)LevelSection.BOTTOM; i++)
        {
            obstacleTiles.Add(null);
            environmentTiles.Add(null);
        }
        obstacleTiles[(int)LevelSection.CRUST] = Resources.LoadAll<TileBase>("TileMaps/Crust/Obstacles/Tiles");
        //obstacleTiles[(int)LevelSection.MANTLE] = Resources.LoadAll<TileBase>("TileMaps/Mantle/Obstacles/Tiles");
        //obstacleTiles[(int)LevelSection.CORE] = Resources.LoadAll<TileBase>("TileMaps/Core/Obstacles/Tiles");

        environmentTiles[(int)LevelSection.CRUST] = Resources.LoadAll<TileBase>("TileMaps/Crust/Environment/Tiles");
        //environmentTiles[(int)LevelSection.MANTLE] = Resources.LoadAll<TileBase>("TileMaps/Mantle/Environment/Tiles");
        //environmentTiles[(int)LevelSection.CORE] = Resources.LoadAll<TileBase>("TileMaps/Core/Environment/Tiles");

        foreach(TileBase[] tiles in obstacleTiles)
        {
            if (tiles == null)
                continue;
            LevelGenerator.InsertionSort(tiles);
        }

        foreach (TileBase[] tiles in environmentTiles)
        {
            if (tiles == null)
                continue;
            LevelGenerator.InsertionSort(tiles);
        }

        GenerateSegment();
    }

    public static void GenerateSegment()
    {
        GenerateBackground(backgroundMaps[(int)UnityEngine.Random.Range(0, backgroundMaps.Length)]);
        GenerateFeedTape(foregroundMaps[(int)UnityEngine.Random.Range(0, foregroundMaps.Length)]);
        int difficulty = 1;
        //(int)UnityEngine.Random.Range(0, obstacleMaps[difficulty].Length)
        GenerateForeground(obstacleMaps[difficulty][4]);
    }

    private static void GenerateForeground(Texture2D segment)
    {
        Debug.Log("Generating Obstacles from: " + segment.name);
        for (int x = 0; x < segment.width; x++)
        {
            for (int y = 0; y < segment.height; y++)
            {
                GenerateTile(segment, x, y, TileMapType.FOREGROUND);
            }
        }

    }

    private static void GenerateFeedTape(Texture2D segment)
    {
        Debug.Log("Generating Foreground from: " + segment.name);
        for (int x = 0; x < segment.width; x++)
        {
            for (int y = 0; y < segment.height; y++)
            {
                GenerateTile(segment, x, y, TileMapType.FEEDTAPE);
            }
        }
    }

    private static void GenerateBackground(Texture2D segment)
    {
        Debug.Log("Generating Background from: " + segment.name);
        for (int x = 0; x < segment.width; x++)
        {
            for (int y = 0; y < segment.height; y++)
            {
                GenerateTile(segment, x, y, TileMapType.BACKGROUND);
            }
        }
    }

    public static void GenerateTile(Texture2D map, int x, int y, TileMapType tileMapType)
    {
        

        Color pixelColor = map.GetPixel(x, y);
        Tilemap tileMap = tileMaps[(int)tileMapType];

        Vector2 pos = tileMapType == TileMapType.FOREGROUND ? 
            (Vector2)(LevelManager.cam.transform.position) - Vector2.up * 4.5f - Vector2.left * 8.5f
            : (Vector2)(LevelManager.cam.transform.position) - Vector2.up * 4.5f - Vector2.left * 6.5f;

        pos -= Vector2.down * 20;
        pos.x += x;
        pos.y += y;
        Vector3Int gridPos = tileMap.WorldToCell(pos);

        if (pixelColor.a == 0)
        { return; }

        ColorUtility.ToHtmlStringRGB(pixelColor);
        int tileIndex = int.Parse(ColorUtility.ToHtmlStringRGB(pixelColor), System.Globalization.NumberStyles.HexNumber);

        if (tileMapType == TileMapType.FOREGROUND)
        {
            if(tileIndex > obstacleTiles[(int)LevelManager.levelSection + 1].Length)
                Debug.LogError("Obstacle (" + x + ", " + y + ") = " + tileIndex);
            tileMap.SetTile(gridPos, obstacleTiles[(int)LevelManager.levelSection+1][tileIndex]);
            Instantiate(obstaclePrefab, obstacleFolder.transform).transform.position = pos;
        }
        else
        {
            if (tileIndex > environmentTiles[(int)LevelManager.levelSection + 1].Length)
                Debug.LogError("Environment (" + x + ", " + y + ") = " + tileIndex);
            tileMap.SetTile(gridPos, environmentTiles[(int)LevelManager.levelSection+1][tileIndex]);
        }

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

    public enum TileMapType
    {
        FOREGROUND,
        FEEDTAPE,
        BACKGROUND
    }

    public static void InsertionSort(TileBase[] input)
    {

        for (int i = 0; i < input.Length; i++)
        {
            var item = input[i];
            var currentIndex = i;
            while (currentIndex > 0 && 
                int.Parse(input[currentIndex - 1].name.Substring(input[currentIndex - 1].name.IndexOf("_") + 1)) > int.Parse(item.name.Substring(item.name.IndexOf("_") + 1)))
            {
                input[currentIndex] = input[currentIndex - 1];
                currentIndex--;
            }

            input[currentIndex] = item;
        }
    }
}




