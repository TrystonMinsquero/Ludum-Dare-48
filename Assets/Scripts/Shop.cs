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

    public Animator suitAnim;
    public Animator bubbleAnim;
    public Animator timeChargeAnim;

    public Text speechBubble;

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
        if (DataControl.suitLevel >= 2)
        {
            suitMaxed.gameObject.SetActive(true);
            upgradeSuit.gameObject.SetActive(false);

        }
        if (DataControl.bubbles)
        {
            //bubbleMaxed.gameObject.SetActive(true);
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
            SoundManager.playSound(SoundManager.notEnoughGems);
    }
    public void hoverSuit()
    {
        SoundManager.playSound(SoundManager.hoverItem);

        speechBubble.text = "Upgrade your suit to help you survive deeper depths!";
        Debug.Log("HOVERING SUIT!!");
    }
    public void unhoverSuit()
    {
        Debug.Log("UNHOVERING SUIT!!");
    }
    public void buyBubble()
    {
        if (DataControl.money >= bubbleCost && !DataControl.bubbles)
        {
            SoundManager.playSound(SoundManager.buyItem);
            DataControl.money -= bubbleCost;
            DataControl.bubbles = true;

            LevelManager.player.GetComponent<Player>().GiveBubble();

            Debug.Log(DataControl.bubbles + "\n" + DataControl.money);
        }
        else
            SoundManager.playSound(SoundManager.notEnoughGems);
    }
    public void hoverBubble()
    {
        SoundManager.playSound(SoundManager.hoverItem);
        bubbleAnim.Play("BubbleShop_1");
        speechBubble.text = "A bubble might help with obstacles!";
        Debug.Log("HOVERING BUBBLE!!");
    }
    public void unhoverBubble()
    {
        bubbleAnim.Play("BubbleShop_0");
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
        else
            SoundManager.playSound(SoundManager.notEnoughGems);
    }
    public void hoverTimeSlow()
    {
        SoundManager.playSound(SoundManager.hoverItem);
        timeChargeAnim.Play("Time Charge Idle_1");
        speechBubble.text = "Slow down the infinitely expanding dimmension of time...for only " + timeSlowCost + " Gems!";
        Debug.Log("HOVERING TIME CHARGE!!");
    }
    public void unhoverTimeSlow()
    {
        timeChargeAnim.Play("Time Charge Idle_0");
        Debug.Log("UNHOVERING TIME CHARGE!!");
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
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
