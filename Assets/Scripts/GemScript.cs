using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemScript : MonoBehaviour
{
    private static bool rareGem;
    private int gemValue;

    private static GameObject Gem;
    public GameObject GemP;

    public int commonCrust;
    public int rareCrust;

    public int commonMantle;
    public int rareMantle;

    public int commonCore;
    public int rareCore;

    private Color gemColor;
    public Color commonCrustColor;
    public Color rareCrustColor;
    public Color commonMantleColor;
    public Color rareMantleColor;
    public Color commonCoreColor;
    public Color rareCoreColor;

    public static void createGem(Vector2 position, bool r)
    {
        rareGem = r;
        Instantiate(Gem, position, Quaternion.identity);
    }
    public void Start()
    {
        Gem = GemP;
        SpriteRenderer sr = Gem.GetComponent<SpriteRenderer>();
        

        switch(LevelManager.levelSection)
        {
            case LevelSection.GROUND:
            case LevelSection.CRUST:
                gemColor = rareGem ? rareCrustColor : commonCrustColor;   
                break;

            case LevelSection.MANTLE:
                gemColor = rareGem ? rareMantleColor : commonMantleColor;
                break;

            case LevelSection.CORE:
                gemColor = rareGem ? rareCoreColor : commonCoreColor;
                break;
        }
        sr.color = gemColor;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
        //add valueOfGem to players $$$
        switch (LevelManager.levelSection)
        {
            case LevelSection.GROUND:
            case LevelSection.CRUST:
                gemValue = rareGem ? rareCrust : commonCrust;
                break;

            case LevelSection.MANTLE:
                gemValue = rareGem ? rareMantle : commonMantle;
                break;

            case LevelSection.CORE:
                gemValue = rareGem ? rareCore : commonCore;
                break;
        }  
        DataControl.money += gemValue; 
    }
  
}
