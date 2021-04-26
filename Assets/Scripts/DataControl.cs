using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataControl : MonoBehaviour
{
    public static DataControl Instance;

    public static int money;
    public int moneyP;

    public static bool bubbles;
    public bool bubblesP;

    public static int timeSlows;
    public int timeSlowsP;

    public static int suitLevel;
    public int suitLevelP;

    private void Start()
    {
        money = moneyP;
        bubbles = bubblesP;
        suitLevel = suitLevelP;
        timeSlows = timeSlowsP;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);

            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);

        }

    }


}
