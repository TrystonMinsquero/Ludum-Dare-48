using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    public static Shop Instance;

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


    

    void Start()
    {
        suitCost = suitCostP;
        bubbleCost = bubbleCostP;
        bubbleMax = bubbleMaxP;
        timeSlowCost = timeSlowCostP;
        timeSlowMax = timeSlowMaxP;

    }
    public void buySuit()
    {
        if(DataControl.money >= suitCost && DataControl.suitLevel < 2)
        {
            DataControl.money -= suitCost;
            DataControl.suitLevel++;

            suitCost = 10; 
        }
        else
        {
            //play uh-oh noise (can't buy)
        }
    }
    public void buyBubble()
    {
        if(DataControl.money >= bubbleCost && DataControl.bubbles < bubbleMax)
        {
            DataControl.money -= bubbleCost;
            DataControl.bubbles++;

            bubbleCost += 5;
        }
    }
    public void buyTimeSlow()
    {
        if(DataControl.money >= timeSlowCost && DataControl.timeSlows < timeSlowMax)
        {
            DataControl.money -= timeSlowCost;
            DataControl.timeSlows++;

            timeSlowCost += 5;
        }
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
