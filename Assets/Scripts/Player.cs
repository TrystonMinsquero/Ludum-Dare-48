using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{

    public AudioSource death;

    public IEnumerator DieAfterTime(float time)
    {
        Debug.Log("Death");
        SoundManager.TransitionToSong(death);
        yield return new WaitForSeconds(time);
        Debug.Log("stuck?");
        while (SoundManager.currentSong.isPlaying)
            yield return null;
        Die();
    }


    public void Die()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
