using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject shopUI;
    public Image suitMaxed;
    public Button upgradeSuit;
    public Image bubbleMaxed;

    

    void Start()
    {
        suitCost = suitCostP;
        bubbleCost = bubbleCostP;
        bubbleMax = bubbleMaxP;
        timeSlowCost = timeSlowCostP;
        timeSlowMax = timeSlowMaxP;

        shopUI.SetActive(false);

    }
    private void Update()
    {
        if(DataControl.suitLevel == 2)
        {
            suitMaxed.gameObject.SetActive(true);
            upgradeSuit.gameObject.SetActive(false);

        }
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

            
        }
    }
    public void buyTimeSlow()
    {
        if(DataControl.money >= timeSlowCost && DataControl.timeSlows < timeSlowMax)
        {
            DataControl.money -= timeSlowCost;
            DataControl.timeSlows++;

            
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        shopUI.SetActive(true);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        shopUI.SetActive(false);
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
