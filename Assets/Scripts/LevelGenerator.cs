using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class LevelGenerator : MonoBehaviour
{
    public Texture2D map;

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
        //GenerateLevel();
    }

    void GenerateLevel()
    {
        
    }

    private void GenerateForeground(Texture2D segment)
    {
        map = segment;
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                GenerateTile(x, y);
            }
        }

    }

    private void GenerateFeedTape(Texture2D segment)
    {
        
    }

    private void GenerateBackground(Texture2D segment)
    {

    }

    void GenerateTile(int x, int y, TileMapType tilemap = TileMapType.FOREGROUND)
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
        
        /*Convert.ToInt32(pixelColor.)

        foreach(ColorToPrefab colorMapping in colorMappings)
        {
            switch(tilemap)
            {
                case TileMapType.FEEDTAPE:
                    if(colorMapping.color.)
            }
            if(colorMapping.color.Equals(pixelColor))
            {
                Vector2 position = new Vector2(x, y);
                Instantiate(colorMapping.prefab, position, Quaternion.identity,transform);
            }
        }*/
    }

}
