using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    float moveSpeed = 3f;


    Controls controls;
    Rigidbody2D rb;

    bool facingRight;
    bool grounded = true;
    bool falling;

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
                rb.velocity = playerMovement * moveSpeed;
                if (playerMovement.x != 0)
                {
                    facingRight = playerMovement.x == 1 ? true : false;
                    transform.localScale = new Vector3(playerMovement.x, 1, 1);
                }
            }
            else
                rb.velocity = Vector2.zero;
        }
        else
        {
            rb.gravityScale = 1;
            if (playerMovement.x != 0)
            {
                rb.velocity = new Vector2(playerMovement.x * moveSpeed, rb.velocity.y);
            }
            else if (grounded)
            {
                rb.velocity = Vector2.zero;
            }

            if (playerMovement.y > 0 && grounded)
            {
                grounded = false;
                rb.AddForce(new Vector2(0, moveSpeed * 2));
            }

        }
        



    }
}
