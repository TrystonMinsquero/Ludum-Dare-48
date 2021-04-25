using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float drag = 1f;
    public float scale = 1.5f;
    public float gravity = 2f;


    Controls controls;
    Rigidbody2D rb;
    BoxCollider2D bc;


    bool facingRight;
    bool grounded = true;
    bool falling;
    bool onRightWall;
    bool onLeftWall;

    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.Enable();
        rb = this.GetComponent<Rigidbody2D>();
        bc = this.GetComponent<BoxCollider2D>();

        rb.drag = drag;
        rb.gravityScale = gravity;
        transform.localScale = Vector3.one * scale;
    }

    private void FixedUpdate()
    {

        Vector2 playerMovement = controls.Gameplay.Movement.ReadValue<Vector2>();

        if (falling)
        {
            rb.gravityScale = 0;
            float minPlayerHeight = LevelManager.cam.transform.position.y - 5 + bc.size.y / 2;
            float maxPlayerHeight = LevelManager.cam.transform.position.y + 5 - bc.size.y / 2;
            //Directional movement
            if (playerMovement != Vector2.zero)
            {
                //Check movement
                Vector2 vel = rb.velocity;

                //Check x velocity
                if (playerMovement.x != 0 && (playerMovement.x > 0 && !onRightWall || playerMovement.x < 0 && !onLeftWall))
                    vel.x = playerMovement.x * moveSpeed;

                //Check y velocity
                if (playerMovement.y != 0 && (transform.position.y < maxPlayerHeight && playerMovement.y <= 0 || transform.position.y > minPlayerHeight && playerMovement.y >= 0))
                    vel.y = playerMovement.y * moveSpeed;

                vel.y = playerMovement.y * moveSpeed;
                rb.velocity = vel;

                //Check facing
                if (playerMovement.x != 0)
                {
                    facingRight = playerMovement.x > 0 ? true : false;
                    Vector3 scaleTemp = Vector3.one * scale;
                    scaleTemp.x = facingRight ? scale : -scale;
                    transform.localScale = scaleTemp;
                }
            }

            if (transform.position.y > maxPlayerHeight)
                transform.position = new Vector2(transform.position.x, maxPlayerHeight);

            if (transform.position.y < minPlayerHeight)
                transform.position = new Vector2(transform.position.x, minPlayerHeight);
        }
        else
        {
            rb.gravityScale = gravity;
            if (playerMovement.x != 0)
            {
                //Check movement
                Vector2 vel = Vector2.zero;
                if (playerMovement.x > 0 && !onRightWall || playerMovement.x < 0 && !onLeftWall)
                    vel.x = playerMovement.x * moveSpeed;
                else
                    vel.x = 0;
                vel.y = rb.velocity.y;
                rb.velocity = vel;

                //Check facing
                facingRight = playerMovement.x > 0 ? true : false;
                Vector3 scaleTemp = Vector3.one * scale;
                scaleTemp.x = facingRight ? scale : -scale;
                transform.localScale = scaleTemp;

            }

            //Jump
            if (playerMovement.y > 0 && grounded)
            {
                //Debug.Log("Jump");
                grounded = false;
                rb.velocity = new Vector2(rb.velocity.x, moveSpeed * 2);
            }

        }

        onLeftWall = false;
        onRightWall = false;
    }

    private IEnumerator Flash(float duration, float stunInterval = .2f)
    {
        float timeUntilFlash = Time.time;
        for (float i = duration; i >= 0; i -= Time.deltaTime)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = !gameObject.GetComponent<SpriteRenderer>().enabled;
            yield return null;
        }
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            //Debug.Log(contact.collider);
            if (contact.collider.CompareTag("Ground"))
            {
                //Debug.Log("Ground!");
                grounded = true;
            }

            if (contact.collider.CompareTag("Right Wall"))
            {
                //Debug.Log("Right Wall!");
                onRightWall = true;
            }

            if (contact.collider.CompareTag("Left Wall"))
            {
                //Debug.Log("Left Wall!");
                onLeftWall = true;
            }

            if(contact.collider.CompareTag("Obstacle"))
            {
                Debug.Log("Die");
            }
        }
        
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            
            if (contact.collider.CompareTag("Right Wall"))
            {
                //Debug.Log("Right Wall!");
                onRightWall = true;
            }

            if (contact.collider.CompareTag("Left Wall"))
            {
                //Debug.Log("Left Wall!");
                onLeftWall = true;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.CompareTag("Transition"))
        {
            falling = true;
            grounded = false;

            LevelManager.NextLevelSection();
            LevelManager.walls.SetActive(true);
            LevelManager.falling = true;
            StartCoroutine(LevelManager.transitionToSong(LevelManager.themes[(int)LevelManager.levelSection]));
        }


    }
}
