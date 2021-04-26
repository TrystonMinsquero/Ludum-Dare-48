﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;

    [Header("Modifers")]
    public int numOfDifficulties = 4;
    public int _creditPlaceDistance = 130;
    public static float rareChance = .2f;

    [Header("Prefabs")]
    public GameObject obstacle_Prefab;
    public GameObject gem_Prefab;
    public GameObject credits_Prefab;
    public GameObject transition_Collider_Prefab;
    public GameObject transition_Tile_Collider_Prefab;

    [Header("Deletion Points")]
    public Transform[] clearTilesPoints = new Transform[2];
    public static GameObject groundFolder;
    public static GameObject obstacleFolder;

    private static float creditsPlaceDistance;
    private static GameObject obstaclePrefab;
    private static GameObject gemPrefab;
    private static GameObject creditsPrefab;
    private static GameObject transitionColliderPrefab;
    private static GameObject transitionTileColliderPrefab;

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
    private bool generating;
    private static bool transition;
    public static LevelSection levelSection;

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
        obstacleFolder = GameObject.Find("Obstacles");
        groundFolder = GameObject.Find("Ground Objects");
        obstaclePrefab = obstacle_Prefab;
        gemPrefab = gem_Prefab;
        creditsPrefab = credits_Prefab;
        tileMaps = tile_Maps;
        transitionColliderPrefab = transition_Collider_Prefab;
        transitionTileColliderPrefab = transition_Tile_Collider_Prefab;
        creditsPlaceDistance = _creditPlaceDistance;


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

        for (int i = (int)LevelSection.GROUND; i < (int)LevelSection.VOID; i++)
        {
            obstacleTiles.Add(null);
            environmentTiles.Add(null);
        }
        for (int i = (int)LevelSection.GROUND; i < (int)LevelSection.VOID / 2; i++)
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
        generating = true;
        transition = false;
        levelSection = LevelSection.GROUND;
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
                if(groundFolder != null)
                {
                    Destroy(groundFolder);
                    groundFolder = null;
                }
            ClearRowOfTiles(clearTilesPoints[0].position, clearTilesPoints[1].position);

        }

        if (generating)
        {
            if (LevelManager.distanceTraveled - sectionsGenerated * 10 > 0 && LevelManager.levelSection < LevelSection.VOID && !transition)
            {
                GenerateSegment((int)UnityEngine.Random.Range(.9f, LevelManager.maxDifficulty + 1));
            }
            else if (LevelManager.levelSection == LevelSection.VOID && generating)
            {
                GenerateCredits();
                generating = false;
            }
        }



    }

    public static void GenerateSegment(int difficulty, bool _transition = false)
    {
        transition = _transition;
        //Debug.Log("Distance To Place = " + distanceToPlace);
        GenerateBackground(backgroundMaps[(int)UnityEngine.Random.Range(0, backgroundMaps.Length)]);
        GenerateFeedTape(foregroundMaps[(int)UnityEngine.Random.Range(0, foregroundMaps.Length)]);
        //(int)UnityEngine.Random.Range(0, obstacleMaps[difficulty].Length)
        if(transition)
            GenerateForeground(transitionMaps[0]);
        else
            GenerateForeground(obstacleMaps[difficulty][(int)UnityEngine.Random.Range(0, obstacleMaps[difficulty].Length)]);
        transition = false;
        sectionsGenerated++;
    }

    public static void GenerateTransitionSegment()
    {
        GenerateSegment(0, true);
        sectionsGenerated--;
        levelSection++;
    }


    private static void GenerateForeground(Texture2D segment)
    {
        //Debug.Log("Generating Obstacles from: " + segment.name);
        for (int x = 0; x < segment.width; x++)
        {
            for (int y = 0; y < segment.height; y++)
            {
                GenerateTile(segment, x, y, TileMapType.OBSTACLES);
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
                GenerateTile(segment, x, y, TileMapType.FOREGROUND);
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
        if (pixelColor.a == 0 || levelSection >= LevelSection.VOID)
        { return; }

        Tilemap tileMap = tileMaps[(int)tileMapType];

        Vector2 pos = tileMapType == TileMapType.OBSTACLES && !transition ? 
            (Vector2)(LevelManager.cam.transform.position) + Vector2.up * 4.5f + Vector2.left * 5.5f
            : (Vector2)(LevelManager.cam.transform.position) + Vector2.up * 4.5f + Vector2.left * 8.5f;

        pos.y -= distanceToPlace;
        pos.x += x;
        pos.y += y;
        //if ((x == 0 || y == 0) && x == y )
            //Debug.Log(Vector2.one * pos);
        Vector3Int gridPos = tileMap.WorldToCell(pos);


        ColorUtility.ToHtmlStringRGB(pixelColor);
        int tileIndex = int.Parse(ColorUtility.ToHtmlStringRGB(pixelColor), System.Globalization.NumberStyles.HexNumber);

        if (tileMapType == TileMapType.OBSTACLES)
        {

            if (!transition)
            {
                //coins have a decimal of 10879231
                if (tileIndex > obstacleTiles[(int)levelSection].Length && tileIndex != 10879231)
                    Debug.LogError("Error for " + map.name + " at " + "(" + x + ", " + y + ") = " + tileIndex);
                else
                {
                    if (tileIndex == 10879231)
                    {
                        GemScript.createGem(gemPrefab, pos, (UnityEngine.Random.Range(0, 100) < rareChance * 100), obstacleFolder.transform);
                    }
                    else
                    {
                        tileMap.SetTile(gridPos, obstacleTiles[(int)levelSection][tileIndex]);
                        Instantiate(obstaclePrefab, obstacleFolder.transform).transform.position = pos;
                    }
                }
            }
            else
            {
                //Debug.Log("Transition Tile (" + x + ", " + y + ") is at " + gridPos + " and has an index of " + tileIndex);
                int transitionIndex = GetTransitionIndex();
                //Debug.Log("Tile for " + map.name + " at " + "(" + x + ", " + y + ") = " + transitionTiles[transitionIndex][tileIndex]);
                if (tileIndex > transitionTiles[transitionIndex].Length)
                    Debug.LogError("Error for " + map.name + " at " + "(" + x + ", " + y + ") = " + tileIndex);
                else
                {
                    tileMap.SetTile(gridPos, transitionTiles[transitionIndex][tileIndex]);
                    if(x == 0)
                        Instantiate(transitionTileColliderPrefab, obstacleFolder.transform).transform.position = pos;
                }
            }

        }
        else
        {
            if (tileIndex > environmentTiles[(int)levelSection].Length)
                Debug.LogError("Error for " + map.name + " at " + "(" + x + ", " + y + ") = " + tileIndex);
            else
                tileMap.SetTile(gridPos, environmentTiles[(int)levelSection][tileIndex]);
        }

    }

    public static void GenerateCredits()
    {
        Vector3 pos = (Vector2)(LevelManager.cam.transform.position) + Vector2.down * 5;
        pos.y -= distanceToPlace;
        pos.y -= creditsPlaceDistance;
        Instantiate(creditsPrefab, obstacleFolder.transform).transform.position = pos;
        RectTransform canvas = GameObject.Find("Credits(Clone)").GetComponent<RectTransform>();
        pos.y += (canvas.rect.height * canvas.localScale.y) / 2 + 5;
        Instantiate(transitionColliderPrefab, obstacleFolder.transform).transform.position = pos;

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

    private static int GetTransitionIndex()
    {
        switch (levelSection)
        {
            case LevelSection.CRUST:
                return 0;
            case LevelSection.MANTLE:
                return 1;
                //when core transition tile added
            case LevelSection.CORE:
                return 1;
                
        }
        Debug.LogError("can't transition at level section " + levelSection);
        return -1;
    }

    public enum TileMapType
    {
        OBSTACLES,
        FOREGROUND,
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




