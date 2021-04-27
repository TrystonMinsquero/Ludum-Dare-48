using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float level_Speed = 5f;
    public float cam_Travel_Time = 2f;
    public float _sectionDistance = 200f;
    public static GameObject player;

    private static float camTravelTime;

    public static LevelSection levelSection;
    public static GameObject cam;
    public static int camWidth = 10;
    public static int camHeight = 16;
    public static GameObject walls;

    public GameObject cameraPositions;
    public CameraPosition camPosition;
    public static CameraPosition CAM_POSITION;
    public static Transform[] camPositions;

    public static float distanceTraveled;
    public static int maxDifficulty;
    public static float levelSpeed;
    public static float sectionDistance;

    public static bool falling;
    public static bool timeSlowed;

    Rigidbody2D rb;

    public void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;

        camTravelTime = cam_Travel_Time;
        levelSpeed = level_Speed;

        player = GameObject.Find("Travis");
        cam = GameObject.Find("Main Camera");
        walls = GameObject.Find("Walls");
        walls.SetActive(false);
        camPositions = sortCamPositions(cameraPositions.GetComponentsInChildren<Transform>());
        levelSection = LevelSection.GROUND;
        camPosition = CameraPosition.INITIAL;
        maxDifficulty = 1;
        falling = false;
        sectionDistance = _sectionDistance;


        rb = this.GetComponent<Rigidbody2D>();
        cam.transform.position = camPositions[(int)camPosition].position;

        cam.AddComponent<Rigidbody2D>();
        cam.GetComponent<Rigidbody2D>().gravityScale = 0;
        cam.GetComponent<Rigidbody2D>().isKinematic = true;

    }

    public void Start()
    {
        
        distanceTraveled = 0;
        ChangeCameraPosition(CameraPosition.INITIAL);
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
                case "Shop Outside":
                    temp[(int)CameraPosition.SHOP_OUTSIDE] = position;
                    break;
            }
        }

        return temp;
    }

    public void FixedUpdate()
    {
        if(levelSection != LevelSection.GROUND && levelSection != LevelSection.VOID)
            rb.velocity = new Vector2(0, levelSpeed);



        distanceTraveled = rb.position.y;

        if (LevelGenerator.sectionsGenerated * 10 >= sectionDistance && LevelGenerator.levelSection + 1 == LevelSection.MANTLE ||
            LevelGenerator.sectionsGenerated * 10 >= sectionDistance * 2 && LevelGenerator.levelSection + 1 == LevelSection.CORE ||
            LevelGenerator.sectionsGenerated * 10 >= sectionDistance * 3 && LevelGenerator.levelSection + 1 == LevelSection.VOID)
        {
            Debug.Log("Generating Transition from " + LevelGenerator.levelSection + " to " + LevelGenerator.levelSection + 1);
            //Debug.Log("Level Generator Section is " + LevelGenerator.levelSection);
            //Debug.Log("Level Manager Section is " + levelSection);
            LevelGenerator.GenerateTransitionSegment();
        }
    }

    public static void NextLevelSection()
    {
        
        levelSection++;
        LevelGenerator.levelSection = levelSection;
        Debug.Log("Level chaninging to " + levelSection);
        switch (levelSection)
        {
            case LevelSection.CRUST:
                ChangeCameraPosition(CameraPosition.GAME);
                walls.SetActive(true);
                falling = true;
                Destroy(GameObject.Find("Temp Walls"));
                break;
            case LevelSection.MANTLE:
                maxDifficulty++;
                break;
            case LevelSection.CORE:
                maxDifficulty++;
                break;
            case LevelSection.VOID:
                break;
            case LevelSection.VICTORY:
                GameObject.Find("End Canvas").GetComponent<Canvas>().enabled = true;
                levelSpeed = levelSpeed / 2;
                break;
        }

        if (levelSection < LevelSection.VOID && (int)levelSection > DataControl.suitLevel + 1)
            player.GetComponent<Player>().StartBurnout();
    }

    public static void ChangeCameraPosition(CameraPosition cameraPosition)
    {
        switch (cameraPosition)
        {
            case CameraPosition.INITIAL:
                //ChangeCameraSize(startSize);
                cam.transform.position = camPositions[(int)CameraPosition.INITIAL].position;
                player.GetComponent<PlayerMovement>().controls.Enable();
                break;
            case CameraPosition.SHOP:
                //ChangeCameraSize(shopSize);
                cam.transform.position = camPositions[(int)CameraPosition.SHOP].position;
                player.GetComponent<PlayerMovement>().controls.Disable();
                break;
            case CameraPosition.GAME:
                //ChangeCameraSize(gameSize);
                cam.transform.position = camPositions[(int)CameraPosition.GAME].position;
                break;
            case CameraPosition.SHOP_OUTSIDE:
                cam.transform.position = camPositions[(int)CameraPosition.SHOP_OUTSIDE].position;
                player.GetComponent<PlayerMovement>().controls.Enable();
                break;
        }
        Vector3 vel = cam.GetComponent<Rigidbody2D>().velocity;
        cam.transform.position = Vector3.Lerp(cam.transform.position, camPositions[(int)cameraPosition].position, camTravelTime);
        CAM_POSITION = cameraPosition;

    }

    public static void ChangeCameraSize(Vector2 size)
    {
        cam.GetComponent<Camera>().orthographicSize = size.x;
        cam.GetComponent<Camera>().aspect = size.y / size.x;
    }

    public IEnumerator PlayThemeAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        AudioSource song = LevelManager.levelSection > LevelSection.MANTLE ? SoundManager.themes[(int)levelSection + 1] : SoundManager.themes[(int)levelSection];
        StartCoroutine(SoundManager.TransitionToSong(song));
    }

    public static void ResetScene()
    {
        SceneManager.LoadScene(0);
    }
}

public enum CameraPosition
{
    INITIAL,
    SHOP,
    GAME,
    SHOP_OUTSIDE

}
