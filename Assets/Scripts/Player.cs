using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{

    public IEnumerator DieAfterTime(float time)
    {
        Debug.Log("Death");
        SoundManager.playDeath();
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
