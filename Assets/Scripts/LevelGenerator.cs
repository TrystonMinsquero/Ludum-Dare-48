using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;

    public int maxDifficulty = 3;
    public float rareChance = .2f;
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

    public static int distanceToPlace = 5;
    public static int sectionsGenerated = 0;

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


        for( int i = 0; i <= maxDifficulty; i++)
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
        obstacleTiles[(int)LevelSection.GROUND] = Resources.LoadAll<TileBase>("TileMaps/Crust/Obstacles/Tiles");
        obstacleTiles[(int)LevelSection.CRUST] = Resources.LoadAll<TileBase>("TileMaps/Crust/Obstacles/Tiles");
        //obstacleTiles[(int)LevelSection.MANTLE] = Resources.LoadAll<TileBase>("TileMaps/Mantle/Obstacles/Tiles");
        //obstacleTiles[(int)LevelSection.CORE] = Resources.LoadAll<TileBase>("TileMaps/Core/Obstacles/Tiles");

        environmentTiles[(int)LevelSection.GROUND] = Resources.LoadAll<TileBase>("TileMaps/Crust/Environment/Tiles");
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

        GenerateSegment(0);
        distanceToPlace += 10;
        GenerateSegment(0);
        distanceToPlace += 10;
        GenerateSegment(1);
    }

    public void FixedUpdate()
    {
        Transform[] obstacles = obstacleFolder.GetComponentsInChildren<Transform>();
        if(obstacles.Length == 0)
            foreach(Transform obstacle in obstacles)
                if (obstacle.position.y > LevelManager.cam.transform.position.y + LevelManager.camHeight)
                    Destroy(obstacle.gameObject);
        
        if (LevelManager.distanceTraveled - sectionsGenerated * 10 > 0)
        {
            GenerateSegment((int)UnityEngine.Random.Range(0, maxDifficulty+1));
        }
    }

    public static void GenerateSegment(int difficulty)
    {
        GenerateBackground(backgroundMaps[(int)UnityEngine.Random.Range(0, backgroundMaps.Length)]);
        GenerateFeedTape(foregroundMaps[(int)UnityEngine.Random.Range(0, foregroundMaps.Length)]);
        //(int)UnityEngine.Random.Range(0, obstacleMaps[difficulty].Length)
        GenerateForeground(obstacleMaps[difficulty][(int)UnityEngine.Random.Range(0, obstacleMaps[difficulty].Length)]);
        sectionsGenerated++;
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
            (Vector2)(LevelManager.cam.transform.position) + Vector2.up * 4.5f + Vector2.left * 5.5f
            : (Vector2)(LevelManager.cam.transform.position) + Vector2.up * 4.5f + Vector2.left * 8.5f;

        pos.y -= distanceToPlace;
        pos.x += x;
        pos.y += y;
        Vector3Int gridPos = tileMap.WorldToCell(pos);

        if (pixelColor.a == 0)
        { return; }

        ColorUtility.ToHtmlStringRGB(pixelColor);
        int tileIndex = int.Parse(ColorUtility.ToHtmlStringRGB(pixelColor), System.Globalization.NumberStyles.HexNumber);

        if (tileMapType == TileMapType.FOREGROUND)
        {
            //Add coins
            if(tileIndex > obstacleTiles[(int)LevelManager.levelSection].Length)
                Debug.LogError("Obstacle (" + x + ", " + y + ") = " + tileIndex);
            else
                tileMap.SetTile(gridPos, obstacleTiles[(int)LevelManager.levelSection][tileIndex]);
            Instantiate(obstaclePrefab, obstacleFolder.transform).transform.position = pos;
        }
        else
        {
            if (tileIndex > environmentTiles[(int)LevelManager.levelSection].Length)
                Debug.LogError("Environment (" + x + ", " + y + ") = " + tileIndex);
            tileMap.SetTile(gridPos, environmentTiles[(int)LevelManager.levelSection][tileIndex]);
        }

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




