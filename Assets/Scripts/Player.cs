using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{

    public AudioSource death;

    public IEnumerator DieAfterTime(float time)
    {
        LevelManager.transitionToSong(death);
        yield return new WaitForSeconds(time);
        while (LevelManager.currentSong.isPlaying)
            yield return null;
        Die();
    }


    public void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
