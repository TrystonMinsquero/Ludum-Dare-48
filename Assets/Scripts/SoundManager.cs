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
    public static AudioSource death;
    public AudioSource _death;

    public static AudioSource buyItem;
    public AudioSource _buyItem;

    public static AudioSource gemPickup;
    public AudioSource _gemPickup;

    public static AudioSource hoverItem;
    public AudioSource _hoverItem;

    public static AudioSource jump;
    public AudioSource _jump;

    public static AudioSource notEnoughGems;
    public AudioSource _notEnoughGems;

    public static AudioSource splat;
    public AudioSource _splat;

    public static AudioSource suitCatchFire;
    public AudioSource _suitCatchFire;

    public static AudioSource useTimeDischarge;
    public AudioSource _useTimeDischarge;

    public static AudioSource fishVictory;
    public AudioSource _fishVictory;


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
        death = _death;
        buyItem = _buyItem;
        gemPickup = _gemPickup;
        hoverItem = _hoverItem;
        jump = _jump;
        notEnoughGems = _notEnoughGems;
        splat = _splat;
        suitCatchFire = _suitCatchFire;
        useTimeDischarge = _useTimeDischarge;
        fishVictory = _fishVictory;

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

        if (LevelManager.player.GetComponent<Player>().isDying)
            yield break;

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

    
    public static IEnumerator playDeath()
    {
        AudioSource sound = death;
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
        while (currentSong.isPlaying)
            yield return null;
        LevelManager.readyToDie = true;
    }

    //Add Method to play each sound here

    public static void playSound(AudioSource sound)
    {
        sound.Play();
    }

    

}
