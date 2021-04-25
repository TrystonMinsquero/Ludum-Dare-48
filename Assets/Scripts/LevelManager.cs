using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float levelSpeed = 5f;
    public float cam_Travel_Time = 2f;
    public float sectionDistance = 200f;
    public float Max_Volume = .1f;
    private float timeUntilSongPlay = 1f;
    public float song_Change_Interval = 1f;
    public GameObject themeParent;


    private static float camTravelTime;

    public static LevelSection levelSection = LevelSection.GROUND;
    public static GameObject cam;
    public static int camWidth = 10;
    public static int camHeight = 16;
    public static GameObject walls;

    public GameObject cameraPositions;
    public CameraPosition camPosition = CameraPosition.INITIAL;
    public static CameraPosition CAM_POSITION;
    public static Transform[] camPositions;

    public static float distanceTraveled;
    public static bool transition;
    public static int maxDifficulty = 1;
    private static float maxVolume;
    public static float songChangeInterval;

    public static AudioSource[] themes;
    public static AudioSource currentSong;

    Rigidbody2D rb;

    public void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;


        camTravelTime = cam_Travel_Time;
        maxVolume = Max_Volume;
        songChangeInterval = song_Change_Interval;


        cam = GameObject.Find("Main Camera");
        walls = GameObject.Find("Walls");
        walls.SetActive(false);
        camPositions = sortCamPositions(cameraPositions.GetComponentsInChildren<Transform>());

        themes = themeParent.GetComponentsInChildren<AudioSource>();

        foreach (AudioSource theme in themes)
            theme.volume = 0;


        rb = this.GetComponent<Rigidbody2D>();
        cam.transform.position = camPositions[(int)camPosition].position;

        StartCoroutine(transitionToSong(themes[0]));
    }

    public void Start()
    {

        timeUntilSongPlay = (LevelGenerator.distanceToPlace - 10) / levelSpeed;
        Debug.Log("distanceToPlace = " + LevelGenerator.distanceToPlace);
        Debug.Log("timeUntilSongPlay = " + timeUntilSongPlay);
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

    public void FixedUpdate()
    {
        if(levelSection != LevelSection.GROUND && levelSection != LevelSection.BOTTOM)
            rb.velocity = new Vector2(0, levelSpeed);



        distanceTraveled = rb.position.y;

        if(distanceTraveled >= sectionDistance && levelSection < LevelSection.MANTLE || distanceTraveled >= sectionDistance * 2 && levelSection < LevelSection.CORE)
        {
            transition = true;
            maxDifficulty++;
            NextLevelSection();
            StartCoroutine(ExecuteAfterTime(timeUntilSongPlay));
            
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

        cam.transform.position = Vector3.Lerp(cam.transform.position, camPositions[(int)cameraPosition].position, camTravelTime);
        CAM_POSITION = cameraPosition;

    }


    public static IEnumerator transitionToSong(AudioSource song)
    {
        if (currentSong != null)
            currentSong.loop = false;
        
        if(song != null)
        {
            song.loop = true;
            song.Play();
        }

        for (float i = songChangeInterval; i >= 0; i -= Time.deltaTime)
        {
            if (currentSong != null && currentSong.volume > 0)
                currentSong.volume -= Time.deltaTime * maxVolume;
            if (song != null && song.volume < maxVolume)
                song.volume += Time.deltaTime * maxVolume;
            yield return null;
        }
        if(currentSong != null)
        {
            currentSong.volume = 0;
            Debug.Log("Current Song Volume: " + currentSong.volume);
        }
        if (song != null)
        {
            song.volume = maxVolume;
            Debug.Log("New Song Volume: " + song.volume);
        }

        currentSong = song;
    }

    IEnumerator ExecuteAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(transitionToSong(themes[(int)levelSection]));
    }

}

public enum CameraPosition
{
    INITIAL,
    SHOP,
    GAME

}
