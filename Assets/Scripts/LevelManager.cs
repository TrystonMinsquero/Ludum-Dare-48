using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float levelSpeed = 5f;
    public float camTravelTime = 2f;
    public float sectionDistance = 200f;

    static float CAM_TRAVEL_TIME;

    public static LevelSection levelSection = LevelSection.GROUND;
    public static GameObject cam;
    public static int camWidth = 10;
    public static int camHeight = 16;

    public GameObject cameraPositions;
    public CameraPosition camPosition = CameraPosition.INITIAL;
    public static CameraPosition CAM_POSITION;
    public static Transform[] camPositions;

    public static float distanceTraveled;
    public static bool transition;

    Rigidbody2D rb;

    public void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;

        cam = GameObject.Find("Main Camera");
        camPositions = sortCamPositions(cameraPositions.GetComponentsInChildren<Transform>());



        CAM_TRAVEL_TIME = camTravelTime;
    }

    private Transform[] sortCamPositions(Transform[] positions)
    {
        Transform[] temp = new Transform[positions.Length];
        foreach(Transform position in positions)
        {
            switch (position.name)
            {
                case "Initial":
                    temp[(int)CameraPosition.INITIAL] = position;
                    break;
                case "Shop":
                    temp[(int)CameraPosition.SHOP] = position;
                    break;
                case "Game":
                    temp[(int)CameraPosition.GAME] = position;
                    break;
            }
        }

        return temp;
    }

    public void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
        cam.transform.position = camPositions[(int)camPosition].position;
    }

    public void FixedUpdate()
    {
        if(levelSection != LevelSection.GROUND && levelSection != LevelSection.BOTTOM)
            rb.velocity = new Vector2(0, levelSpeed);



        distanceTraveled = rb.position.y;

        if(distanceTraveled >= sectionDistance && levelSection < LevelSection.MANTLE)
        {
            transition = true;
            NextLevelSection();
        }
    }

    public static void ChangeLevelSection(LevelSection section)
    {
        levelSection = section;

        //Do Stuff
        //switch (section) { }
    }

    public static void NextLevelSection()
    {
        levelSection++;

        switch(levelSection)
        {
            case LevelSection.CRUST:
                ChangeCameraPosition(CameraPosition.GAME);
                break;
        }
    }

    public static void ChangeCameraPosition(CameraPosition cameraPosition)
    {

        cam.transform.position = Vector3.Lerp(cam.transform.position, camPositions[(int)cameraPosition].position, CAM_TRAVEL_TIME);
        CAM_POSITION = cameraPosition;

    }

    

}

public enum CameraPosition
{
    INITIAL,
    SHOP,
    GAME

}
