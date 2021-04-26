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

    public Text defaultText;

    public Image suitMaxed;
    public Button upgradeSuit;
    public Text hoverSuitText;

    public Image bubbleMaxed;
    public Button upgradeBubble;
    public Text hoverBubbleText;

    public Text hoverTimeSlowText;


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
        if (DataControl.suitLevel >= 2)
        {
            suitMaxed.gameObject.SetActive(true);
            upgradeSuit.gameObject.SetActive(false);

        }
        if (DataControl.bubbles)
        {
            bubbleMaxed.gameObject.SetActive(true);
            upgradeBubble.gameObject.SetActive(false);
        }
    }
    public void buySuit()
    {
        if (DataControl.money >= suitCost && DataControl.suitLevel < 2)
        {
            SoundManager.playSound(SoundManager.buyItem);
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
    public void hoverSuit()
    {
        SoundManager.playSound(SoundManager.hoverItem);
        defaultText.gameObject.SetActive(false);
        hoverSuitText.gameObject.SetActive(true);
        Debug.Log("HOVERING SUIT!!");
    }
    public void unhoverSuit()
    {
        defaultText.gameObject.SetActive(true);
        hoverSuitText.gameObject.SetActive(false);
        Debug.Log("UNHOVERING SUIT!!");
    }
    public void buyBubble()
    {
        if (DataControl.money >= bubbleCost && !DataControl.bubbles)
        {
            SoundManager.playSound(SoundManager.buyItem);
            DataControl.money -= bubbleCost;
            DataControl.bubbles = true;

            LevelManager.player.GetComponent<PlayerMovement>().GiveBubble();

            Debug.Log(DataControl.bubbles + "\n" + DataControl.money);
        }
    }
    public void hoverBubble()
    {
        SoundManager.playSound(SoundManager.hoverItem);
        defaultText.gameObject.SetActive(false);
        hoverBubbleText.gameObject.SetActive(true);
        Debug.Log("HOVERING BUBBLE!!");
    }
    public void unhoverBubble()
    {
        defaultText.gameObject.SetActive(true);
        hoverBubbleText.gameObject.SetActive(false);
        Debug.Log("UNHOVERING BUBBLE!!");
    }
    public void buyTimeSlow()
    {
        if (DataControl.money >= timeSlowCost && DataControl.timeSlows < timeSlowMax)
        {
            SoundManager.playSound(SoundManager.buyItem);
            DataControl.money -= timeSlowCost;
            DataControl.timeSlows++;

            Debug.Log(DataControl.timeSlows + "\n" + DataControl.money);
        }
    }
    public void hoverTimeSlow()
    {
        SoundManager.playSound(SoundManager.hoverItem);
        defaultText.gameObject.SetActive(false);
        hoverTimeSlowText.gameObject.SetActive(true);
        Debug.Log("HOVERING TIME CHARGE!!");
    }
    public void unhoverTimeSlow()
    {
        defaultText.gameObject.SetActive(true);
        hoverTimeSlowText.gameObject.SetActive(false);
        Debug.Log("UNHOVERING TIME CHARGE!!");
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            shopUI.SetActive(true);
            defaultText.gameObject.SetActive(true);
            hoverSuitText.gameObject.SetActive(false);
            hoverBubbleText.gameObject.SetActive(false);
            hoverTimeSlowText.gameObject.SetActive(false);
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
