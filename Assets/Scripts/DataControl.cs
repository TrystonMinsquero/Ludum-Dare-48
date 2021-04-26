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


    public static int suitCost;
    public int suitCostP;

    public static int bubbleCost;
    public int bubbleCostP;
    public static int bubbleMax;
    public int bubbleMaxP;

    public static int timeSlowCost;
    public int timeSlowCostP;
    public static int timeSlowMax;
    public int timeSlowMaxP;

    public static float timeChargeDuration;
    public float time_Charge_Duration = 3f;
    public static float speedReduction;
    public float speed_Reduction = .5f;

    private void Start()
    {
        money = moneyP;
        bubbles = bubblesP;
        suitLevel = suitLevelP;
        timeSlows = timeSlowsP;

        suitCost = suitCostP;
        bubbleCost = bubbleCostP;
        bubbleMax = bubbleMaxP;
        timeSlowCost = timeSlowCostP;
        timeSlowMax = timeSlowMaxP;
        timeChargeDuration = time_Charge_Duration;
        speedReduction = speed_Reduction;
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
