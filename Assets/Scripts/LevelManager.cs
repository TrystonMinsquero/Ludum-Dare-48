using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float levelSpeed = 5f;

    public static LevelSection levelSection = LevelSection.GROUND;

    float distanceTraveled;
    Rigidbody2D rb;

    public void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    public void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        if(levelSection != LevelSection.GROUND && levelSection != LevelSection.BOTTOM)
            rb.velocity = new Vector2(0, levelSpeed);
        distanceTraveled += rb.velocity.y;
    }

    public static void ChangeLevelSection(LevelSection section)
    {
        levelSection = section;

        //Do Stuff
        //switch (section) { }
    }

}
