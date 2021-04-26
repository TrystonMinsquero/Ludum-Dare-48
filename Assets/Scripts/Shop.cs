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

    public static float timeChargeDuration;
    public float time_Charge_Duration = 3f;
    public static float speedReduction;
    public float speed_Reduction = .5f;

    public GameObject shopUI;

    public Image suitMaxed;
    public Button upgradeSuit;

    public Image bubbleMaxed;
    public Button upgradeBubble;


    void Start()
    {
        suitCost = suitCostP;
        bubbleCost = bubbleCostP;
        bubbleMax = bubbleMaxP;
        timeSlowCost = timeSlowCostP;
        timeSlowMax = timeSlowMaxP;
        timeChargeDuration = time_Charge_Duration;
        speedReduction = speed_Reduction;

        shopUI.SetActive(false);

    }
    private void Update()
    {
        if(DataControl.suitLevel >= 2)
        {
            suitMaxed.gameObject.SetActive(true);
            upgradeSuit.gameObject.SetActive(false);

        }
        if(DataControl.bubbles >= bubbleMax)
        {
            bubbleMaxed.gameObject.SetActive(true);
            upgradeBubble.gameObject.SetActive(false);
        }
    }
    public void buySuit()
    {
        if(DataControl.money >= suitCost && DataControl.suitLevel < 2)
        {
            DataControl.money -= suitCost;
            DataControl.suitLevel++;

            suitCost = 10;

            Debug.Log(DataControl.suitLevel + "\n" + DataControl.money);
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

            Debug.Log(DataControl.bubbles+"\n"+DataControl.money);
        }
    }
    public void buyTimeSlow()
    {
        if(DataControl.money >= timeSlowCost && DataControl.timeSlows < timeSlowMax)
        {
            DataControl.money -= timeSlowCost;
            DataControl.timeSlows++;

            Debug.Log(DataControl.timeSlows + "\n" + DataControl.money);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            shopUI.SetActive(true);
            LevelManager.ChangeCameraPosition(CameraPosition.SHOP);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        shopUI.SetActive(false);
        LevelManager.ChangeCameraPosition(CameraPosition.INITIAL);
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
