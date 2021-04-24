using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemScript : MonoBehaviour
{
    public int valueOfGem;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject);
        //add valueOfGem to players $$$
        DataControl.money += valueOfGem; 
    }
  
}
