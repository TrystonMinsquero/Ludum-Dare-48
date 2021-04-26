using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{

    ParticleSystem smoke;
    BoxCollider2D bc;
    SpriteRenderer sr;
    SpriteRenderer bubble;

    public float fadeOutDuration = 2.5f;
    public float timeUntilFade = 3f;

    [System.NonSerialized]
    public bool hasBubble;
    [System.NonSerialized]
    public bool isDying;

    private void Start()
    {
        bc = this.GetComponent<BoxCollider2D>();
        sr = this.GetComponent<SpriteRenderer>();
        smoke = this.GetComponentInChildren<ParticleSystem>();
        foreach (SpriteRenderer renderer in this.GetComponentsInChildren<SpriteRenderer>())
            if (renderer != sr)
                bubble = renderer;


        smoke.Stop();
        bubble.enabled = false;
    }

    private void FixedUpdate()
    {
        smoke.gameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);
    }


    public IEnumerator DieAfterTime(float time)
    {
        StartCoroutine(SoundManager.playDeath());
        yield return new WaitForSeconds(time);
        while (!LevelManager.readyToDie)
            yield return null;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }


    public IEnumerator Die()
    {
        isDying = true;
        Debug.Log("Start dying music");
        StartCoroutine(SoundManager.playDeath());
        Debug.Log("Waiting for music to finish");
        while (!LevelManager.readyToDie)
            yield return null;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void CheckSuitLevel()
    {
        if ((int)LevelManager.levelSection > DataControl.suitLevel + 1)
            StartCoroutine(Burnout());
    }

    public void StartBurnout()
    {
        StartCoroutine(Burnout());
    }

    public IEnumerator Burnout()
    {
        StartCoroutine(SoundManager.TransitionToSong(null));
        isDying = true;
        Debug.Log("Start smoke");
        SoundManager.playSound(SoundManager.suitCatchFire);
        SoundManager.playSound(SoundManager.death);
        smoke.Play();
        for (float i = timeUntilFade; i >= 0; i -= Time.deltaTime)
            yield return null;
        bc.enabled = false;
        Debug.Log("Start fade");
        this.gameObject.GetComponent<PlayerMovement>().controls.Disable();
        for (float i = fadeOutDuration; i >= 0; i -= Time.deltaTime)
        {
            Color newAlpha = sr.color;
            newAlpha.a -= Time.deltaTime / fadeOutDuration;
            sr.color = newAlpha;
            yield return null;
        }
        Debug.Log("Start Final words");
        smoke.Stop();

        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }


    public void GiveBubble()
    {
        hasBubble = true;
        bubble.enabled = true;
    }

    public void PopBubble()
    {
        bubble.enabled = false;
        StartCoroutine(Flash(1.5f));
    }


    private IEnumerator Flash(float duration)
    {
        bc.enabled = false;
        for (float i = duration; i >= 0; i -= Time.fixedDeltaTime)
        {
            sr.enabled = !sr.enabled;
            yield return null;
        }
        sr.enabled = true;
        bc.enabled = true;
        hasBubble = false;

    }
}
