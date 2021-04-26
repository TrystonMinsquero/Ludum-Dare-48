using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemDisplay : MonoBehaviour
{
    public Text numOfGems;
    public Text numOfCharges;

    void Update()
    {
        numOfGems.text = "x"+DataControl.money.ToString();
        numOfCharges.text = "x" + DataControl.timeSlows.ToString();
    }
}
