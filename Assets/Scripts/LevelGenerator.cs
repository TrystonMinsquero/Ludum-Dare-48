using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;
    public int numOfDifficulties = 4;

    public static float rareChance = .2f;
    public GameObject obstacle_Folder;
    public GameObject obstacle_Prefab;
    public GameObject gem_Prefab;
    public Transform[] environmentPoints = new Transform[2];
    public Transform[] obstaclePoints = new Transform[2];
    public Transform[] beginningPoints = new Transform[2];

    public Transform environmentStartClear;
    public Transform obstacleStartClear;
    public Transform beginningStartClear;

    private static GameObject obstacleFolder;
    private static GameObject obstaclePrefab;
    private static GameObject gemPrefab;

    public static List<Texture2D[]> obstacleMaps;
    public static Texture2D[] foregroundMaps;
    public static Texture2D[] backgroundMaps;
    public static Texture2D[] transitionMaps;

    public static List<TileBase[]> obstacleTiles;
    public static List<TileBase[]> environmentTiles;
    public static List<TileBase[]> transitionTiles;

    public Tilemap[] tile_Maps;

    private static Tilemap[] tileMaps;

    public static int distanceToPlace = 8;
    public static int sectionsGenerated = 0;

    public static int transitionIndex = 0;

private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;

        obstacleMaps = new List<Texture2D[]>();
        obstacleTiles = new List<TileBase[]>();
        environmentTiles = new List<TileBase[]>();
        transitionTiles = new List<TileBase[]>();
        obstacleFolder = obstacle_Folder;
        obstaclePrefab = obstacle_Prefab;
        gemPrefab = gem_Prefab;
        tileMaps = tile_Maps;


        for (int i = 0; i < numOfDifficulties; i++)
        {
            obstacleMaps.Add(Resources.LoadAll<Texture2D>("Segments/Obstacles/Difficulty_" + i));
            //Debug.Log("Obstacle maps of diff " + i + " loaded " + obstacleMaps[i].Length + " maps");
        }

        foregroundMaps = Resources.LoadAll<Texture2D>("Segments/Foreground");
        //Debug.Log("Foreground maps loaded " + foregroundMaps.Length + " maps");
        backgroundMaps = Resources.LoadAll<Texture2D>("Segments/Background");
        //Debug.Log("Background maps loaded " + backgroundMaps.Length + " maps");
        transitionMaps = Resources.LoadAll<Texture2D>("Segments/Transitions");
        //Debug.Log("Transition maps loaded " + transitionMaps.Length + " maps");

        for (int i = (int)LevelSection.GROUND; i < (int)LevelSection.BOTTOM; i++)
        {
            obstacleTiles.Add(null);
            environmentTiles.Add(null);
        }
        for (int i = (int)LevelSection.GROUND; i < (int)LevelSection.BOTTOM / 2; i++)
        {
            transitionTiles.Add(null);
        }

        obstacleTiles[(int)LevelSection.GROUND] = Resources.LoadAll<TileBase>("TileMaps/Crust/Obstacles/Tiles");
        obstacleTiles[(int)LevelSection.CRUST] = Resources.LoadAll<TileBase>("TileMaps/Crust/Obstacles/Tiles");
        obstacleTiles[(int)LevelSection.MANTLE] = Resources.LoadAll<TileBase>("TileMaps/Mantle/Obstacles/Tiles");
        obstacleTiles[(int)LevelSection.CORE] = Resources.LoadAll<TileBase>("TileMaps/Core/Obstacles/Tiles");

        environmentTiles[(int)LevelSection.GROUND] = Resources.LoadAll<TileBase>("TileMaps/Crust/Environment/Tiles");
        environmentTiles[(int)LevelSection.CRUST] = Resources.LoadAll<TileBase>("TileMaps/Crust/Environment/Tiles");
        environmentTiles[(int)LevelSection.MANTLE] = Resources.LoadAll<TileBase>("TileMaps/Mantle/Environment/Tiles");
        environmentTiles[(int)LevelSection.CORE] = Resources.LoadAll<TileBase>("TileMaps/Core/Environment/Tiles");

        transitionTiles[0] = Resources.LoadAll<TileBase>("TileMaps/Crust/Transition/Tiles");
        transitionTiles[1] = Resources.LoadAll<TileBase>("TileMaps/Mantle/Transition/Tiles");

        foreach (TileBase[] tiles in obstacleTiles)
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

        foreach (TileBase[] tiles in transitionTiles)
        {
            if (tiles == null)
                continue;
            LevelGenerator.InsertionSort(tiles);
        }

        distanceToPlace = 8;
        sectionsGenerated = 0;
        transitionIndex = 0;

}

    void Start()
    {
        distanceToPlace += 8;
        GenerateSegment(0);
        distanceToPlace += 10;
        GenerateSegment(0);
        distanceToPlace += 10;
        distanceToPlace -= 11;
        sectionsGenerated--;
    }

    public void FixedUpdate()
    {
        Transform[] obstacles = obstacleFolder.GetComponentsInChildren<Transform>();
        
        if(obstacles.Length > 0)
            foreach(Transform obstacle in obstacles)
                if (obstacle != obstacleFolder.transform && obstacle.position.y > LevelManager.cam.transform.position.y + LevelManager.camHeight)
                {
                    Destroy(obstacle.gameObject);
                }

        if (LevelManager.falling)
        {
            if(LevelManager.distanceTraveled - LevelManager.camHeight <= 35)
                ClearRowOfTiles(beginningPoints[0].position, beginningPoints[1].position);
            ClearRowOfTiles(environmentPoints[0].position, environmentPoints[1].position);
            ClearRowOfTiles(obstaclePoints[0].position, obstaclePoints[1].position);

        }
        
        if (LevelManager.distanceTraveled - sectionsGenerated * 10 > 0)
        {
            GenerateSegment((int)UnityEngine.Random.Range(.9f, LevelManager.maxDifficulty + 1));
        }
    }

    public static void GenerateSegment(int difficulty)
    {
        //Debug.Log("Distance To Place = " + distanceToPlace);
        GenerateBackground(backgroundMaps[(int)UnityEngine.Random.Range(0, backgroundMaps.Length)]);
        GenerateFeedTape(foregroundMaps[(int)UnityEngine.Random.Range(0, foregroundMaps.Length)]);
        //(int)UnityEngine.Random.Range(0, obstacleMaps[difficulty].Length)
        if(LevelManager.transition)
            GenerateForeground(transitionMaps[0]);
        else
            GenerateForeground(obstacleMaps[difficulty][(int)UnityEngine.Random.Range(0, obstacleMaps[difficulty].Length)]);
        sectionsGenerated++;
        if (LevelManager.transition)
            LevelManager.transition = false;
    }

    private static void GenerateForeground(Texture2D segment)
    {
        //Debug.Log("Generating Obstacles from: " + segment.name);
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
        //Debug.Log("Generating Foreground from: " + segment.name);
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
        //Debug.Log("Generating Background from: " + segment.name);
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

        Vector2 pos = tileMapType == TileMapType.FOREGROUND && !LevelManager.transition ? 
            (Vector2)(LevelManager.cam.transform.position) + Vector2.up * 4.5f + Vector2.left * 5.5f
            : (Vector2)(LevelManager.cam.transform.position) + Vector2.up * 4.5f + Vector2.left * 8.5f;

        pos.y -= distanceToPlace;
        pos.x += x;
        pos.y += y;
        //if ((x == 0 || y == 0) && x == y )
            //Debug.Log(Vector2.one * pos);
        Vector3Int gridPos = tileMap.WorldToCell(pos);

        if (pixelColor.a == 0)
        { return; }

        ColorUtility.ToHtmlStringRGB(pixelColor);
        int tileIndex = int.Parse(ColorUtility.ToHtmlStringRGB(pixelColor), System.Globalization.NumberStyles.HexNumber);

        if (tileMapType == TileMapType.FOREGROUND)
        {
            
            if (!LevelManager.transition)
            {
                //coins have a decimal of 10879231
                if (tileIndex > obstacleTiles[(int)LevelManager.levelSection].Length && tileIndex != 10879231)
                    Debug.LogError("Error for " + map.name + " at " + "(" + x + ", " + y + ") = " + tileIndex);
                else
                {
                    if (tileIndex == 10879231)
                    {
                        GemScript.createGem(gemPrefab, pos, (UnityEngine.Random.Range(0, 100) < rareChance * 100), obstacleFolder.transform);
                    }
                    else
                    {
                        tileMap.SetTile(gridPos, obstacleTiles[(int)LevelManager.levelSection][tileIndex]);
                        Instantiate(obstaclePrefab, obstacleFolder.transform).transform.position = pos;
                    }
                }
                    
                    
            }
            else
            {
                SetTransitionIndex();
                if (tileIndex > transitionTiles[transitionIndex].Length)
                    Debug.LogError("Error for " + map.name + " at " + "(" + x + ", " + y + ") = " + tileIndex);
                else
                    tileMap.SetTile(gridPos, transitionTiles[transitionIndex][tileIndex]);
            }

        }
        else
        {
            int sectionIndex = LevelManager.transition ? (int)LevelManager.levelSection - 1 : (int)LevelManager.levelSection;
            if (tileIndex > environmentTiles[sectionIndex].Length)
                Debug.LogError("Environment (" + x + ", " + y + ") = " + tileIndex);
            else
                tileMap.SetTile(gridPos, environmentTiles[sectionIndex][tileIndex]);
        }

    }

    private static void ClearRowOfTiles(Vector2 startPosition, Vector2 endPosition)
    {
        int length = (int)Mathf.Round(endPosition.x - startPosition.x + 1);
        foreach(Tilemap tilemap in tileMaps)
        {
            for(int i = 0; i < length; i++)
            {
                Vector3Int gridPos = tilemap.WorldToCell(startPosition + Vector2.right * i);
                if (tilemap.HasTile(gridPos))
                    tilemap.SetTile(gridPos, null);

            }
        }
    }

    private static void SetTransitionIndex()
    {
        switch (LevelManager.levelSection)
        {
            case LevelSection.MANTLE:
                transitionIndex = 0;
                break;
            case LevelSection.CORE:
                transitionIndex = 1;
                break;
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




