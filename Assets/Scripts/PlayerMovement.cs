using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float drag = 1f;


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
    }

    private void FixedUpdate()
    {

        Vector2 playerMovement = controls.Gameplay.Movement.ReadValue<Vector2>();

        if (falling)
        {
            rb.gravityScale = 0;
            float minPlayerHeight = LevelManager.mainCamera.transform.position.y - 5 + bc.size.y / 2;
            float maxPlayerHeight = LevelManager.mainCamera.transform.position.y + 5 - bc.size.y / 2;
            //Directional movement
            if (playerMovement != Vector2.zero)
            {
                //Check movement
                Vector2 vel = rb.velocity;

                //Check x velocity
                if (playerMovement.x != 0 && (playerMovement.x > 0 && !onRightWall || playerMovement.x < 0 && !onLeftWall))
                    vel.x = playerMovement.x * moveSpeed;

                Debug.Log(bc.size);
                //Check y velocity
                if (playerMovement.y != 0 && (transform.position.y < maxPlayerHeight && playerMovement.y <= 0 || transform.position.y > minPlayerHeight && playerMovement.y >= 0))
                    vel.y = playerMovement.y * moveSpeed;

                vel.y = playerMovement.y * moveSpeed;
                rb.velocity = vel;

                //Check facing
                if (playerMovement.x != 0)
                {
                    facingRight = playerMovement.x > 0 ? true : false;
                    Vector3 scale = Vector3.one * 2;
                    scale.x = facingRight ? 2 : -2;
                    transform.localScale = scale;
                }
            }

            if (transform.position.y > maxPlayerHeight)
                transform.position = new Vector2(transform.position.x, maxPlayerHeight);

            if (transform.position.y < minPlayerHeight)
                transform.position = new Vector2(transform.position.x, minPlayerHeight);
        }
        else
        {
            rb.gravityScale = 1;
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
                Vector3 scale = Vector3.one * 2;
                scale.x = facingRight ? 2 : -2;
                transform.localScale = scale;

            }

            //Jump
            if (playerMovement.y > 0 && grounded)
            {
                Debug.Log("Jump");
                grounded = false;
                rb.velocity = new Vector2(rb.velocity.x, moveSpeed * 2);
            }

        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            Debug.Log(collision.transform.parent);
            grounded = true;
        }

        if(collision.transform.tag == "Right Wall")
        {
            Debug.Log("Right Wall!");
            onRightWall = true;
        }
        
        if(collision.transform.tag == "Left Wall")
        {
            Debug.Log("Left Wall!");
            onLeftWall = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.tag == "Right Wall")
            onRightWall = false;

        if (collision.transform.tag == "Left Wall")
            onLeftWall = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Hole Entrance")
        {
            falling = true;
            grounded = false;

            LevelManager.ChangeLevelSection(LevelSection.CRUST);
        }


    }
}
