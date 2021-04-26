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
        numOfGems.text = DataControl.money.ToString();
        numOfCharges.text = DataControl.timeSlows.ToString();
    }
}
