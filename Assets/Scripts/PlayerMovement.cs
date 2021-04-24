using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float moveSpeed = 3f;


    Controls controls;
    Rigidbody2D rb;

    bool facingRight;
    bool grounded;
    bool falling;
    bool onRightWall;
    bool onLeftWall;

    // Start is called before the first frame update
    void Start()
    {
        controls = new Controls();
        controls.Enable();
        rb = this.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {

        Vector2 playerMovement = controls.Gameplay.Movement.ReadValue<Vector2>();

        if (falling)
        {
            rb.gravityScale = 0;
            //Directional movement
            if (playerMovement != Vector2.zero)
            {
                //Check movement
                Vector2 vel = Vector2.zero;
                if (playerMovement.x > 0 && !onRightWall || playerMovement.x < 0 && !onLeftWall)
                    vel.x = playerMovement.x * moveSpeed;
                vel.y = playerMovement.y * moveSpeed;
                rb.velocity = vel;

                //Check facing
                if (playerMovement.x != 0)
                {
                    facingRight = playerMovement.x > 0 ? true : false;
                    Vector3 scale = Vector3.one;
                    scale.x = facingRight ? 1 : -1;
                    transform.localScale = scale;
                }
            }
        }
        else
        {
            rb.gravityScale = 1;
            rb.drag = 2;
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
                Vector3 scale = Vector3.one;
                scale.x = facingRight ? 1 : -1;
                transform.localScale = scale;

            }

            //Jump
            if (playerMovement.y > 0 && grounded)
            {
                grounded = false;
                rb.velocity = new Vector2(rb.velocity.x, moveSpeed * 2);
            }

        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Ground")
        {
            Debug.Log(collision.gameObject.GetComponentInParent<Transform>().name);
            if(!falling)
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
        }
    }
}
