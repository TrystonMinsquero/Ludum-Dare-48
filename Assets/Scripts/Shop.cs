using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
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
        shopUI.SetActive(false);
        Debug.Log(DataControl.bubbles);
    }
    private void Update()
    {
        if (DataControl.suitLevel >= 2)
        {
            suitMaxed.gameObject.SetActive(true);
            upgradeSuit.gameObject.SetActive(false);

        }
        
        
    }
    public void buySuit()
    {
        if (DataControl.money >= DataControl.suitCost && DataControl.suitLevel < 2)
        {
            SoundManager.playSound(SoundManager.buyItem);
            DataControl.money -= DataControl.suitCost;
            DataControl.suitLevel++;

            DataControl.suitCost = 100;

            Debug.Log(DataControl.suitLevel + "\n" + DataControl.money);
        }
        else
            SoundManager.playSound(SoundManager.notEnoughGems);
    }
    public void hoverSuit()
    {
        SoundManager.playSound(SoundManager.hoverItem);

        speechBubble.text = "Upgrade your suit to help you survive deeper depths!";
        //Debug.Log("HOVERING SUIT!!");
    }
    public void unhoverSuit()
    {
        //Debug.Log("UNHOVERING SUIT!!");
    }
    public void buyBubble()
    {
        if (DataControl.money >= DataControl.bubbleCost && !DataControl.bubbles)
        {
            SoundManager.playSound(SoundManager.buyItem);
            DataControl.money -= DataControl.bubbleCost;
            DataControl.bubbles = true;
            

            LevelManager.player.GetComponent<Player>().GiveBubble();

            upgradeBubble.gameObject.SetActive(false);
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
        //Debug.Log("HOVERING BUBBLE!!");
    }
    public void unhoverBubble()
    {
        bubbleAnim.Play("BubbleShop_0");
        //Debug.Log("UNHOVERING BUBBLE!!");
    }
    public void buyTimeSlow()
    {
        if (DataControl.money >= DataControl.timeSlowCost && DataControl.timeSlows < DataControl.timeSlowMax)
        {
            SoundManager.playSound(SoundManager.buyItem);
            DataControl.money -= DataControl.timeSlowCost;
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
        speechBubble.text = "Slow down the infinitely expanding dimmension of time...for only " + DataControl.timeSlowCost + " Gems!";
        //Debug.Log("HOVERING TIME CHARGE!!");
    }
    public void unhoverTimeSlow()
    {
        timeChargeAnim.Play("Time Charge Idle_0");
        //Debug.Log("UNHOVERING TIME CHARGE!!");
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
    


}
