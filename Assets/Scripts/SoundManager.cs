using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public float song_Change_Interval = 1f;
    public float Max_Volume = .1f;


    public static float timeUntilSongPlay = 1f;
    private static float maxVolume;
    public static float songChangeInterval;
    public GameObject themeParent;
    public GameObject soundParent;

    //Add all sound variables and their static counterpart

    public static AudioSource[] themes;
    public static AudioSource currentSong;

    public void Awake()
    {
        if (instance != null)
            Destroy(this);
        else
            instance = this;
    }

    private void Start()
    {
        maxVolume = Max_Volume;
        songChangeInterval = song_Change_Interval;

        //set up static variables

        themes = themeParent.GetComponentsInChildren<AudioSource>();
        


        foreach (AudioSource theme in themes)
            theme.volume = 0;


        StartCoroutine(TransitionToSong(themes[0]));
    }

    public static IEnumerator SlowTime(float duration, float slowFactor)
    {
        LevelManager.levelSpeed = LevelManager.levelSpeed * slowFactor;
        currentSong.pitch = .5f;
        LevelManager.timeSlowed = true;
        for (float i = duration; i > 0; i -= Time.deltaTime)
            yield return null;
        LevelManager.timeSlowed = false;
        LevelManager.levelSpeed = LevelManager.levelSpeed / slowFactor;
        currentSong.pitch = 1;

    }


    public static IEnumerator TransitionToSong(AudioSource song)
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

    /*
    public static IEnumerator playDeath()
    {
        // sound = death
        for (float i = songChangeInterval; i >= 0; i -= Time.deltaTime)
        {
            if (currentSong != null && currentSong.volume > 0)
                currentSong.volume -= Time.deltaTime * maxVolume;
            if (sound != null && sound.volume < maxVolume)
                sound.volume += Time.deltaTime * maxVolume;
            yield return null;
        }

        if (currentSong != null)
            currentSong.volume = 0;
        if (sound != null)
        {
            sound.volume = maxVolume;
        }

        currentSong = sound;
    }*/

    //Add Method to play each sound here

    /* for audiosource, need to make each one static
    public static void playsound()
    {
        sound.play();
    }
    */

    

}
