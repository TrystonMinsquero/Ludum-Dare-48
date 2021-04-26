using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfGame : MonoBehaviour
{
    public void playAgain()
    {
        SceneManager.LoadScene(0);
    }
    public void exitGame()
    {
        Application.Quit();
    }
}
