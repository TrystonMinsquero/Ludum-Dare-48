using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public float level_Speed = 5f;
    public float cam_Travel_Time = 2f;
    public float sectionDistance = 200f;
    public float Max_Volume = .1f;
    private float timeUntilSongPlay = 1f;
    public float song_Change_Interval = 1f;
    public GameObject themeParent;
    public static GameObject player;


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
    public static float levelSpeed;

    public static AudioSource[] themes;
    public static AudioSource currentSong;

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
        maxVolume = Max_Volume;
        songChangeInterval = song_Change_Interval;
        levelSpeed = level_Speed;

        player = GameObject.Find("Travis");
        cam = GameObject.Find("Main Camera");
        walls = GameObject.Find("Walls");
        walls.SetActive(false);
        camPositions = sortCamPositions(cameraPositions.GetComponentsInChildren<Transform>());

        themes = themeParent.GetComponentsInChildren<AudioSource>();

        foreach (AudioSource theme in themes)
            theme.volume = 0;


        rb = this.GetComponent<Rigidbody2D>();
        cam.transform.position = camPositions[(int)camPosition].position;

        cam.AddComponent<Rigidbody2D>();
        cam.GetComponent<Rigidbody2D>().gravityScale = 0;
        cam.GetComponent<Rigidbody2D>().isKinematic = true;

        StartCoroutine(transitionToSong(themes[0]));
    }

    public void Start()
    {

        timeUntilSongPlay = (LevelGenerator.distanceToPlace - 10) / levelSpeed;
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


        if ((int)levelSection > DataControl.suitLevel + 1)
            StartCoroutine(player.GetComponent<PlayerMovement>().Burnout());

        if (distanceTraveled >= sectionDistance && levelSection < LevelSection.MANTLE || distanceTraveled >= sectionDistance * 2 && levelSection < LevelSection.CORE)
        {
            transition = true;
            maxDifficulty++;
            NextLevelSection();
            StartCoroutine(PlayAfterTime(timeUntilSongPlay));
            
        }
    }

    public static void NextLevelSection()
    {
        levelSection++;
        switch(levelSection)
        {
            case LevelSection.CRUST:
                ChangeCameraPosition(CameraPosition.GAME);
                GameObject.Find("Temp Walls").SetActive(false);
                break;
        }
    }

    public static void ChangeCameraPosition(CameraPosition cameraPosition)
    {
        Vector3 vel = cam.GetComponent<Rigidbody2D>().velocity;
        cam.transform.position = Vector3.Lerp(cam.transform.position, camPositions[(int)cameraPosition].position, camTravelTime);
        CAM_POSITION = cameraPosition;

    }

    public static IEnumerator SlowTime(float duration, float slowFactor)
    {
        levelSpeed = levelSpeed * slowFactor;
        currentSong.pitch = .5f;
        timeSlowed = true;
        for (float i = duration; i > 0; i -= Time.deltaTime)
            yield return null;
        timeSlowed = false;
        levelSpeed = levelSpeed / slowFactor;
        currentSong.pitch = 1;

    }


    public static IEnumerator transitionToSong(AudioSource song)
    {
        if (currentSong != null)
            currentSong.loop = false;

        
        if(song != null)
        {
            if(song != themes[(int)LevelSection.MANTLE])
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
            currentSong.volume = 0;
        if (song != null)
        {
            song.volume = maxVolume;
            if(song == themes[(int)LevelSection.MANTLE])
            {
                while (song.isPlaying)
                {
                    yield return null;
                }
                song.volume = 0;
                song.loop = false;
                song = themes[(int)LevelSection.MANTLE + 1];
                song.volume = maxVolume;
                song.loop = true;
                song.Play();
            }
        }

        currentSong = song;
    }

    public static IEnumerator transitionToSound(AudioSource song)
    {
        if (currentSong != null)
            currentSong.loop = false;


        if (song != null)
        {
            if (song != themes[(int)LevelSection.MANTLE])
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
        
        if (currentSong != null)
            currentSong.volume = 0;
        if (song != null)
        {
            song.volume = maxVolume;
            if (song == themes[(int)LevelSection.MANTLE])
            {
                while (song.isPlaying)
                {
                    yield return null;
                }
                song.volume = 0;
                song.loop = false;
                song = themes[(int)LevelSection.MANTLE + 1];
                song.volume = maxVolume;
                song.loop = true;
                song.Play();
            }
        }

        currentSong = song;
    }

    IEnumerator PlayAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        AudioSource song = levelSection > LevelSection.MANTLE ? themes[(int)levelSection + 1] : themes[(int)levelSection];
        StartCoroutine(transitionToSong(song));
    }

}

public enum CameraPosition
{
    INITIAL,
    SHOP,
    GAME

}
