using UnityEngine;

public class GemScript : MonoBehaviour
{
    bool rareGem;
    int gemValue;

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

    SpriteRenderer sr;

    public static void createGem(GameObject gemPrefab, Vector2 position, bool r, Transform parent)
    {
        GameObject gem = Instantiate(gemPrefab, parent);
        gem.GetComponent<GemScript>().rareGem = r;
        gem.transform.position = position;
        gem.GetComponent<GemScript>().SetColor();
        gem.GetComponent<GemScript>().SetValue();

    }

    public void SetColor()
    {
        switch (LevelManager.levelSection)
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
        this.GetComponent<SpriteRenderer>().color = gemColor;
        
    }

    public void SetValue()
    {
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            DataControl.money += gemValue;
            SoundManager.playSound(SoundManager.gemPickup);
            Destroy(gameObject);
        }
    }
  
}
